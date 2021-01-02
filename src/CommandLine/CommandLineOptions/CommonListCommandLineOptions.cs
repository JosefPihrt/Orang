// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Orang.CommandLine
{
    [OptionValueProvider(nameof(Filter), OptionValueProviderNames.PatternOptions_List)]
    internal abstract class CommonListCommandLineOptions : BaseCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Filter,
            longName: OptionNames.Filter,
            HelpText = "Regular expression to filter results (case-insensitive by default).",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Filter { get; set; } = null!;

        public bool TryParse(CommonListCommandOptions options)
        {
            if (!FilterParser.TryParse(
                Filter,
                OptionNames.Filter,
                OptionValueProviders.PatternOptions_List_Provider,
                out Filter? filter,
                includedPatternOptions: PatternOptions.IgnoreCase,
                allowNull: true))
            {
                return false;
            }

            options.Filter = filter;

            return true;
        }
    }
}
