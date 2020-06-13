// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Orang.FileSystem
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SearchProgress
    {
        internal SearchProgress(string path, SearchProgressKind kind, bool isDirectory, Exception? exception = null)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Kind = kind;
            Exception = exception;
            IsDirectory = isDirectory;
        }

        public string Path { get; }

        public SearchProgressKind Kind { get; }

        public Exception? Exception { get; }

        public bool IsDirectory { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return (Path != null)
                    ? $"{Kind}  {nameof(IsDirectory)} = {IsDirectory}  {Path}"
                    : "Uninitialized";
            }
        }
    }
}
