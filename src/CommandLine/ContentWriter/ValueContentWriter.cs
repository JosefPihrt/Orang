// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class ValueContentWriter : ContentWriter
    {
        private ValueWriter? _valueWriter;

        public ValueContentWriter(
            string input,
            ContentWriterOptions options,
            IResultStorage? storage = null,
            MatchOutputInfo? outputInfo = null) : this(input, ContentTextWriter.Default, options, storage, outputInfo)
        {
        }

        public ValueContentWriter(
            string input,
            ContentTextWriter? writer,
            ContentWriterOptions options,
            IResultStorage? storage = null,
            MatchOutputInfo? outputInfo = null) : base(input, writer, options)
        {
            ResultStorage = storage;
            OutputInfo = outputInfo;
        }

        public IResultStorage? ResultStorage { get; }

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

        protected override void WriteStartMatches()
        {
        }

        protected override void WriteStartMatch(ICapture capture)
        {
            ResultStorage?.Add(capture.Value);

            if (MatchCount == 0
                || Options.Separator?.EndsWith('\n') != false)
            {
                Write(Options.Indent);
            }

            if (OutputInfo != null)
                Write(OutputInfo.GetText(capture, MatchCount + 1, groupName: Options.GroupName));
        }

        protected override void WriteEndMatch(ICapture capture)
        {
        }

        protected override void WriteMatchSeparator()
        {
            if (Options.Separator != null)
            {
                Write(Options.Separator);
            }
            else
            {
                WriteLine();
            }
        }

        protected override void WriteStartReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndReplacement(ICapture capture, string? result)
        {
        }

        protected override void WriteEndMatches()
        {
            WriteLine();
        }

        public override void Dispose()
        {
        }
    }
}
