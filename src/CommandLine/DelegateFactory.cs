// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class DelegateFactory
    {
        public static bool TryCreateMatchEvaluator<TDelegate>(string path, out TDelegate? result) where TDelegate : Delegate
        {
            return TryCreate(path, new Type[] { typeof(Match) }, out result);
        }

        public static bool TryCreate<TDelegate>(string path, Type[] parameters, out TDelegate? result) where TDelegate : Delegate
        {
            result = default;

            if (path == null)
                return true;

            int index = path.LastIndexOf(',');

            if (index <= 0
                || index >= path.Length - 1)
            {
                WriteError($"Invalid value: {path}. The expected format is \"MyLib.dll,MyNamespace.MyClass.MyMethod\".");
                return false;
            }

            string assemblyName = path.Substring(0, index);

            string methodFullName = path.Substring(index + 1);

            index = methodFullName.LastIndexOf('.');

            if (index < 0
                || index >= path.Length - 1)
            {
                WriteError($"Invalid method full name: {methodFullName}. The expected format is \"MyNamespace.MyClass.MyMethod\".");
                return false;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                string typeName = methodFullName.Substring(0, index);

                Type? type = assembly.GetType(typeName);

                if (type == null)
                {
                    WriteError($"Cannot find type '{typeName}' in assembly '{assembly.FullName}'");
                    return false;
                }

                string methodName = methodFullName.Substring(index + 1);
                MethodInfo? method = type.GetMethod(methodName, parameters);

                if (method == null)
                {
                    WriteError($"Cannot find method '{methodName}' in type '{typeName}'");
                    return false;
                }

                if (method.IsStatic)
                {
                    result = (TDelegate)method.CreateDelegate(typeof(TDelegate));
                }
                else
                {
                    object? typeInstance = Activator.CreateInstance(type);

                    if (typeInstance == null)
                    {
                        WriteError($"Cannot create instance of '{typeName}'");
                        return false;
                    }

                    result = (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), typeInstance, methodName);
                }

                return true;
            }
            catch (Exception ex) when (ex is ArgumentException
                || ex is IOException
                || ex is BadImageFormatException
                || ex is AmbiguousMatchException
                || ex is TargetInvocationException
                || ex is MemberAccessException
                || ex is MissingMemberException)
            {
                WriteError(ex, $"Cannot create delegate: {ex.Message}");
                return false;
            }
        }
    }
}
