// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    internal abstract class CommonCopyCommandLineOptions : CommonFindCommandLineOptions
    {
        public override ContentDisplayStyle DefaultContentDisplayStyle => ContentDisplayStyle.Omit;

        [Option(
            longName: OptionNames.Compare,
            HelpText = "File properties to be compared.",
            MetaValue = MetaValues.CompareOptions)]
        public IEnumerable<string> Compare { get; set; } = null!;
#if DEBUG
        [Option(
            longName: OptionNames.AllowedTimeDiff,
            HelpText = "Syntax is d|[d.]hh:mm[:ss[.ff]].",
            MetaValue = MetaValues.TimeSpan)]
        public string AllowedTimeDiff { get; set; } = null!;

        [Option(
            longName: OptionNames.IgnoredAttributes,
            HelpText = "File attributes that should be ignored during comparison.",
            MetaValue = MetaValues.Attributes)]
        public IEnumerable<string> IgnoredAttributes { get; set; } = null!;
#endif
        public bool TryParse(CommonCopyCommandOptions options)
        {
            var baseOptions = (CommonFindCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CommonCopyCommandOptions)baseOptions;

            if (!FilterParser.TryParse(
                Name,
                OptionNames.Name,
                OptionValueProviders.PatternOptionsProvider,
                out Filter? nameFilter,
                out FileNamePart namePart,
                allowNull: true))
            {
                return false;
            }
#if DEBUG
            if (!TryParseAsEnumFlags(
                IgnoredAttributes,
                OptionNames.IgnoredAttributes,
                out FileSystemAttributes noCompareAttributes,
                provider: OptionValueProviders.FileSystemAttributesToSkipProvider))
            {
                return false;
            }

            TimeSpan allowedTimeDiff = TimeSpan.Zero;

            if (AllowedTimeDiff != null
                && !TimeSpan.TryParse(AllowedTimeDiff, CultureInfo.InvariantCulture, out allowedTimeDiff))
            {
                Logger.WriteError($"Option '{OptionNames.GetHelpText(OptionNames.AllowedTimeDiff)}' "
                    + $"has invalid value '{AllowedTimeDiff}'.");

                return false;
            }

            options.AllowedTimeDiff = allowedTimeDiff;
            options.NoCompareAttributes = GetFileAttributes(noCompareAttributes);
#endif
            options.NameFilter = nameFilter;
            options.NamePart = namePart;

            return true;
        }
    }
}
