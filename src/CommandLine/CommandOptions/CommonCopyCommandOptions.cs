// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal abstract class CommonCopyCommandOptions : CommonFindCommandOptions
    {
        private string _target = null!;

        protected CommonCopyCommandOptions()
        {
        }

        public FileCompareOptions CompareOptions { get; internal set; }

        public bool DryRun { get; internal set; }

        public bool Flat { get; internal set; }

        public string Target
        {
            get { return _target; }

            internal set
            {
                _target = value;
                TargetNormalized = null;
            }
        }

        public string? TargetNormalized { get; internal set; }

        public ConflictResolution ConflictResolution { get; internal set; }
    }
}
