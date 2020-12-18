// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Orang.CommandLine
{
    internal abstract class CommonCopyCommandLineOptions : CommonFindCommandLineOptions
    {
        [Option(
            longName: OptionNames.Compare,
            HelpText = "File properties to be compared.",
            MetaValue = MetaValues.CompareOptions)]
        public IEnumerable<string> Compare { get; set; } = null!;

        public bool TryParse(CommonCopyCommandOptions options)
        {
            var baseOptions = (CommonFindCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CommonCopyCommandOptions)baseOptions;

            return true;
        }
    }
}
