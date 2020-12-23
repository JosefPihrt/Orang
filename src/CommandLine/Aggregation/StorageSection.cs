// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Orang.FileSystem;

namespace Orang.Aggregation
{
    internal readonly struct StorageSection
    {
        public StorageSection(FileMatch fileMatch, string? baseDirectoryPath, int count)
        {
            FileMatch = fileMatch;
            BaseDirectoryPath = baseDirectoryPath;
            Count = count;
        }

        public FileMatch FileMatch { get; }

        public string? BaseDirectoryPath { get; }

        public int Count { get; }
    }
}
