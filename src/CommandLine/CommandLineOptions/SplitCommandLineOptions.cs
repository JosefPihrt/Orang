// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb(
        "split",
        HelpText = "Splits the input string into an list of substrings at the positions defined by a regular expression.")]
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutGroupAndPartAndNegative)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.SplitHighlightOptions)]
    [CommandGroup("Regex", 2)]
    internal sealed class SplitCommandLineOptions : RegexCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Content,
            longName: OptionNames.Content,
            Required = true,
            HelpText = "Regular expression for the input string.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.MaxCount,
            longName: OptionNames.MaxCount,
            Default = 0,
            HelpText = "Maximum number of times the split can occur.",
            MetaValue = MetaValues.Num)]
        public int MaxCount { get; set; }

        [Option(
            longName: OptionNames.NoGroups,
            HelpText = "Do not include groups in the results.")]
        public bool NoGroups { get; set; }

        public bool TryParse(SplitCommandOptions options)
        {
            var baseOptions = (RegexCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (SplitCommandOptions)baseOptions;

            if (!FilterParser.TryParse(
                Content,
                OptionNames.Content,
                OptionValueProviders.PatternOptionsWithoutGroupAndPartAndNegativeProvider,
                out Filter? filter))
            {
                return false;
            }

            if (!TryParseHighlightOptions(
                Highlight,
                out HighlightOptions highlightOptions,
                defaultValue: HighlightOptions.Split,
                provider: OptionValueProviders.SplitHighlightOptionsProvider))
            {
                return false;
            }

            options.Filter = filter!;
            options.HighlightOptions = highlightOptions;
            options.OmitGroups = NoGroups;
            options.MaxCount = MaxCount;

            return true;
        }
    }
}
