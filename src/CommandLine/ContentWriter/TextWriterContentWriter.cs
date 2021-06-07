// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Orang.CommandLine
{
    internal class TextWriterContentWriter : ContentWriter, IReportReplacement
    {
        private readonly TextWriter? _textWriter;
        private int _writerIndex;

        public TextWriterContentWriter(
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

        protected override ValueWriter ValueWriter => throw new NotSupportedException();

        protected override void WriteStartMatches()
        {
            ReplacementCount = 0;
            _writerIndex = 0;
        }

        protected override void WriteStartMatch(ICapture capture)
        {
        }

        protected override void WriteMatch(ICapture capture)
        {
            if (SpellcheckState?.Data.IgnoredValues.Contains(capture.Value) == true)
                return;

            string result = Replacer.Replace(capture);

            if (result != null)
            {
                _textWriter?.Write(Input.AsSpan(_writerIndex, capture.Index - _writerIndex));
                _textWriter?.Write(result);

                _writerIndex = capture.Index + capture.Length;

                ReplacementCount++;
            }

            SpellcheckState?.ProcessReplacement(Input, capture, result);
        }

        protected override void WriteEndMatch(ICapture capture)
        {
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteEndMatches()
        {
            if (ReplacementCount > 0)
                _textWriter?.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));
        }

        protected override void WriteStartReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndReplacement(ICapture capture, string? result)
        {
        }

        public override void Dispose()
        {
            if (ReplacementCount > 0)
                _textWriter?.Dispose();
        }
    }
}
