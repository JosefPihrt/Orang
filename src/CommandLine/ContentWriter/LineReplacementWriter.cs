// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal class LineReplacementWriter : LineContentWriter
    {
        private readonly TextWriter? _textWriter;
        private int _writerIndex;
        private ValueWriter? _valueWriter;
        private ValueWriter? _replacementValueWriter;

        public LineReplacementWriter(
            string input,
            ReplaceOptions replaceOptions,
            ContentWriterOptions options,
            TextWriter? textWriter = null) : base(input, options)
        {
            ReplaceOptions = replaceOptions;
            _textWriter = textWriter;
        }

        public ReplaceOptions ReplaceOptions { get; }

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
            _writerIndex = 0;

            base.WriteStartMatches();
        }

        protected override void WriteNonEmptyMatchValue(Capture capture)
        {
            if (Options.HighlightMatch)
                base.WriteNonEmptyMatchValue(capture);
        }

        protected override void WriteNonEmptyReplacementValue(string result)
        {
            ReplacementValueWriter.Write(result, Symbols, ReplacementColors, ReplacementBoundaryColors);
        }

        protected override void WriteEndMatch(Capture capture)
        {
            var match = (Match)capture;

            string result = ReplaceOptions.Replace(match);

            WriteReplacement(match, result);

            base.WriteEndMatch(capture);
        }

        protected override void WriteEndReplacement(Match match, string result)
        {
            _textWriter?.Write(Input.AsSpan(_writerIndex, match.Index - _writerIndex));
            _textWriter?.Write(result);

            _writerIndex = match.Index + match.Length;

            base.WriteEndReplacement(match, result);
        }

        protected override void WriteEndMatches()
        {
            _textWriter?.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));

            base.WriteEndMatches();
        }

        public override void Dispose()
        {
            _textWriter?.Dispose();
        }
    }
}
