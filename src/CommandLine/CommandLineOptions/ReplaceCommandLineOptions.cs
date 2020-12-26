// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [Verb("replace", HelpText = "Searches the file system for files and replaces its content.")]
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutGroupAndPartAndNegative)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.ReplaceHighlightOptions)]
    [CommandGroup("Main", 0)]
    internal sealed class ReplaceCommandLineOptions : FileSystemCommandLineOptions
    {
        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask for permission after each file or value.",
            MetaValue = MetaValues.AskMode)]
        public string Ask { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Content,
            longName: OptionNames.Content,
            Required = true,
            HelpText = "Regular expression for files' content.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files should be updated but do not actually update any file.")]
        public bool DryRun { get; set; }

        [Hidden]
        [Option(
            longName: OptionNames.Evaluator,
            HelpText = "[deprecated] Use option -r, --replacement instead.",
            MetaValue = MetaValues.Evaluator)]
        public string Evaluator { get; set; } = null!;

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
            longName: OptionNames.Modify,
            HelpText = "Functions to modify result.",
            MetaValue = MetaValues.ReplaceModify)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Name,
            longName: OptionNames.Name,
            HelpText = "Regular expression for file or directory name.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Replacement,
            longName: OptionNames.Replacement,
            HelpText = "Replacement pattern.",
            MetaValue = MetaValues.Replacement)]
        public IEnumerable<string> Replacement { get; set; } = null!;

        public bool TryParse(ReplaceCommandOptions options)
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

            var baseOptions = (FileSystemCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (ReplaceCommandOptions)baseOptions;

            if (!TryParseProperties(Ask, Name, options))
                return false;

            if (!TryParseAsEnumFlags(
                Highlight,
                OptionNames.Highlight,
                out HighlightOptions highlightOptions,
                defaultValue: HighlightOptions.Replacement,
                provider: OptionValueProviders.ReplaceHighlightOptionsProvider))
            {
                return false;
            }

            if (!FilterParser.TryParse(
                Content,
                OptionNames.Content,
                OptionValueProviders.PatternOptionsWithoutGroupAndPartAndNegativeProvider,
                out Filter? contentFilter))
            {
                return false;
            }

            if (!TryParseReplacement(Replacement, out string? replacement, out MatchEvaluator? matchEvaluator))
                return false;

            if (matchEvaluator == null
                && Evaluator != null)
            {
                WriteWarning($"Option '{OptionNames.GetHelpText(OptionNames.Evaluator)}' is obsolete. " +
                    $"Use option '{OptionNames.GetHelpText(OptionNames.Replacement)}' instead.");

                if (!DelegateFactory.TryCreateFromAssembly(Evaluator, out matchEvaluator))
                    return false;
            }

            if (!TryParseReplaceOptions(
                Modify,
                OptionNames.Modify,
                replacement,
                matchEvaluator,
                out ReplaceOptions? replaceOptions))
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
                    WriteError("Cannot use both redirected/piped input and " +
                        $"option '{OptionNames.GetHelpText(OptionNames.Input)}'.");

                    return false;
                }

                if (options.ContentFilter == null)
                {
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Content)}' is required " +
                        "when redirected/piped input is used as a text to be searched.");

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

                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Display)}' cannot have value " +
                        $"'{helpValue}' when option '{OptionNames.GetHelpText(OptionNames.Ask)}' has value '{helpValue2}'.");

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
            options.ReplaceOptions = replaceOptions;
            options.Input = input;
            options.DryRun = DryRun;
            options.MaxMatchesInFile = maxMatchesInFile;
            options.MaxMatchingFiles = maxMatchingFiles;
            options.MaxTotalMatches = 0;

            return true;
        }

        protected override bool TryParsePaths(out ImmutableArray<PathInfo> paths)
        {
            if (Path.Any()
                && Input.Any())
            {
                WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Input)}' and " +
                    $"argument '{ArgumentMetaNames.Path}' cannot be set both at the same time.");

                return false;
            }

            return base.TryParsePaths(out paths);
        }
    }
}
