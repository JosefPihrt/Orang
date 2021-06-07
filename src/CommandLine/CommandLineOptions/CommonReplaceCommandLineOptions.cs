// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutGroupAndPartAndNegative)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.ReplaceHighlightOptions)]
    internal abstract class CommonReplaceCommandLineOptions : FileSystemCommandLineOptions
    {
        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask for permission after each file or value.",
            MetaValue = MetaValues.AskMode)]
        public string Ask { get; set; } = null!;

        public abstract IEnumerable<string> Content { get; set; }

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files should be updated but do not actually update any file.")]
        public bool DryRun { get; set; }

        [Option(
            longName: OptionNames.Interactive,
            HelpText = "Enable editing of a replacement.")]
        public bool Interactive { get; set; }

        [Option(
            longName: OptionNames.Pipe,
            HelpText = "Defines how to use redirected/piped input.",
            MetaValue = MetaValues.PipeMode)]
        public string Pipe { get; set; } = null!;

        [Option(
            longName: OptionNames.Input,
            HelpText = "The input string to be searched.",
            MetaValue = MetaValues.Input)]
        public IEnumerable<string> Input { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.MaxCount,
            longName: OptionNames.MaxCount,
            HelpText = "Stop searching after specified number is reached.",
            MetaValue = MetaValues.MaxOptions)]
        public IEnumerable<string> MaxCount { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Name,
            longName: OptionNames.Name,
            HelpText = "Regular expression for file or directory name.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        public bool TryParse(CommonReplaceCommandOptions options)
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

            var baseOptions = (FileSystemCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CommonReplaceCommandOptions)baseOptions;

            Filter? contentFilter = null;
            if (!Content.Any())
            {
                contentFilter = GetDefaultContentFilter();
            }

            if (contentFilter == null
                && !FilterParser.TryParse(
                    Content,
                    OptionNames.Content,
                    OptionValueProviders.PatternOptionsWithoutGroupAndPartAndNegativeProvider,
                    out contentFilter))
            {
                return false;
            }

            if (!TryParseMaxCount(MaxCount, out int maxMatchingFiles, out int maxMatchesInFile))
                return false;

            string? input = null;

            if (Input.Any()
                && !TryParseInput(Input, out input))
            {
                return false;
            }

            if (pipeMode != PipeMode.Paths
                && Console.IsInputRedirected)
            {
                if (input != null)
                {
                    WriteError("Cannot use both redirected/piped input and "
                        + $"option '{OptionNames.GetHelpText(OptionNames.Input)}'.");

                    return false;
                }

                if (contentFilter == null)
                {
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Content)}' is required "
                        + "when redirected/piped input is used as a text to be searched.");

                    return false;
                }

                input = ConsoleHelpers.ReadRedirectedInput();
            }

            ContentDisplayStyle contentDisplayStyle;
            PathDisplayStyle pathDisplayStyle;

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle? contentDisplayStyle2,
                pathDisplayStyle: out PathDisplayStyle? pathDisplayStyle2,
                lineDisplayOptions: out LineDisplayOptions lineDisplayOptions,
                lineContext: out LineContext lineContext,
                displayParts: out DisplayParts displayParts,
                fileProperties: out ImmutableArray<FileProperty> fileProperties,
                indent: out string? indent,
                separator: out string? separator,
                noAlign: out bool noAlign,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider_WithoutUnmatchedLines,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider))
            {
                return false;
            }

            if (contentDisplayStyle2 != null)
            {
                if (options.AskMode == AskMode.Value
                    && contentDisplayStyle2 == ContentDisplayStyle.AllLines)
                {
                    string helpValue = OptionValueProviders.ContentDisplayStyleProvider
                        .GetValue(nameof(ContentDisplayStyle.AllLines))
                        .HelpValue;

                    string helpValue2 = OptionValueProviders.AskModeProvider.GetValue(nameof(AskMode.Value)).HelpValue;

                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Display)}' cannot have value "
                        + $"'{helpValue}' when option '{OptionNames.GetHelpText(OptionNames.Ask)}' has value '{helpValue2}'.");

                    return false;
                }

                contentDisplayStyle = contentDisplayStyle2.Value;
            }
            else if (Input.Any())
            {
                contentDisplayStyle = ContentDisplayStyle.AllLines;
            }
            else
            {
                contentDisplayStyle = ContentDisplayStyle.Line;
            }

            pathDisplayStyle = pathDisplayStyle2 ?? PathDisplayStyle.Full;

            if (pathDisplayStyle == PathDisplayStyle.Relative
                && options.Paths.Length > 1
                && options.SortOptions != null)
            {
                pathDisplayStyle = PathDisplayStyle.Full;
            }

            if (!TryParseHighlightOptions(
                Highlight,
                out HighlightOptions highlightOptions,
                defaultValue: HighlightOptions.Replacement,
                contentDisplayStyle: contentDisplayStyle,
                provider: OptionValueProviders.ReplaceHighlightOptionsProvider))
            {
                return false;
            }

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle,
                pathDisplayStyle: pathDisplayStyle,
                lineOptions: lineDisplayOptions,
                lineContext: lineContext,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator,
                alignColumns: !noAlign);

            options.HighlightOptions = highlightOptions;
            options.ContentFilter = contentFilter;
            options.Input = input;
            options.DryRun = DryRun;
            options.MaxMatchesInFile = maxMatchesInFile;
            options.MaxMatchingFiles = maxMatchingFiles;
            options.MaxTotalMatches = 0;
            options.Interactive = Interactive;

            return true;
        }

        protected virtual Filter? GetDefaultContentFilter()
        {
            return null;
        }

        protected override bool TryParsePaths(out ImmutableArray<PathInfo> paths)
        {
            if (Path.Any()
                && Input.Any())
            {
                WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Input)}' and "
                    + $"argument '{ArgumentMetaNames.Path}' cannot be set both at the same time.");

                return false;
            }

            return base.TryParsePaths(out paths);
        }
    }
}
