// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Orang.CommandLine
{
    internal class BaseCommandLineOptions : AbstractCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Output,
            longName: OptionNames.Output,
            HelpText = "Path to a file that should store output.",
            MetaValue = MetaValues.Output)]
        public IEnumerable<string> Output { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Help,
            longName: OptionNames.Help,
            HelpText = "Show command line help.")]
        public bool Help { get; set; }
    }
}
