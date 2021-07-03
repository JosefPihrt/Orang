// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [Verb("find", HelpText = "Searches the file system for files and directories and optionally searches files' content.")]
    [CommandGroup("Main", 0)]
    internal sealed class FindCommandLineOptions : CommonFindCommandLineOptions
    {
        public override ContentDisplayStyle DefaultContentDisplayStyle => ContentDisplayStyle.Line;

        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask for permission after each file or value.",
            MetaValue = MetaValues.AskMode)]
        public string Ask { get; set; } = null!;

        [Option(
            longName: OptionNames.Pipe,
            HelpText = "Defines how to use redirected/piped input.",
            MetaValue = MetaValues.PipeMode)]
        public string Pipe { get; set; } = null!;
#if DEBUG // --modifier
        [Option(
            longName: OptionNames.Modifier,
            HelpText = OptionHelpText.Modifier,
            MetaValue = MetaValues.Modifier)]
        public IEnumerable<string> Modifier { get; set; } = null!;
#endif
        [Option(
            longName: OptionNames.Modify,
            HelpText = "Functions to modify results.",
            MetaValue = MetaValues.ModifyOptions)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(
            longName: "split",
            HelpText = "Execute regex in a split mode.")]
        public bool Split { get; set; }

        public bool TryParse(FindCommandOptions options)
        {
            if (!TryParseAsEnum(
                Pipe,
                OptionNames.Pipe,
                out PipeMode pipeMode,
                PipeMode.None,
                OptionValueProviders.PipeMode))
            {
                return false;
            }

            if (pipeMode == PipeMode.None)
            {
                if (Console.IsInputRedirected)
                    PipeMode = PipeMode.Text;
            }
            else
            {
                if (!Console.IsInputRedirected)
                {
                    WriteError("Redirected/piped input is required "
                        + $"when option '{OptionNames.GetHelpText(OptionNames.Pipe)}' is specified.");

                    return false;
                }

                PipeMode = pipeMode;
            }

            if (!TryParseProperties(Ask, Name, options))
                return false;

            var baseOptions = (CommonFindCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (FindCommandOptions)baseOptions;

            string? input = null;

            if (pipeMode != PipeMode.Paths
                && Console.IsInputRedirected)
            {
                if (options.ContentFilter == null)
                {
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Content)}' is required "
                        + "when redirected/piped input is used as a text to be searched.");

                    return false;
                }

                input = ConsoleHelpers.ReadRedirectedInput();
            }

            EnumerableModifier<string>? modifier = null;
#if DEBUG // --modifier
            if (Modifier.Any()
                && !TryParseModifier(Modifier, OptionNames.Modifier, out modifier))
            {
                return false;
            }
#endif
            if (!TryParseModifyOptions(
                Modify,
                OptionNames.Modify,
                modifier,
                out ModifyOptions? modifyOptions,
                out bool aggregateOnly))
            {
                return false;
            }

            OutputDisplayFormat format = options.Format;
            ContentDisplayStyle contentDisplayStyle = format.ContentDisplayStyle;
            PathDisplayStyle pathDisplayStyle = format.PathDisplayStyle;

            if (modifyOptions.HasAnyFunction
                && contentDisplayStyle == ContentDisplayStyle.ValueDetail)
            {
                contentDisplayStyle = ContentDisplayStyle.Value;
            }

            options.Input = input;
            options.ModifyOptions = modifyOptions;
            options.AggregateOnly = aggregateOnly;
            options.Split = Split;

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle,
                pathDisplayStyle: pathDisplayStyle,
                lineOptions: format.LineOptions,
                lineContext: format.LineContext,
                displayParts: format.DisplayParts,
                fileProperties: format.FileProperties,
                indent: format.Indent,
                separator: format.Separator,
                alignColumns: format.AlignColumns);

            return true;
        }
    }
}
