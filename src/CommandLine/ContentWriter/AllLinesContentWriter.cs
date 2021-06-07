// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class AllLinesContentWriter : ContentWriter
    {
        private int _lastPos;
        private ValueWriter? _valueWriter;

        public AllLinesContentWriter(
            string input,
            ContentWriterOptions options,
            IResultStorage? storage = null) : this(input, ContentTextWriter.Default, options, storage)
        {
        }

        public AllLinesContentWriter(
            string input,
            ContentTextWriter? writer,
            ContentWriterOptions options,
            IResultStorage? storage = null) : base(input, writer, options)
        {
            ResultStorage = storage;
        }

        public IResultStorage? ResultStorage { get; }

        protected override ValueWriter ValueWriter
        {
            get
            {
                if (_valueWriter == null)
                {
                    if (Options.IncludeLineNumber)
                    {
                        _valueWriter = new LineNumberValueWriter(Writer, Options.Indent);
                    }
                    else
                    {
                        _valueWriter = new ValueWriter(Writer, Options.Indent);
                    }
                }

                return _valueWriter;
            }
        }

        protected override void WriteStartMatches()
        {
            _lastPos = 0;

            Write(Options.Indent);

            if (Options.IncludeLineNumber)
                WriteLineNumber(1);

            ResultStorage?.Add(Input);
        }

        protected override void WriteStartMatch(ICapture capture)
        {
            int index = capture.Index;

            ValueWriter.Write(Input, _lastPos, index - _lastPos, symbols: null);
        }

        protected override void WriteEndMatch(ICapture capture)
        {
            _lastPos = capture.Index + capture.Length;
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
            ValueWriter.Write(Input, _lastPos, Input.Length - _lastPos, symbols: null);
            WriteLine();
        }

        public override void Dispose()
        {
        }
    }
}
