// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Orang.CommandLine
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class NewWord
    {
        public NewWord(
            string value,
            string line,
            int lineCharIndex,
            int? lineNumber = null,
            string? filePath = null,
            string? containingValue = null)
        {
            Value = value;
            Line = line;
            LineCharIndex = lineCharIndex;
            LineNumber = lineNumber;
            FilePath = filePath;
            ContainingValue = containingValue;
        }

        public string Value { get; }

        public string Line { get; }

        public int LineCharIndex { get; }

        public int? LineNumber { get; }

        public string? FilePath { get; }

        public string? ContainingValue { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Value}  {FilePath}";
    }
}
