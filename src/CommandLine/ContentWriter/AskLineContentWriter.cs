// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal class AskLineContentWriter : LineContentWriter
    {
        public AskLineContentWriter(
            string input,
            ContentWriterOptions options,
            IResultStorage? storage = null) : base(input, options, storage)
        {
            Ask = true;
        }

        public bool Ask { get; set; }

        protected override void WriteStartMatch(CaptureInfo capture)
        {
            ResultStorage?.Add(capture.Value);

            if (Options.IncludeLineNumber)
            {
                _lineNumber += TextHelpers.CountLines(Input, _solIndex, capture.Index - _solIndex);
                ((LineNumberValueWriter)ValueWriter).LineNumber = _lineNumber;
            }

            int solIndex = FindStartOfLine(capture);

            if (Options.ContextBefore > 0)
                WriteContextBefore(0, solIndex);

            Write(Options.Indent);

            if (Options.IncludeLineNumber)
                WriteLineNumber(_lineNumber);

            if (capture.Index >= _eolIndex)
            {
                MatchingLineCount++;

                int length = capture.Length;

                if (length > 0)
                {
                    if (Input[capture.Index + length - 1] == '\n')
                        length--;

                    MatchingLineCount += TextHelpers.CountLines(Input, capture.Index, length);
                }
            }

            _solIndex = solIndex;
            _eolIndex = FindEndOfLine(capture);

            WriteStartLine(_solIndex, capture.Index);
        }

        protected override void WriteEndMatch(CaptureInfo capture)
        {
            int endIndex = capture.Index + capture.Length;

            int eolIndex = FindEndOfLine(capture);

            WriteEndLine(endIndex, eolIndex);

            if (Options.ContextAfter > 0)
                WriteContextAfter(eolIndex, Input.Length, _lineNumber);

            if (Ask
                && ConsoleHelpers.AskToContinue(Options.Indent))
            {
                Ask = false;
            }
        }

        protected override void WriteEndMatches()
        {
        }
    }
}
