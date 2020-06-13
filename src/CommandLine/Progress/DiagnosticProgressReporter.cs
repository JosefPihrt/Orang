// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Orang.CommandLine;
using static Orang.Logger;

namespace Orang.FileSystem
{
    internal class DiagnosticProgressReporter : DotProgressReporter
    {
        public DiagnosticProgressReporter(
            ProgressReportMode consoleReportMode,
            ProgressReportMode fileReportMode,
            FileSystemCommandOptions options,
            string indent) : base(indent)
        {
            ConsoleReportMode = consoleReportMode;
            FileReportMode = fileReportMode;
            Options = options;
        }

        public ProgressReportMode ConsoleReportMode { get; }

        public ProgressReportMode FileReportMode { get; }

        public FileSystemCommandOptions Options { get; }

        public override void Report(SearchProgress value)
        {
            if (value.Exception != null)
            {
                WriteError(value);
            }
            else
            {
                switch (value.Kind)
                {
                    case SearchProgressKind.SearchDirectory:
                        {
                            SearchedDirectoryCount++;

                            if (ConsoleReportMode == ProgressReportMode.Path)
                            {
                                WritePath(value.Path, value.Kind, Indent);
                            }
                            else if (FileReportMode == ProgressReportMode.Path)
                            {
                                WritePathToFile(value.Path, value.Kind);
                            }

                            break;
                        }
                    case SearchProgressKind.Directory:
                        {
                            DirectoryCount++;
                            WritePath(value.Path, value.Kind);
                            break;
                        }
                    case SearchProgressKind.File:
                        {
                            FileCount++;
                            WritePath(value.Path, value.Kind);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown enum value '{value.Kind}'.");
                        }
                }
            }
        }

        private void WritePath(string path, SearchProgressKind kind)
        {
            if (ConsoleReportMode == ProgressReportMode.Dot)
            {
                WriteProgress();

                if (FileReportMode == ProgressReportMode.Path)
                    WritePathToFile(path, kind, Indent);
            }
            else if (ConsoleReportMode == ProgressReportMode.Path)
            {
                WritePath(path, kind, Indent);
            }
            else if (FileReportMode == ProgressReportMode.Path)
            {
                WritePathToFile(path, kind, Indent);
            }
        }

        private void WritePathToFile(string path, SearchProgressKind kind, string? indent = null)
        {
            Out!.Write(indent);
            Out.Write(GetPrefix(kind));
            Out.WriteLine(GetPath(path));
        }

        private void WritePath(string path, SearchProgressKind kind, string? indent = null)
        {
            ReadOnlySpan<char> pathDisplay = GetPath(path);

            ConsoleOut.Write(indent, Colors.Path_Progress);
            ConsoleOut.Write(GetPrefix(kind), Colors.Path_Progress);
            ConsoleOut.WriteLine(pathDisplay, Colors.Path_Progress);

            if (FileReportMode == ProgressReportMode.Path)
            {
                Out!.Write(indent);
                Out.Write(GetPrefix(kind));
                Out.WriteLine(pathDisplay);
            }
        }

        private ReadOnlySpan<char> GetPath(string path)
        {
            return GetPath(path, BaseDirectoryPath, Options.DisplayRelativePath);
        }

        private static ReadOnlySpan<char> GetPath(
            string path,
            string? basePath,
            bool relativePath)
        {
            if (string.Equals(path, basePath, FileSystemHelpers.Comparison))
                return (relativePath) ? "." : path;

            if (relativePath
                && basePath != null
                && path.Length > basePath.Length
                && path.StartsWith(basePath, FileSystemHelpers.Comparison))
            {
                int startIndex = basePath.Length;

                if (FileSystemHelpers.IsDirectorySeparator(path[startIndex]))
                    startIndex++;

                return path.AsSpan(startIndex);
            }

            return path;
        }

        private static string GetPrefix(SearchProgressKind kind)
        {
            return kind switch
            {
                SearchProgressKind.SearchDirectory => "[S] ",
                SearchProgressKind.Directory => "[D] ",
                SearchProgressKind.File => "[F] ",
                _ => throw new InvalidOperationException($"Unknown enum value '{kind}'."),
            };
        }
    }
}
