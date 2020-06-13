// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutPart)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.FindHighlightOptions)]
    internal abstract class CommonFindCommandLineOptions : FileSystemCommandLineOptions
    {
        [Option(longName: OptionNames.Ask,
            HelpText = "Ask for permission after each file or value.",
            MetaValue = MetaValues.AskMode)]
        public string Ask { get; set; } = null!;

        [Option(shortName: OptionShortNames.Content, longName: OptionNames.Content,
            HelpText = "Regular expression for files' content. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(shortName: OptionShortNames.MaxCount, longName: OptionNames.MaxCount,
            HelpText = "Stop searching after specified number is reached.",
            MetaValue = MetaValues.MaxOptions)]
        public IEnumerable<string> MaxCount { get; set; } = null!;

        [Option(shortName: OptionShortNames.Name, longName: OptionNames.Name,
            HelpText = "Regular expression for file or directory name. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        public bool TryParse(CommonFindCommandOptions options)
        {
            var baseOptions = (FileSystemCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CommonFindCommandOptions)baseOptions;

            if (!TryParseProperties(Ask, Name, options))
                return false;

            if (!TryParseAsEnumFlags(Highlight, OptionNames.Highlight, out HighlightOptions highlightOptions, defaultValue: HighlightOptions.Default, provider: OptionValueProviders.FindHighlightOptionsProvider))
                return false;

            if (!FilterParser.TryParse(Content, OptionNames.Content, OptionValueProviders.PatternOptionsWithoutPartProvider, out Filter? contentFilter, allowNull: true))
                return false;

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle? contentDisplayStyle,
                pathDisplayStyle: out PathDisplayStyle? pathDisplayStyle,
                lineDisplayOptions: out LineDisplayOptions lineDisplayOptions,
                lineContext: out LineContext lineContext,
                displayParts: out DisplayParts displayParts,
                fileProperties: out ImmutableArray<FileProperty> fileProperties,
                indent: out string? indent,
                separator: out string? separator,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider))
            {
                return false;
            }

            if (pathDisplayStyle == PathDisplayStyle.Relative
                && options.Paths.Length > 1
                && options.SortOptions != null)
            {
                pathDisplayStyle = PathDisplayStyle.Full;
            }

            if (options.AskMode == AskMode.Value
                && (contentDisplayStyle == ContentDisplayStyle.AllLines || contentDisplayStyle == ContentDisplayStyle.UnmatchedLines))
            {
                WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Display)}' cannot have value '{OptionValueProviders.ContentDisplayStyleProvider.GetValue(contentDisplayStyle.Value.ToString()).HelpValue}' when option '{OptionNames.GetHelpText(OptionNames.Ask)}' has value '{OptionValueProviders.AskModeProvider.GetValue(nameof(AskMode.Value)).HelpValue}'.");
                return false;
            }

            if (!TryParseMaxCount(MaxCount, out int maxMatchingFiles, out int maxMatchesInFile))
                return false;

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle ?? ContentDisplayStyle.Line,
                pathDisplayStyle: pathDisplayStyle ?? PathDisplayStyle.Full,
                lineOptions: lineDisplayOptions,
                lineContext: lineContext,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator);

            options.HighlightOptions = highlightOptions;
            options.SearchTarget = GetSearchTarget();
            options.ContentFilter = contentFilter;
            options.MaxMatchesInFile = maxMatchesInFile;
            options.MaxMatchingFiles = maxMatchingFiles;
            options.MaxTotalMatches = 0;

            return true;
        }
    }
}
