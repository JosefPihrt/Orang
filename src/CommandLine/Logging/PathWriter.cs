// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;
using static Orang.Logger;

namespace Orang
{
    internal class PathWriter
    {
        public PathWriter(
            ConsoleColors pathColors,
            ConsoleColors matchColors = default,
            bool relativePath = false,
            string? indent = null,
            Verbosity verbosity = Verbosity.Minimal)
        {
            PathColors = pathColors;
            MatchColors = matchColors;
            RelativePath = relativePath;
            Indent = indent;
            Verbosity = verbosity;
        }

        public ConsoleColors BasePathColors { get; } = Colors.BasePath;

        public ConsoleColors PathColors { get; }

        public ConsoleColors MatchColors { get; }

        public bool RelativePath { get; }

        public string? Indent { get; }

        public Verbosity Verbosity { get; }

        public void WritePath(
            FileMatch fileMatch,
            string? basePath,
            string? indent = null)
        {
            Write(indent ?? Indent, Verbosity);

            string path = fileMatch.Path;

            if (RelativePath
                && string.Equals(path, basePath, FileSystemHelpers.Comparison))
            {
                Write(".", PathColors, Verbosity);
                return;
            }

            int matchIndex = fileMatch.Index;
            int startIndex = GetBasePathLength(path, basePath);

            if (matchIndex >= 0
                && !MatchColors.IsDefault)
            {
                if (matchIndex < startIndex)
                {
                    startIndex = matchIndex;

                    Write(path, 0, startIndex, BasePathColors, Verbosity);
                }
                else if (!RelativePath)
                {
                    Write(path, 0, startIndex, BasePathColors, Verbosity);
                }

                int matchLength = fileMatch.Length;

                Write(path, startIndex, matchIndex - startIndex, colors: PathColors, Verbosity);
                Write(path, matchIndex, matchLength, MatchColors, Verbosity);
                Write(path, matchIndex + matchLength, path.Length - matchIndex - matchLength, colors: PathColors, Verbosity);
            }
            else
            {
                if (!RelativePath)
                    Write(path, 0, startIndex, BasePathColors, Verbosity);

                Write(path, startIndex, path.Length - startIndex, colors: PathColors, Verbosity);
            }
        }

        public void WritePath(
            string path,
            string? basePath,
            string? indent = null)
        {
            Write(indent ?? Indent, Verbosity);

            if (RelativePath
                && string.Equals(path, basePath, FileSystemHelpers.Comparison))
            {
                Write(".", PathColors, Verbosity);
            }
            else
            {
                int startIndex = GetBasePathLength(path, basePath);

                if (!RelativePath)
                    Write(path, 0, startIndex, BasePathColors, Verbosity);

                Write(path, startIndex, path.Length - startIndex, colors: PathColors, Verbosity);
            }
        }

        public void WritePath(
            FileMatch fileMatch,
            List<ReplaceItem> items,
            ConsoleColors replacementColors,
            string? basePath,
            string? indent = null)
        {
            Write(indent ?? Indent, Verbosity);

            string path = fileMatch.Path;
            int startIndex = GetBasePathLength(path, basePath);

            if (!RelativePath)
                Write(path, 0, startIndex, Colors.BasePath, Verbosity);

            foreach (ReplaceItem item in items)
            {
                Match match = item.Match;

                Write(path, startIndex, fileMatch.NameSpan.Start + match.Index - startIndex, Verbosity);

                if (!MatchColors.IsDefault)
                    Write(match.Value, MatchColors, Verbosity);

                Write(item.Value, replacementColors, Verbosity);

                startIndex = fileMatch.NameSpan.Start + match.Index + match.Length;
            }

            Write(path, startIndex, path.Length - startIndex, Verbosity);
        }

        protected int GetBasePathLength(string path, string? basePath)
        {
            if (basePath != null
                && path.Length > basePath.Length
                && path.StartsWith(basePath, FileSystemHelpers.Comparison))
            {
                int length = basePath.Length;

                if (FileSystemHelpers.IsDirectorySeparator(path[length]))
                    length++;

                return length;
            }

            return 0;
        }
    }
}
