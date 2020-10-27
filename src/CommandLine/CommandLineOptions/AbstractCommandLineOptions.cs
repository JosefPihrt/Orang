// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Orang.CommandLine
{
    internal abstract class AbstractCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Verbosity,
            longName: OptionNames.Verbosity,
            HelpText = "The amount of information to display in the log.",
            MetaValue = MetaValues.Verbosity)]
        public string Verbosity { get; set; } = null!;
    }
}
