// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Orang.CommandLine
{
    internal class ValueReplacementWriter : ValueContentWriter, IReportReplacement
    {
        private readonly TextWriter? _textWriter;
        private int _writerIndex;
        private ValueWriter? _valueWriter;

        public ValueReplacementWriter(
            string input,
            IReplacer replacer,
            ContentWriterOptions options,
            TextWriter? textWriter = null,
            MatchOutputInfo? outputInfo = null,
            SpellcheckState? spellcheckState = null) : base(input, options, outputInfo: outputInfo)
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
                    string infoIndent = Options.Indent + new string(' ', OutputInfo?.Width ?? 0);

                    _valueWriter = new ValueWriter(Writer, infoIndent);
                }

                return _valueWriter;
            }
        }

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

            SpellcheckState?.ProcessReplacement(Input, capture, result);

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
