// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal abstract class CommonCopyCommandOptions : CommonFindCommandOptions
    {
        protected CommonCopyCommandOptions()
        {
        }

        public TimeSpan AllowedTimeDiff { get; internal set; }

        public FileAttributes NoCompareAttributes { get; internal set; }

        public FileCompareOptions CompareOptions { get; internal set; }

        public bool DryRun { get; internal set; }

        public bool Flat { get; internal set; }

        public bool StructureOnly { get; internal set; }

        public string Target { get; internal set; } = null!;

        public ConflictResolution ConflictResolution { get; internal set; }
    }
}
