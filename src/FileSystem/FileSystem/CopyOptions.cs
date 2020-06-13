// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Orang.FileSystem
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CopyOptions
    {
        internal static CopyOptions Default { get; } = new CopyOptions();

        public CopyOptions(
            ConflictResolution conflictResolution = ConflictResolution.Skip,
            FileCompareOptions compareOptions = FileCompareOptions.None,
            bool flat = false)
        {
            ConflictResolution = conflictResolution;
            CompareOptions = compareOptions;
            Flat = flat;
        }

        public ConflictResolution ConflictResolution { get; }

        public FileCompareOptions CompareOptions { get; }

        public bool Flat { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{nameof(ConflictResolution)} = {ConflictResolution}  {nameof(CompareOptions)} = {CompareOptions}";
    }
}
