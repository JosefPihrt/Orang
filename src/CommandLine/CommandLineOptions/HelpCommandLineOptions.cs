// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Orang.CommandLine
{
    [Verb("help", HelpText = "Displays help.")]
    [OptionValueProvider(nameof(Filter), OptionValueProviderNames.PatternOptions_List)]
    [CommandGroup("Other", 3)]
    internal sealed class HelpCommandLineOptions : AbstractCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "Command name.",
            MetaName = ArgumentMetaNames.Command)]
        public string Command { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Filter,
            longName: OptionNames.Filter,
            HelpText = "Regular expression to filter results.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Filter { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Manual,
            longName: OptionNames.Manual,
            HelpText = "Display full manual.")]
        public bool Manual { get; set; }

        [Option(
            shortName: OptionShortNames.Online,
            longName: OptionNames.Online,
            HelpText = "Launch online help in a default browser.")]
        public bool Online { get; set; }

        public bool TryParse(HelpCommandOptions options)
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

            options.Command = Command;
            options.Filter = filter;
            options.Manual = Manual;
            options.Online = Online;

            return true;
        }
    }
}
