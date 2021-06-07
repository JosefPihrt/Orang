// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("replace", HelpText = "Searches the file system for files and replaces its content.")]
    [CommandGroup("Main", 0)]
    internal sealed class ReplaceCommandLineOptions : CommonReplaceCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Content,
            longName: OptionNames.Content,
            Required = true,
            HelpText = "Regular expression for files' content.",
            MetaValue = MetaValues.Regex)]
        public override IEnumerable<string> Content { get; set; } = null!;

        [Hidden]
        [Option(
            longName: OptionNames.Evaluator,
            HelpText = "[deprecated] Use option -r, --replacement instead.",
            MetaValue = MetaValues.Evaluator)]
        public string Evaluator { get; set; } = null!;
#if DEBUG // --find
        [Option(
            longName: "find",
            HelpText = "Equivalent to --replacement \"\" --highlight match --dry-run.")]
        public bool Find { get; set; }
#endif
        [Option(
            longName: OptionNames.Modify,
            HelpText = "Functions to modify result.",
            MetaValue = MetaValues.ReplaceModify)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Replacement,
            longName: OptionNames.Replacement,
            HelpText = "Replacement pattern.",
            MetaValue = MetaValues.Replacement)]
        public IEnumerable<string> Replacement { get; set; } = null!;

        public bool TryParse(ReplaceCommandOptions options)
        {
            var baseOptions = (CommonReplaceCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (ReplaceCommandOptions)baseOptions;

            if (!TryParseReplacement(Replacement, out string? replacement, out MatchEvaluator? matchEvaluator))
                return false;

            if (matchEvaluator == null
                && Evaluator != null)
            {
                LogHelpers.WriteObsoleteWarning(
                    $"Option '{OptionNames.GetHelpText(OptionNames.Evaluator)}' is obsolete. "
                        + $"Use option '{OptionNames.GetHelpText(OptionNames.Replacement)}' instead.");

                if (!DelegateFactory.TryCreateFromAssembly(Evaluator, typeof(string), typeof(Match), out matchEvaluator))
                    return false;
            }

            if (!TryParseReplaceOptions(
                Modify,
                OptionNames.Modify,
                replacement,
                matchEvaluator,
                out ReplaceOptions? replaceOptions))
            {
                return false;
            }

            options.Replacer = replaceOptions;
#if DEBUG // --find
            if (Find)
            {
                options.Replacer = ReplaceOptions.Empty;
                options.HighlightOptions = HighlightOptions.Match;
                options.DryRun = true;
            }
#endif
            return true;
        }
    }
}
