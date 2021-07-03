// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Orang.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal abstract class AskReplacementWriter : ContentWriter, IReportReplacement
    {
        private static readonly char[] _newLineChars = new[] { '\r', '\n' };

        protected readonly Lazy<TextWriter>? _lazyWriter;
        protected int _writerIndex;
        private ValueWriter? _valueWriter;

        protected AskReplacementWriter(
            string input,
            IReplacer replacer,
            Lazy<TextWriter>? lazyWriter,
            ContentWriterOptions options,
            bool isInteractive,
            SpellcheckState? spellcheckState = null) : base(input, options)
        {
            Replacer = replacer;
            IsInteractive = isInteractive;
            SpellcheckState = spellcheckState;

            _lazyWriter = lazyWriter;
        }

        public IReplacer Replacer { get; }

        public int ReplacementCount { get; private set; }

        public bool ContinueWithoutAsking { get; private set; }

        public bool IsInteractive { get; }

        public SpellcheckState? SpellcheckState { get; }

        public static AskReplacementWriter Create(
            ContentDisplayStyle contentDisplayStyle,
            string input,
            IReplacer replacer,
            Lazy<TextWriter>? lazyWriter,
            ContentWriterOptions options,
            MatchOutputInfo? outputInfo,
            bool isInteractive,
            SpellcheckState? spellcheckState = null)
        {
            switch (contentDisplayStyle)
            {
                case ContentDisplayStyle.Value:
                case ContentDisplayStyle.ValueDetail:
                    return new AskValueReplacementWriter(
                        input,
                        replacer,
                        lazyWriter,
                        options,
                        outputInfo,
                        isInteractive,
                        spellcheckState);
                case ContentDisplayStyle.Line:
                    return new AskLineReplacementWriter(input, replacer, lazyWriter, options, isInteractive, spellcheckState);
                case ContentDisplayStyle.UnmatchedLines:
                case ContentDisplayStyle.AllLines:
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException($"Unknown enum value '{contentDisplayStyle}'.");
            }
        }

        protected override void WriteMatch(ICapture capture)
        {
            if (SpellcheckState?.Data.IgnoredValues.Contains(capture.Value) == true)
                return;

            base.WriteMatch(capture);
        }

        protected override void WriteEndReplacement(ICapture capture, string? result)
        {
            bool isUserInput = IsInteractive
                && (capture is RegexCapture
                    || result == null)
                && (result == null
                    || result.IndexOfAny(_newLineChars) == -1);

            string replacement = result ?? capture.Value;

            if (isUserInput)
                replacement = ConsoleHelpers.ReadUserInput(replacement, Options.Indent + "Replacement: ");

            if (!string.Equals(capture.Value, replacement, StringComparison.Ordinal))
            {
                if (_lazyWriter != null)
                {
                    if (IsInteractive
                        || ConsoleHelpers.AskToExecute("Replace?", Options.Indent))
                    {
                        _lazyWriter.Value.Write(Input.AsSpan(_writerIndex, capture.Index - _writerIndex));
                        _lazyWriter.Value.Write(replacement);

                        _writerIndex = capture.Index + capture.Length;

                        ReplacementCount++;
                    }
                }
                else if (!ContinueWithoutAsking
                    && !IsInteractive
                    && ConsoleHelpers.AskToContinue(Options.Indent) == DialogResult.YesToAll)
                {
                    ContinueWithoutAsking = true;
                }
            }

            SpellcheckState?.ProcessReplacement(
                Input,
                capture,
                replacement,
                lineNumber: (ValueWriter as LineNumberValueWriter)?.LineNumber,
                isUserInput: true);
        }

        protected override void WriteStartMatches()
        {
            ReplacementCount = 0;
            _writerIndex = 0;
        }

        protected override void WriteNonEmptyMatchValue(ICapture capture)
        {
            if (Options.HighlightMatch)
                base.WriteNonEmptyMatchValue(capture);
        }

        protected override void WriteEndMatch(ICapture capture)
        {
            string result = Replacer.Replace(capture);

            WriteReplacement(capture, result);
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteStartReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndMatches()
        {
            if (_lazyWriter?.IsValueCreated == true)
                _lazyWriter.Value.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));
        }

        public override void Dispose()
        {
            if (_lazyWriter?.IsValueCreated == true)
                _lazyWriter.Value.Dispose();
        }

        private class AskValueReplacementWriter : AskReplacementWriter
        {
            public AskValueReplacementWriter(
                string input,
                IReplacer replacer,
                Lazy<TextWriter>? lazyWriter,
                ContentWriterOptions options,
                MatchOutputInfo? outputInfo,
                bool isInteractive,
                SpellcheckState? spellcheckState = null) : base(input, replacer, lazyWriter, options, isInteractive, spellcheckState)
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

            protected override void WriteStartMatch(ICapture capture)
            {
                Write(Options.Indent);

                if (OutputInfo != null)
                    Write(OutputInfo.GetText(capture, MatchCount + 1, groupName: Options.GroupName));
            }

            protected override void WriteEndReplacement(ICapture capture, string? result)
            {
                WriteLine();

                base.WriteEndReplacement(capture, result);
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
                IReplacer replacer,
                Lazy<TextWriter>? lazyWriter,
                ContentWriterOptions options,
                bool isInteractive,
                SpellcheckState? spellcheckState = null) : base(input, replacer, lazyWriter, options, isInteractive, spellcheckState)
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

            protected override void WriteStartMatch(ICapture capture)
            {
                if (Options.IncludeLineNumber)
                {
                    _lineNumber += TextHelpers.CountLines(Input, _solIndex, capture.Index - _solIndex);
                    ((LineNumberValueWriter)ValueWriter).LineNumber = _lineNumber;
                }

                _solIndex = FindStartOfLine(capture);

                if (Options.ContextBefore > 0)
                    WriteContextBefore(0, _solIndex, _lineNumber);

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

                _eolIndex = FindEndOfLine(capture);

                WriteStartLine(_solIndex, capture.Index);
            }

            protected override void WriteNonEmptyReplacementValue(
                string result,
                in ConsoleColors colors,
                in ConsoleColors boundaryColors)
            {
                ReplacementValueWriter.Write(result, Symbols, colors, boundaryColors);
            }

            protected override void WriteEndReplacement(ICapture capture, string? result)
            {
                int endIndex = capture.Index + capture.Length;

                int eolIndex = FindEndOfLine(capture);

                WriteEndLine(endIndex, eolIndex);

                if (Options.ContextAfter > 0)
                    WriteContextAfter(eolIndex, Input.Length, _lineNumber);

                base.WriteEndReplacement(capture, result);
            }
        }
    }
}
