// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Orang.Spelling;

namespace Orang.CommandLine
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SpellingFixResult
    {
        private int _lineStartIndex = -1;
        private int _lineEndIndex = -1;

        public SpellingFixResult(
            string input,
            SpellingCapture capture,
            SpellingFix fix,
            int? lineNumber = null,
            string? filePath = null)
        {
            Input = input;
            Capture = capture;
            Fix = fix;
            FilePath = filePath;
            LineNumber = lineNumber;
        }

        public string Input { get; }

        public SpellingCapture Capture { get; }

        public string Value => Capture.Value;

        public int Index => Capture.Index;

        public int Length => Capture.Length;

        public string? ContainingValue => Capture.ContainingValue;

        public int ContainingValueIndex => Capture.ContainingValueIndex;

        public SpellingFix Fix { get; }

        public string Replacement => Fix.Value;

        public SpellingFixKind Kind => Fix.Kind;

        public bool HasFix => !Fix.IsDefault;

        public int? LineNumber { get; }

        public string? FilePath { get; }

        public int LineStartIndex
        {
            get
            {
                if (_lineStartIndex == -1)
                    _lineStartIndex = TextUtility.GetLineStartIndex(Input, Index);

                return _lineStartIndex;
            }
        }

        public int LineEndIndex
        {
            get
            {
                if (_lineEndIndex == -1)
                    _lineEndIndex = TextUtility.GetLineEndIndex(Input, Index + Length);

                return _lineEndIndex;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Value}  {FilePath}";
    }
}
