// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal class TextWriterContentWriter : ContentWriter
    {
        private readonly TextWriter _textWriter;
        private int _writerIndex;

        public TextWriterContentWriter(
            string input,
            ReplaceOptions replaceOptions,
            TextWriter textWriter,
            ContentWriterOptions options) : base(input, options)
        {
            ReplaceOptions = replaceOptions;
            _textWriter = textWriter;
        }

        public ReplaceOptions ReplaceOptions { get; }

        protected override ValueWriter ValueWriter => throw new NotSupportedException();

        protected override void WriteStartMatches()
        {
            _writerIndex = 0;
        }

        protected override void WriteStartMatch(CaptureInfo capture)
        {
        }

        protected override void WriteMatch(CaptureInfo capture)
        {
            _textWriter.Write(Input.AsSpan(_writerIndex, capture.Index - _writerIndex));

            string result = ReplaceOptions.Replace((Match)capture.Capture!);

            _textWriter.Write(result);

            _writerIndex = capture.Index + capture.Length;
        }

        protected override void WriteEndMatch(CaptureInfo capture)
        {
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteEndMatches()
        {
            _textWriter.Write(Input.AsSpan(_writerIndex, Input.Length - _writerIndex));
        }

        protected override void WriteStartReplacement(Match match, string result)
        {
        }

        protected override void WriteEndReplacement(Match match, string result)
        {
        }

        public override void Dispose()
        {
            _textWriter.Dispose();
        }
    }
}
