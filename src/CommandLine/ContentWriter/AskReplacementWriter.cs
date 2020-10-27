// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal abstract class AskReplacementWriter : ContentWriter
    {
        protected readonly Lazy<TextWriter>? _lazyWriter;
        protected int _writerIndex;
        private ValueWriter? _valueWriter;

        protected AskReplacementWriter(
            string input,
            ReplaceOptions replaceOptions,
            Lazy<TextWriter>? lazyWriter,
            ContentWriterOptions options) : base(input, options)
        {
            ReplaceOptions = replaceOptions;
            _lazyWriter = lazyWriter;
        }

        public ReplaceOptions ReplaceOptions { get; }

        public int ReplacementCount { get; private set; }

        public bool ContinueWithoutAsking { get; private set; }

        public static AskReplacementWriter Create(
            ContentDisplayStyle contentDisplayStyle,
            string input,
            ReplaceOptions replaceOptions,
            Lazy<TextWriter>? lazyWriter,
            ContentWriterOptions options,
            MatchOutputInfo? outputInfo)
        {
            switch (contentDisplayStyle)
            {
                case ContentDisplayStyle.Value:
                case ContentDisplayStyle.ValueDetail:
                    return new AskValueReplacementWriter(input, replaceOptions, lazyWriter, options, outputInfo);
                case ContentDisplayStyle.Line:
                    return new AskLineReplacementWriter(input, replaceOptions, lazyWriter, options);
                case ContentDisplayStyle.UnmatchedLines:
                case ContentDisplayStyle.AllLines:
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException($"Unknown enum value '{contentDisplayStyle}'.");
            }
        }

        protected override void WriteEndReplacement(Match match, string result)
        {
            if (_lazyWriter != null)
            {
                if (ConsoleHelpers.AskToExecute("Replace?", Options.Indent))
                {
                    _lazyWriter.Value.Write(Input.AsSpan(_writerIndex, match.Index - _writerIndex));
                    _lazyWriter.Value.Write(result);

                    _writerIndex = match.Index + match.Length;

                    ReplacementCount++;
                }
            }
            else if (!ContinueWithoutAsking
                && ConsoleHelpers.AskToContinue(Options.Indent))
            {
                ContinueWithoutAsking = true;
            }
        }

        protected override void WriteStartMatches()
        {
            ReplacementCount = 0;
            _writerIndex = 0;
        }

        protected override void WriteNonEmptyMatchValue(Capture capture)
        {
            if (Options.HighlightMatch)
                base.WriteNonEmptyMatchValue(capture);
        }

        protected override void WriteEndMatch(Capture capture)
        {
            var match = (Match)capture;

            string result = ReplaceOptions.Replace(match);

            WriteReplacement(match, result);
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteStartReplacement(Match match, string result)
        {
        }

        protected override void WriteEndMatches()
        {
        }

        public override void Dispose()
        {
            if (_lazyWriter?.IsValueCreated == true)
            {
                _lazyWriter.Value.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));
                _lazyWriter.Value.Dispose();
            }
        }

        private class AskValueReplacementWriter : AskReplacementWriter
        {
            public AskValueReplacementWriter(
                string input,
                ReplaceOptions replaceOptions,
                Lazy<TextWriter>? lazyWriter,
                ContentWriterOptions options,
                MatchOutputInfo? outputInfo) : base(input, replaceOptions, lazyWriter, options)
            {
                OutputInfo = outputInfo;
            }

            public MatchOutputInfo? OutputInfo { get; }

            protected override ValueWriter ValueWriter
            {
                get
                {
                    if (_valueWriter == null)
                    {
                        string infoIndent = Options.Indent + new string(' ', OutputInfo?.Width ?? 0);

                        _valueWriter = new ValueWriter(Writer, infoIndent, includeEndingIndent: false);
                    }

                    return _valueWriter;
                }
            }

            protected override void WriteStartMatch(Capture capture)
            {
                Write(Options.Indent);

                if (OutputInfo != null)
                    Write(OutputInfo.GetText(capture, MatchCount + 1, groupName: Options.GroupName, captureNumber: -1));
            }

            protected override void WriteEndReplacement(Match match, string result)
            {
                WriteLine();

                base.WriteEndReplacement(match, result);
            }
        }

        private class AskLineReplacementWriter : AskReplacementWriter
        {
            private int _eolIndex;
            private int _solIndex;
            private int _lineNumber;
            private ValueWriter? _replacementValueWriter;

            public AskLineReplacementWriter(
                string input,
                ReplaceOptions replaceOptions,
                Lazy<TextWriter>? lazyWriter,
                ContentWriterOptions options) : base(input, replaceOptions, lazyWriter, options)
            {
                MatchingLineCount = 0;
            }

            protected override ValueWriter ValueWriter
            {
                get
                {
                    if (_valueWriter == null)
                    {
                        if (Options.IncludeLineNumber)
                        {
                            _valueWriter = new LineNumberValueWriter(Writer, Options.Indent, includeEndingLineNumber: false);
                        }
                        else
                        {
                            _valueWriter = new ValueWriter(Writer, Options.Indent);
                        }
                    }

                    return _valueWriter;
                }
            }

            private ValueWriter ReplacementValueWriter
                => _replacementValueWriter ??= new ValueWriter(Writer, Options.Indent, includeEndingIndent: false);

            protected override void WriteStartMatches()
            {
                _solIndex = 0;
                _lineNumber = 1;

                base.WriteStartMatches();
            }

            protected override void WriteStartMatch(Capture capture)
            {
                Write(Options.Indent);

                if (Options.IncludeLineNumber)
                {
                    _lineNumber += TextHelpers.CountLines(Input, _solIndex, capture.Index - _solIndex);
                    ((LineNumberValueWriter)ValueWriter).LineNumber = _lineNumber;

                    WriteLineNumber(_lineNumber);
                }

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

                _solIndex = FindStartOfLine(capture);
                _eolIndex = FindEndOfLine(capture);

                WriteStartLine(_solIndex, capture.Index);
            }

            protected override void WriteNonEmptyReplacementValue(string result)
            {
                ReplacementValueWriter.Write(result, Symbols, ReplacementColors, ReplacementBoundaryColors);
            }

            protected override void WriteEndReplacement(Match match, string result)
            {
                int endIndex = match.Index + match.Length;

                int eolIndex = FindEndOfLine(match);

                WriteEndLine(endIndex, eolIndex);

                base.WriteEndReplacement(match, result);
            }
        }
    }
}
