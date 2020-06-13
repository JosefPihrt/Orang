// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal sealed class ReplaceCommandLineOptions : FileSystemCommandLineOptions
    {
        [Option(longName: OptionNames.Ask,
            HelpText = "Ask for permission after each file or value.",
            MetaValue = MetaValues.AskMode)]
        public string Ask { get; set; } = null!;

        [Option(shortName: OptionShortNames.Content, longName: OptionNames.Content,
            Required = true,
            HelpText = "Regular expression for files' content. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(shortName: OptionShortNames.DryRun, longName: OptionNames.DryRun,
            HelpText = "Display which files should be updated but do not actually update any file.")]
        public bool DryRun { get; set; }

        [Option(longName: OptionNames.Evaluator,
            HelpText = "Path to the evaluator method to compute replacements. The format is \"LibraryPath,FullTypeName.MethodName\".",
            MetaValue = MetaValues.Evaluator)]
        public string Evaluator { get; set; } = null!;

        [Option(longName: OptionNames.Input,
            HelpText = "The input string to be searched. Syntax is <INPUT> [<INPUT_OPTIONS>].",
            MetaValue = MetaValues.Input)]
        public IEnumerable<string> Input { get; set; } = null!;

        [Option(shortName: OptionShortNames.MaxCount, longName: OptionNames.MaxCount,
            HelpText = "Stop searching after specified number is reached.",
            MetaValue = MetaValues.MaxOptions)]
        public IEnumerable<string> MaxCount { get; set; } = null!;

        [Option(longName: OptionNames.Modify,
            HelpText = "Functions to modify result.",
            MetaValue = MetaValues.ReplaceModify)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(shortName: OptionShortNames.Name, longName: OptionNames.Name,
            HelpText = "Regular expression for file or directory name. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        [Option(shortName: OptionShortNames.Replacement, longName: OptionNames.Replacement,
            HelpText = "Replacement pattern. Syntax is <REPLACEMENT> [<REPLACEMENT_OPTIONS>].",
            MetaValue = MetaValues.Replacement)]
        public IEnumerable<string> Replacement { get; set; } = null!;

        public bool TryParse(ReplaceCommandOptions options)
        {
            var baseOptions = (FileSystemCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (ReplaceCommandOptions)baseOptions;

            if (!TryParseProperties(Ask, Name, options))
                return false;

            if (!TryParseAsEnumFlags(Highlight, OptionNames.Highlight, out HighlightOptions highlightOptions, defaultValue: HighlightOptions.Replacement, provider: OptionValueProviders.ReplaceHighlightOptionsProvider))
                return false;

            if (!FilterParser.TryParse(Content, OptionNames.Content, OptionValueProviders.PatternOptionsWithoutGroupAndPartAndNegativeProvider, out Filter? contentFilter))
                return false;

            if (!TryParseReplacement(Replacement, out string? replacement))
                return false;

            if (!DelegateFactory.TryCreateMatchEvaluator(Evaluator, out MatchEvaluator? matchEvaluator))
                return false;

            if (replacement != null && matchEvaluator != null)
            {
                WriteError($"Options '{OptionNames.GetHelpText(OptionNames.Replacement)}' and '{OptionNames.GetHelpText(OptionNames.Evaluator)}' cannot be set both at the same time.");
                return false;
            }

            if (!TryParseReplaceOptions(Modify, OptionNames.Modify, replacement, matchEvaluator, out ReplaceOptions? replaceOptions))
                return false;

            if (!TryParseMaxCount(MaxCount, out int maxMatchingFiles, out int maxMatchesInFile))
                return false;

            string? input = null;

            if (Input.Any()
                && !TryParseInput(Input, out input))
            {
                return false;
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
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Display)}' cannot have value '{OptionValueProviders.ContentDisplayStyleProvider.GetValue(nameof(ContentDisplayStyle.AllLines)).HelpValue}' when option '{OptionNames.GetHelpText(OptionNames.Ask)}' has value '{OptionValueProviders.AskModeProvider.GetValue(nameof(AskMode.Value)).HelpValue}'.");
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
                separator: separator);

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
                WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Input)}' and argument '{ArgumentMetaNames.Path}' cannot be set both at the same time.");
                return false;
            }

            return base.TryParsePaths(out paths);
        }
    }
}
