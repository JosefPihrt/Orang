// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Orang.CommandLine
{
    internal class LineReplacementWriter : LineContentWriter, IReportReplacement
    {
        private readonly TextWriter? _textWriter;
        private int _writerIndex;
        private ValueWriter? _valueWriter;
        private ValueWriter? _replacementValueWriter;

        public LineReplacementWriter(
            string input,
            IReplacer replacer,
            ContentWriterOptions options,
            TextWriter? textWriter = null,
            SpellcheckState? spellcheckState = null) : base(input, options)
        {
            Replacer = replacer;
            _textWriter = textWriter;
            SpellcheckState = spellcheckState;
        }

        public IReplacer Replacer { get; }

        public SpellcheckState? SpellcheckState { get; }

        public int ReplacementCount { get; private set; }

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
            ReplacementCount = 0;
            _writerIndex = 0;

            base.WriteStartMatches();
        }

        protected override void WriteNonEmptyMatchValue(ICapture capture)
        {
            if (Options.HighlightMatch)
                base.WriteNonEmptyMatchValue(capture);
        }

        protected override void WriteNonEmptyReplacementValue(
            string result,
            in ConsoleColors colors,
            in ConsoleColors boundaryColors)
        {
            ReplacementValueWriter.Write(result, Symbols, colors, boundaryColors);
        }

        protected override void WriteMatch(ICapture capture)
        {
            if (SpellcheckState?.Data.IgnoredValues.Contains(capture.Value) == true)
                return;

            base.WriteMatch(capture);
        }

        protected override void WriteEndMatch(ICapture capture)
        {
            string result = Replacer.Replace(capture);

            WriteReplacement(capture, result);

            base.WriteEndMatch(capture);
        }

        protected override void WriteEndReplacement(ICapture capture, string? result)
        {
            if (result != null)
            {
                _textWriter?.Write(Input.AsSpan(_writerIndex, capture.Index - _writerIndex));
                _textWriter?.Write(result);

                _writerIndex = capture.Index + capture.Length;

                ReplacementCount++;
            }

            SpellcheckState?.ProcessReplacement(Input, capture, result, lineNumber: (ValueWriter as LineNumberValueWriter)?.LineNumber);

            base.WriteEndReplacement(capture, result);
        }

        protected override void WriteEndMatches()
        {
            if (ReplacementCount > 0)
                _textWriter?.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));

            base.WriteEndMatches();
        }

        public override void Dispose()
        {
            if (ReplacementCount > 0)
                _textWriter?.Dispose();
        }
    }
}
