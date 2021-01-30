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
        public abstract ContentDisplayStyle DefaultContentDisplayStyle { get; }

        [Option(
            shortName: OptionShortNames.Content,
            longName: OptionNames.Content,
            HelpText = "Regular expression for files' content.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

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

        public bool TryParse(CommonFindCommandOptions options)
        {
            var baseOptions = (FileSystemCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CommonFindCommandOptions)baseOptions;

            if (!FilterParser.TryParse(
                Content,
                OptionNames.Content,
                OptionValueProviders.PatternOptionsWithoutPartProvider,
                out Filter? contentFilter,
                allowNull: true))
            {
                return false;
            }

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
                noAlign: out bool noAlign,
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
                && (contentDisplayStyle == ContentDisplayStyle.AllLines
                    || contentDisplayStyle == ContentDisplayStyle.UnmatchedLines))
            {
                string helpValue = OptionValueProviders.ContentDisplayStyleProvider
                    .GetValue(contentDisplayStyle.Value.ToString())
                    .HelpValue;

                string helpValue2 = OptionValueProviders.AskModeProvider.GetValue(nameof(AskMode.Value)).HelpValue;

                WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Display)}' cannot have value '{helpValue}' "
                    + $"when option '{OptionNames.GetHelpText(OptionNames.Ask)}' has value '{helpValue2}'.");

                return false;
            }

            if (!TryParseMaxCount(MaxCount, out int maxMatchingFiles, out int maxMatchesInFile))
                return false;

            if (!TryParseHighlightOptions(
                Highlight,
                out HighlightOptions highlightOptions,
                defaultValue: HighlightOptions.Default,
                contentDisplayStyle: contentDisplayStyle,
                provider: OptionValueProviders.FindHighlightOptionsProvider))
            {
                return false;
            }

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle ?? DefaultContentDisplayStyle,
                pathDisplayStyle: pathDisplayStyle ?? PathDisplayStyle.Full,
                lineOptions: lineDisplayOptions,
                lineContext: lineContext,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator,
                alignColumns: !noAlign);

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
