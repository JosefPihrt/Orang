// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Orang.Text.RegularExpressions;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class MatchCommand : RegexCommand<MatchCommandOptions>
    {
        public MatchCommand(MatchCommandOptions options) : base(options)
        {
        }

        protected override CommandResult ExecuteCore(CancellationToken cancellationToken = default)
        {
            MatchData matchData = MatchData.Create(
                Options.Input,
                Options.Filter.Regex,
                Options.MaxCount,
                cancellationToken);

            var outputWriter = new OutputWriter(Options.HighlightOptions);

            int count = outputWriter.WriteMatches(matchData, Options, cancellationToken);

            if (count > 0)
                WriteLine();

            if (ShouldLog(Verbosity.Detailed)
                || Options.IncludeSummary)
            {
                Verbosity verbosity = (Options.IncludeSummary) ? Verbosity.Minimal : Verbosity.Detailed;

                WriteGroups(matchData.GroupDefinitions, verbosity: verbosity);
                WriteLine(verbosity);

                if (Options.ContentDisplayStyle == ContentDisplayStyle.Value
                    && Options.ModifyOptions.HasAnyFunction)
                {
                    WriteCount("Values", count, Colors.Message_OK, verbosity);
                }
                else
                {
                    WriteCount("Matches", matchData.Count, Colors.Message_OK, verbosity);

                    if (count != matchData.Count)
                    {
                        Write("  ", Colors.Message_OK, verbosity);
                        WriteCount("Captures", count, Colors.Message_OK, verbosity);
                    }
                }

                WriteLine(verbosity);
            }

            return (count > 0) ? CommandResult.Success : CommandResult.NoMatch;
        }
    }
}
