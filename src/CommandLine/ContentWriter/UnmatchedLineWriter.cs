// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.CommandLine
{
    internal class UnmatchedLineWriter : ContentWriter
    {
        private int _lastEndIndex;
        private int _lineNumber;

        public UnmatchedLineWriter(
            string input,
            ContentWriterOptions options,
            IResultStorage? storage = null) : this(input, ContentTextWriter.Default, options, storage)
        {
        }

        public UnmatchedLineWriter(
            string input,
            ContentTextWriter? writer,
            ContentWriterOptions options,
            IResultStorage? storage = null) : base(input, writer, options)
        {
            ResultStorage = storage;
            MatchingLineCount = 0;
        }

        public IResultStorage? ResultStorage { get; }

        protected override ValueWriter ValueWriter => throw new NotSupportedException();

        protected override void WriteStartMatch(ICapture capture) => throw new NotSupportedException();

        protected override void WriteEndMatch(ICapture capture) => throw new NotSupportedException();

        protected override void WriteStartMatches()
        {
            _lastEndIndex = 0;
            _lineNumber = 1;
        }

        protected override void WriteMatch(ICapture capture)
        {
            int index = capture.Index;

            if (index >= _lastEndIndex)
            {
                int solIndex = FindStartOfLine(index);

                if (solIndex > _lastEndIndex)
                {
                    if (solIndex > 0)
                    {
                        solIndex--;

                        if (solIndex > 0
                            && Input[solIndex - 1] == '\r')
                        {
                            solIndex--;
                        }
                    }

                    WriteUnmatchedLines(solIndex);
                }

                _lineNumber++;
            }

            if (Options.IncludeLineNumber)
                _lineNumber += TextHelpers.CountLines(Input, index, capture.Length);

            int i = index + capture.Length;
            while (i < Input.Length)
            {
                if (Input[i] == '\n')
                {
                    i++;
                    break;
                }

                i++;
            }

            _lastEndIndex = i;
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteStartReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndMatches()
        {
            WriteUnmatchedLines(Input.Length);
        }

        public override void Dispose()
        {
        }

        private void WriteUnmatchedLines(int index)
        {
            Write(Options.Indent);

            if (Options.IncludeLineNumber)
                WriteLineNumber(_lineNumber);

            WriteValue(_lastEndIndex, index - _lastEndIndex);
            WriteLine();
            _lineNumber++;
            MatchingLineCount++;

            if (ResultStorage != null)
            {
                int lastPos = _lastEndIndex;

                for (int i = _lastEndIndex; i < index; i++)
                {
                    switch (Input[i])
                    {
                        case '\r':
                            {
                                ResultStorage.Add(Input, lastPos, i - lastPos);

                                if (i < Input.Length - 1
                                    && Input[i + 1] == '\n')
                                {
                                    i++;
                                }

                                lastPos = i + 1;
                                break;
                            }
                        case '\n':
                            {
                                ResultStorage.Add(Input, lastPos, i - lastPos);

                                lastPos = i + 1;
                                break;
                            }
                    }
                }

                ResultStorage.Add(Input, lastPos, index - lastPos);
            }
        }

        private void WriteValue(int startIndex, int length)
        {
            int lastPos = startIndex;
            int endIndex = startIndex + length - 1;

            for (int i = startIndex; i <= endIndex; i++)
            {
                switch (Input[i])
                {
                    case (char)10:
                        {
                            WriteEndOfLine(i);
                            break;
                        }
                    case (char)13:
                        {
                            if (i < Input.Length - 1
                                && Input[i + 1] == '\n')
                            {
                                i++;
                                WriteEndOfLine(i);
                            }

                            break;
                        }
                }
            }

            Write(Input, lastPos, startIndex + length - lastPos);

            void WriteEndOfLine(int index)
            {
                index++;
                Write(Input, lastPos, index - lastPos);
                lastPos = index;

                _lineNumber++;
                MatchingLineCount++;

                Write(Options.Indent);

                if (Options.IncludeLineNumber)
                    WriteLineNumber(_lineNumber);
            }
        }
    }
}
