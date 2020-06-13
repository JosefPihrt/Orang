// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using static Orang.FileSystem.FileSystemHelpers;

namespace Orang.FileSystem
{
    //TODO: PathSpan, FilePathSpan
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct FileNameSpan
    {
        internal FileNameSpan(string path, int start, int length, FileNamePart part)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), start, null);

            if (start + length > path.Length)
                throw new ArgumentOutOfRangeException(nameof(length), length, null);

            Start = start;
            Length = length;
            Part = part;
        }

        public string Path { get; }

        public int Start { get; }

        public int Length { get; }

        public FileNamePart Part { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Part} Index = {Start} Length = {Length} {Path}";

        public override string ToString() => Path.Substring(Start, Length);

        public static FileNameSpan FromFile(string path, FileNamePart part)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            switch (part)
            {
                case FileNamePart.Name:
                    {
                        int index = GetFileNameIndex(path);

                        return new FileNameSpan(path, index, path.Length - index, part);
                    }
                case FileNamePart.FullName:
                    {
                        return new FileNameSpan(path, 0, path.Length, part);
                    }
                case FileNamePart.NameWithoutExtension:
                    {
                        int index = GetFileNameIndex(path);

                        int dotIndex = GetExtensionIndex(path);

                        return new FileNameSpan(path, index, dotIndex - index, part);
                    }
                case FileNamePart.Extension:
                    {
                        int dotIndex = GetExtensionIndex(path);

                        if (dotIndex >= path.Length - 1)
                        {
                            return new FileNameSpan(path, path.Length, 0, part);
                        }

                        return new FileNameSpan(path, dotIndex + 1, path.Length - dotIndex - 1, part);
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(part));
                    }
            }
        }

        public static FileNameSpan FromDirectory(string path, FileNamePart part)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            switch (part)
            {
                case FileNamePart.Name:
                case FileNamePart.NameWithoutExtension:
                    {
                        int index = GetFileNameIndex(path);

                        return new FileNameSpan(path, index, path.Length - index, part);
                    }
                case FileNamePart.FullName:
                    {
                        return new FileNameSpan(path, 0, path.Length, part);
                    }
                //case NamePartKind.Extension:
                default:
                    {
                        throw new ArgumentException("", nameof(part));
                    }
            }
        }
    }
}
