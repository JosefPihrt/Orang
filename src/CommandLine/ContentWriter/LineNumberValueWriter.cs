// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class LineNumberValueWriter : ValueWriter
    {
        public LineNumberValueWriter(
            ContentTextWriter? writer,
            string? indent = null,
            bool includeEndingIndent = true,
            bool includeEndingLineNumber = true) : base(writer, indent: indent, includeEndingIndent: includeEndingIndent)
        {
            LineNumber = 1;
            IncludeEndingLineNumber = includeEndingLineNumber;
        }

        public int LineNumber { get; set; }

        public bool IncludeEndingLineNumber { get; }

        public bool PendingLineNumber { get; set; }

        protected override void WriteEndOfLine(string value, int index, int endIndex, OutputSymbols symbols)
        {
            LineNumber++;

            WriteNewLine(value, index);

            if (IncludeEndingIndent
                || index < endIndex)
            {
                Write(Indent);

                if (IncludeEndingLineNumber
                    || index < endIndex)
                {
                    Write(LineNumber.ToString(), Colors.LineNumber);
                    Write(" ");
                }
                else
                {
                    PendingLineNumber = true;
                }
            }
        }
    }
}
