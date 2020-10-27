// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Orang.CommandLine
{
    internal static class AssemblyFactory
    {
        public static Assembly? FromExpression(string expressionText)
        {
            ExpressionSyntax expression = ParseExpression(expressionText);

            CompilationUnitSyntax compilationUnit = CompilationUnit(
                default,
                default,
                default,
                SingletonList<MemberDeclarationSyntax>(
                    NamespaceDeclaration(
                        ParseName("Orang.Runtime"),
                        default,
                        List(new UsingDirectiveSyntax[]
                            {
                                UsingDirective(ParseName("System")),
                                UsingDirective(ParseName("System.Collections.Generic")),
                                UsingDirective(ParseName("System.Linq")),
                                UsingDirective(ParseName("System.Text")),
                                UsingDirective(ParseName("System.Text.RegularExpressions")),
                            }),
                        SingletonList<MemberDeclarationSyntax>(
                            ClassDeclaration(
                                default,
                                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
                                Identifier("EvaluatorClass"),
                                default,
                                default,
                                default,
                                SingletonList<MemberDeclarationSyntax>(
                                    MethodDeclaration(
                                        default,
                                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
                                        PredefinedType(Token(SyntaxKind.StringKeyword)),
                                        default!,
                                        Identifier("EvaluatorMethod"),
                                        default!,
                                        ParameterList(
                                            SingletonSeparatedList(
                                                Parameter(
                                                    default,
                                                    default,
                                                    ParseTypeName("Match"),
                                                    Identifier("match"),
                                                    default))),
                                        default,
                                        default,
                                        ArrowExpressionClause(expression),
                                        default(SyntaxToken))))))));

            SyntaxTree? syntaxTree = CSharpSyntaxTree.Create(compilationUnit);

            var namespaceDeclaration = (NamespaceDeclarationSyntax)compilationUnit.Members[0];
            var classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members[0];
            var methodDeclaration = (MethodDeclarationSyntax)classDeclaration.Members[0];

            expression = methodDeclaration.ExpressionBody!.Expression;

            return FromSyntaxTree(syntaxTree, expressionSpan: expression.Span);
        }

        public static Assembly? FromSourceText(string text)
        {
            return FromSyntaxTree(CSharpSyntaxTree.ParseText(text));
        }

        public static Assembly? FromSyntaxTree(SyntaxTree syntaxTree, TextSpan? expressionSpan = null)
        {
            string assemblyName = Path.GetRandomFileName();

            IEnumerable<MetadataReference> metadataReferences = RuntimeMetadataReference.DefaultAssemblyNames
                .Select(f => MetadataReference.CreateFromFile(RuntimeMetadataReference.TrustedPlatformAssemblyMap[f]))
                .Append(RuntimeMetadataReference.CorLibReference);

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new SyntaxTree[] { syntaxTree },
                references: metadataReferences,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();

            if (expressionSpan != null)
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error
                        && diagnostic.Location.Kind == LocationKind.SourceFile
                        && !expressionSpan.Value.Contains(diagnostic.Location.SourceSpan))
                    {
                        expressionSpan = null;
                        break;
                    }
                }
            }

            var hasDiagnostic = false;

            foreach (Diagnostic diagnostic in diagnostics.OrderBy(f => f.Location.SourceSpan))
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    WriteDiagnostic(diagnostic, expressionSpan);
                    hasDiagnostic = true;
                }
            }

            if (hasDiagnostic)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(memoryStream);

                if (!emitResult.Success)
                {
                    foreach (Diagnostic diagnostic in emitResult.Diagnostics.OrderBy(f => f.Location.SourceSpan))
                        WriteDiagnostic(diagnostic, expressionSpan);

                    return null;
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(memoryStream.ToArray());
            }
        }

        private static void WriteDiagnostic(Diagnostic diagnostic, TextSpan? expressionSpan = null)
        {
            string location;
            if (expressionSpan != null)
            {
                location = $"1,{diagnostic.Location.SourceSpan.Start - expressionSpan.Value.Start + 1}";
            }
            else
            {
                LinePosition start = diagnostic.Location.GetLineSpan().Span.Start;
                location = $"{start.Line + 1},{start.Character + 1}";
            }

            Logger.WriteError($"({location}): {diagnostic.Id}: {diagnostic.GetMessage()}");
        }
    }
}
