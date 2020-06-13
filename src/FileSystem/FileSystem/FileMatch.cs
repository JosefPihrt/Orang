// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Orang.FileSystem
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class FileMatch
    {
        private FileSystemInfo? _fileSystemInfo;

        internal FileMatch(in FileNameSpan nameSpan, Match? nameMatch, FileSystemInfo? fileSystemInfo = null, bool isDirectory = false)
            : this(nameSpan, nameMatch, default(FileContent), default(Match), fileSystemInfo, isDirectory)
        {
        }

        internal FileMatch(in FileNameSpan nameSpan, Match? nameMatch, in FileContent content, Match? contentMatch, FileSystemInfo? fileSystemInfo = null, bool isDirectory = false)
        {
            NameMatch = nameMatch;
            NameSpan = nameSpan;
            Content = content;
            ContentMatch = contentMatch;
            IsDirectory = isDirectory;
            _fileSystemInfo = fileSystemInfo;
        }

        public FileNameSpan NameSpan { get; }

        public Match? NameMatch { get; }

        public FileContent Content { get; }

        public Match? ContentMatch { get; }

        public bool IsDirectory { get; }

        internal FileSystemInfo FileSystemInfo
        {
            get
            {
                if (_fileSystemInfo == null)
                {
                    if (IsDirectory)
                    {
                        _fileSystemInfo = new DirectoryInfo(Path);
                    }
                    else
                    {
                        _fileSystemInfo = new FileInfo(Path);
                    }
                }

                return _fileSystemInfo;
            }
        }

        public string Path => NameSpan.Path;

        internal int Index => (NameMatch?.Success == true) ? NameMatch.Index : -1;

        internal int Length => (NameMatch?.Success == true) ? NameMatch.Length : -1;

        internal string ContentText => Content.Text;

        internal Encoding Encoding => Content.Encoding;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Path}";
    }
}
