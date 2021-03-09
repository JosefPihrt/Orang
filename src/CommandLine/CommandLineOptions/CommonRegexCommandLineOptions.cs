// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Orang.CommandLine
{
    internal abstract class CommonRegexCommandLineOptions : BaseCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Display,
            longName: OptionNames.Display,
            HelpText = "Display of the results.",
            MetaValue = MetaValues.DisplayOptions)]
        public IEnumerable<string> Display { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Highlight,
            longName: OptionNames.Highlight,
            HelpText = "Parts of the output to highlight.",
            MetaValue = MetaValues.Highlight)]
        public IEnumerable<string> Highlight { get; set; } = null!;

        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        [SuppressMessage("Redundancy", "RCS1163:Unused parameter.")]
        public bool TryParse(CommonRegexCommandOptions options)
        {
            return true;
        }
    }
}
