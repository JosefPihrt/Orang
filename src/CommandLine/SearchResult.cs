// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SearchResult
    {
        private long? _size;

        public SearchResult(
            FileMatch fileMatch,
            string? baseDirectoryPath,
            ContentWriterOptions? writerOptions = null)
        {
            FileMatch = fileMatch;
            BaseDirectoryPath = baseDirectoryPath;
            WriterOptions = writerOptions;
        }

        public FileMatch FileMatch { get; }

        public string? BaseDirectoryPath { get; }

        public ContentWriterOptions? WriterOptions { get; }

        public string Path => FileMatch.Path;

        public bool IsDirectory => FileMatch.IsDirectory;

        public Match? ContentMatch => FileMatch.ContentMatch;

        public string ContentText => FileMatch.ContentText;

        public FileSystemInfo FileSystemInfo => FileMatch.FileSystemInfo;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Path}";

        public long GetSize()
        {
            return _size
                ?? (_size = (IsDirectory)
                    ? FileSystemHelpers.GetDirectorySize(Path)
                    : ((FileInfo)FileSystemInfo).Length).Value;
        }
    }
}
