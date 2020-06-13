// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CommandLine;
using Orang.Syntax;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("list-patterns", HelpText = "Lists regular expression patterns.")]
    internal sealed class ListPatternsCommandLineOptions : CommonListCommandLineOptions
    {
        [Value(index: 0,
            Required = false,
            HelpText = "Character or a decimal number that represents the character. For a number literal use escape like \\1.",
            MetaName = ArgumentMetaNames.Char)]
        public string Value { get; set; } = null!;

        [Option(longName: OptionNames.CharGroup,
            HelpText = "Treat character as if it is in the character group.")]
        public bool CharGroup { get; set; }

        [Option(shortName: OptionShortNames.Options, longName: OptionNames.Options,
            HelpText = "Regex options that should be used. Relevant values are [e]cma-[s]cript or [i]gnore-case.",
            MetaValue = MetaValues.RegexOptions)]
        public IEnumerable<string> Options { get; set; } = null!;

        [Option(shortName: OptionShortNames.Section, longName: OptionNames.Section,
            HelpText = "Syntax sections to filter.",
            MetaValue = MetaValues.SyntaxSections)]
        public IEnumerable<string> Section { get; set; } = null!;

        public bool TryParse(ListPatternsCommandOptions options)
        {
            var baseOptions = (CommonListCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (ListPatternsCommandOptions)baseOptions;

            char? value = null;

            if (Value != null)
            {
                if (!TryParseChar(Value, out char ch))
                    return false;

                value = ch;
            }

            if (!TryParseAsEnumFlags(Options, OptionNames.Options, out RegexOptions regexOptions, provider: OptionValueProviders.RegexOptionsProvider))
                return false;

            if (!TryParseAsEnumValues(Section, OptionNames.Section, out ImmutableArray<SyntaxSection> sections, provider: OptionValueProviders.SyntaxSectionProvider))
                return false;

            options.Value = value;
            options.RegexOptions = regexOptions;
            options.InCharGroup = CharGroup;
            options.Sections = sections;

            return true;
        }
    }
}
