// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Orang.FileSystem
{
    public class RenameOptions : ReplaceOptions
    {
        public RenameOptions(
            string replacement,
            ReplaceFunctions functions = ReplaceFunctions.None,
            bool cultureInvariant = false,
            ConflictResolution conflictResolution = ConflictResolution.Skip)
            : base(replacement, functions, cultureInvariant)
        {
            ConflictResolution = conflictResolution;
        }

        public RenameOptions(
            MatchEvaluator matchEvaluator,
            ReplaceFunctions functions = ReplaceFunctions.None,
            bool cultureInvariant = false,
            ConflictResolution conflictResolution = ConflictResolution.Skip)
            : base(matchEvaluator, functions, cultureInvariant)
        {
            ConflictResolution = conflictResolution;
        }

        public ConflictResolution ConflictResolution { get; }
    }
}
