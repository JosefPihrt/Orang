// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [Verb("find", HelpText = "Searches the file system for files and directories and optionally searches files' content.")]
    internal sealed class FindCommandLineOptions : CommonFindCommandLineOptions
    {
        [Option(
            longName: OptionNames.Pipe,
            HelpText = "Defines how to use redirected/piped input.",
            MetaValue = MetaValues.PipeMode)]
        public string Pipe { get; set; } = null!;

        [Option(
            longName: OptionNames.Modify,
            HelpText = "Functions to modify results.",
            MetaValue = MetaValues.ModifyOptions)]
        public IEnumerable<string> Modify { get; set; } = null!;

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
                    WriteError($"Redirected/piped input is required when option '{OptionNames.GetHelpText(OptionNames.Pipe)}' is specified.");
                    return false;
                }

                PipeMode = pipeMode;
            }

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
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Content)}' is required " +
                        "when redirected/piped input is used as a text to be searched.");

                    return false;
                }

                input = ConsoleHelpers.ReadRedirectedInput();
            }

            if (!TryParseModifyOptions(Modify, OptionNames.Modify, out ModifyOptions? modifyOptions, out bool aggregateOnly))
                return false;

            OutputDisplayFormat format = options.Format;
            ContentDisplayStyle contentDisplayStyle = format.ContentDisplayStyle;
            PathDisplayStyle pathDisplayStyle = format.PathDisplayStyle;

            if (modifyOptions.HasAnyFunction
                && contentDisplayStyle == ContentDisplayStyle.ValueDetail)
            {
                contentDisplayStyle = ContentDisplayStyle.Value;
            }

            if (aggregateOnly)
            {
                if (ConsoleOut.Verbosity > Orang.Verbosity.Minimal)
                    ConsoleOut.Verbosity = Orang.Verbosity.Minimal;

                pathDisplayStyle = PathDisplayStyle.Omit;
            }

            options.Input = input;
            options.ModifyOptions = modifyOptions;

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle,
                pathDisplayStyle: pathDisplayStyle,
                lineOptions: format.LineOptions,
                lineContext: format.LineContext,
                displayParts: format.DisplayParts,
                fileProperties: format.FileProperties,
                indent: format.Indent,
                separator: format.Separator,
                alignColumns: format.AlignColumns,
                includeBaseDirectory: format.IncludeBaseDirectory);

            return true;
        }
    }
}
