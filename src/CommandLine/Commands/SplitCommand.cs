// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Orang.Text.RegularExpressions;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class SplitCommand : RegexCommand<SplitCommandOptions>
    {
        public SplitCommand(SplitCommandOptions options) : base(options)
        {
        }

        protected override CommandResult ExecuteCore(CancellationToken cancellationToken = default)
        {
            SplitData splitData = SplitData.Create(
                Options.Filter.Regex,
                Options.Input,
                Options.MaxCount,
                omitGroups: Options.OmitGroups,
                cancellationToken);

            var outputWriter = new OutputWriter(Options.HighlightOptions);

            int count = outputWriter.WriteSplits(splitData, Options, cancellationToken);

            WriteLine();

            if (ShouldLog(Verbosity.Detailed)
                || Options.IncludeSummary)
            {
                Verbosity verbosity = (Options.IncludeSummary) ? Verbosity.Minimal : Verbosity.Detailed;

                WriteGroups(splitData.GroupDefinitions, verbosity: verbosity);
                WriteLine(verbosity);

                WriteCount(
                    (Options.ContentDisplayStyle == ContentDisplayStyle.Value && Options.ModifyOptions.HasAnyFunction)
                        ? "Values"
                        : "Splits",
                    count,
                    Colors.Message_OK,
                    verbosity);

                WriteLine(verbosity);
            }

            return (count > 0) ? CommandResult.Success : CommandResult.NoMatch;
        }
    }
}
