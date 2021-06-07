// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class AskValueContentWriter : ValueContentWriter
    {
        public AskValueContentWriter(
            string input,
            ContentWriterOptions options,
            IResultStorage? storage = null,
            MatchOutputInfo? outputInfo = null) : base(input, options, storage, outputInfo)
        {
            Ask = true;
        }

        public bool Ask { get; set; }

        protected override void WriteStartMatch(ICapture capture)
        {
            ResultStorage?.Add(capture.Value);

            Write(Options.Indent);

            if (OutputInfo != null)
                Write(OutputInfo.GetText(capture, MatchCount + 1, groupName: Options.GroupName));
        }

        protected override void WriteEndMatch(ICapture capture)
        {
            WriteLine();

            if (Ask
                && ConsoleHelpers.AskToContinue(Options.Indent) == DialogResult.YesToAll)
            {
                Ask = false;
            }
        }

        protected override void WriteMatchSeparator()
        {
        }

        protected override void WriteEndMatches()
        {
        }
    }
}
