// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Orang.FileSystem;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class DelegateFactory
    {
        public static bool TryCreateFromExpression<TDelegate>(
            string expressionText,
            string className,
            string methodName,
            string returnTypeName,
            Type returnType,
            string parameterTypeName,
            Type parameterType,
            string parameterName,
            [NotNullWhen(true)] out TDelegate? result) where TDelegate : Delegate
        {
            Assembly? assembly = AssemblyFactory.FromExpression(
                expressionText,
                className,
                methodName,
                returnTypeName,
                parameterTypeName,
                parameterName);

            if (assembly == null)
            {
                result = null;
                return false;
            }

            result = CreateDelegateAndCatchIfThrows<TDelegate>(
                assembly,
                returnType,
                new Type[] { parameterType },
                $"Orang.Runtime.{className}",
                methodName);

            return result != null;
        }

        public static bool TryCreateFromSourceText<TDelegate>(
            string sourceText,
            Type returnType,
            Type parameterType,
            [NotNullWhen(true)] out TDelegate? result) where TDelegate : Delegate
        {
            Assembly? assembly = AssemblyFactory.FromSourceText(sourceText);

            if (assembly == null)
            {
                result = null;
                return false;
            }

            result = CreateDelegateAndCatchIfThrows<TDelegate>(assembly, returnType, new Type[] { parameterType });
            return result != null;
        }

        public static bool TryCreateFromCodeFile<TDelegate>(
            string filePath,
            Type returnType,
            Type parameterType,
            [NotNullWhen(true)] out TDelegate? result) where TDelegate : Delegate
        {
            if (!FileSystemHelpers.TryReadAllText(filePath, out string? content, f => WriteError(f)))
            {
                result = null;
                return false;
            }

            Assembly? assembly = AssemblyFactory.FromSourceText(content);

            if (assembly == null)
            {
                result = null;
                return false;
            }

            result = CreateDelegateAndCatchIfThrows<TDelegate>(assembly, returnType, new Type[] { parameterType });
            return result != null;
        }

        public static bool TryCreateFromAssembly<TDelegate>(
            string path,
            Type returnType,
            Type parameterType,
            [NotNullWhen(true)] out TDelegate? result) where TDelegate : Delegate
        {
            return TryCreateFromAssembly(path, returnType, new Type[] { parameterType }, out result);
        }

        private static bool TryCreateFromAssembly<TDelegate>(
            string path,
            Type returnType,
            Type[] parameters,
            [NotNullWhen(true)] out TDelegate? result) where TDelegate : Delegate
        {
            result = default;

            if (path == null)
                return false;

            int index = path.LastIndexOf(',');

            if (index <= 0
                || index >= path.Length - 1)
            {
                WriteError($"Invalid value: {path}. "
                    + "The expected format is \"MyLib.dll,MyNamespace.MyClass.MyMethod\".");

                return false;
            }

            string assemblyName = path.Substring(0, index);

            string methodFullName = path.Substring(index + 1);

            index = methodFullName.LastIndexOf('.');

            if (index < 0
                || index >= path.Length - 1)
            {
                WriteError($"Invalid method full name: {methodFullName}. "
                    + "The expected format is \"MyNamespace.MyClass.MyMethod\".");

                return false;
            }

            Assembly? assembly;

            try
            {
                assembly = Assembly.LoadFrom(assemblyName);
            }
            catch (Exception ex) when (ex is ArgumentException
                || ex is IOException
                || ex is BadImageFormatException)
            {
                WriteError(ex, ex.Message);
                return false;
            }

            string typeName = methodFullName.Substring(0, index);

            string methodName = methodFullName.Substring(index + 1);

            result = CreateDelegateAndCatchIfThrows<TDelegate>(assembly, returnType, parameters, typeName, methodName);

            return result != null;
        }

        private static TDelegate? CreateDelegateAndCatchIfThrows<TDelegate>(
            Assembly assembly,
            Type returnType,
            Type[] parameters,
            string? typeName = null,
            string? methodName = null) where TDelegate : Delegate
        {
            try
            {
                return CreateDelegate<TDelegate>(assembly, returnType, parameters, typeName, methodName);
            }
            catch (Exception ex) when (ex is ArgumentException
                || ex is AmbiguousMatchException
                || ex is TargetInvocationException
                || ex is MemberAccessException
                || ex is MissingMemberException
                || ex is TypeLoadException)
            {
                WriteError(ex, $"Cannot create delegate: {ex.Message}");
                return null;
            }
        }

        private static TDelegate? CreateDelegate<TDelegate>(
            Assembly assembly,
            Type returnType,
            Type[] parameters,
            string? typeName = null,
            string? methodName = null) where TDelegate : Delegate
        {
            Type? type;
            MethodInfo? method;

            if (typeName != null)
            {
                type = assembly.GetType(typeName);

                if (type == null)
                {
                    WriteError($"Cannot find type '{typeName}' in assembly '{assembly.FullName}'");
                    return null;
                }

                if (methodName != null)
                {
                    method = type.GetMethod(methodName, parameters);

                    if (method == null)
                    {
                        WriteError($"Cannot find method '{methodName}' in type '{typeName}'");
                        return null;
                    }
                }
                else
                {
                    method = FindMethod(type, returnType, parameters);

                    if (method == null)
                    {
                        WriteError("Cannot find public method with signature 'string M(Match)' in type "
                            + $"'{typeName}'");

                        return null;
                    }

                    methodName = method.Name;
                }
            }
            else
            {
                method = FindMethod(assembly, returnType, parameters, methodName);

                if (method == null)
                {
                    WriteError($"Cannot find public method with signature 'string {methodName ?? "M"}(Match)' in type "
                        + $"'{typeName}'");

                    return null;
                }

                methodName ??= method.Name;

                type = method.DeclaringType;
            }

            if (method.IsStatic)
            {
                return (TDelegate)method.CreateDelegate(typeof(TDelegate));
            }
            else
            {
                object? typeInstance = Activator.CreateInstance(type!);

                if (typeInstance == null)
                {
                    WriteError($"Cannot create instance of '{typeName}'");
                    return null;
                }

                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), typeInstance, methodName);
            }
        }

        private static MethodInfo? FindMethod(
            Assembly assembly,
            Type returnType,
            Type[] parameters,
            string? methodName = null)
        {
            foreach (Type type in assembly.GetTypes())
            {
                MethodInfo? method = (methodName != null)
                    ? type.GetMethod(methodName, parameters)
                    : FindMethod(type, returnType, parameters);

                if (method != null)
                    return method;
            }

            return null;
        }

        private static MethodInfo? FindMethod(Type type, Type returnType, Type[] parameters)
        {
            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                if (!methodInfo.IsPublic)
                    continue;

                if (methodInfo.ReturnType != returnType)
                    continue;

                ParameterInfo[]? parameters2 = methodInfo.GetParameters();

                if (parameters2.Length != parameters.Length)
                    continue;

                var areEqual = true;

                for (int i = 0; i < parameters2.Length; i++)
                {
                    if (parameters2[i].ParameterType != parameters[i])
                    {
                        areEqual = false;
                        break;
                    }
                }

                if (areEqual)
                    return methodInfo;
            }

            return null;
        }
    }
}
