// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("rename", HelpText = "Renames files and directories.")]
    [OptionValueProvider(nameof(Content), OptionValueProviderNames.PatternOptionsWithoutPart)]
    [OptionValueProvider(nameof(Display), OptionValueProviderNames.Display_NonContent)]
    [OptionValueProvider(nameof(Highlight), OptionValueProviderNames.RenameHighlightOptions)]
    [OptionValueProvider(nameof(Name), OptionValueProviderNames.PatternOptionsWithoutGroupAndNegative)]
    [CommandGroup("File System", 1)]
    internal sealed class RenameCommandLineOptions : DeleteOrRenameCommandLineOptions
    {
        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask for a permission to rename file or directory.")]
        public bool Ask { get; set; }

        [Option(
            longName: OptionNames.Conflict,
            HelpText = "Defines how to resolve conflict when a file/directory already exists.",
            MetaValue = MetaValues.ConflictResolution)]
        public string Conflict { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Content,
            longName: OptionNames.Content,
            HelpText = "Regular expression for files' content.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Content { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files/directories should be renamed "
                + "but do not actually rename any file/directory.")]
        public bool DryRun { get; set; }

        [Hidden]
        [Option(
            longName: OptionNames.Evaluator,
            HelpText = "[deprecated] Use option -r, --replacement instead.",
            MetaValue = MetaValues.Evaluator)]
        public string Evaluator { get; set; } = null!;

        [Option(
            longName: OptionNames.Interactive,
            HelpText = "Enable editing of a new name.")]
        public bool Interactive { get; set; }

        [Option(
            shortName: OptionShortNames.MaxCount,
            longName: OptionNames.MaxCount,
            HelpText = "Stop renaming after specified number is reached.",
            MetaValue = MetaValues.Num)]
        public int MaxCount { get; set; }

        [Option(
            longName: OptionNames.Modify,
            HelpText = "Functions to modify result.",
            MetaValue = MetaValues.ReplaceModify)]
        public IEnumerable<string> Modify { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Name,
            longName: OptionNames.Name,
            Required = true,
            HelpText = "Regular expression for file or directory name.",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.Replacement,
            longName: OptionNames.Replacement,
            HelpText = "Replacement pattern.",
            MetaValue = MetaValues.Replacement)]
        public IEnumerable<string> Replacement { get; set; } = null!;

        public bool TryParse(RenameCommandOptions options)
        {
            var baseOptions = (DeleteOrRenameCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (RenameCommandOptions)baseOptions;

            if (!TryParseHighlightOptions(
                Highlight,
                out HighlightOptions highlightOptions,
                defaultValue: HighlightOptions.Replacement,
                provider: OptionValueProviders.RenameHighlightOptionsProvider))
            {
                return false;
            }

            if (!FilterParser.TryParse(
                Name,
                OptionNames.Name,
                OptionValueProviders.PatternOptionsWithoutGroupAndNegativeProvider,
                out Filter? nameFilter,
                out FileNamePart namePart,
                namePartProvider: OptionValueProviders.NamePartKindProvider_WithoutFullName))
            {
                return false;
            }

            if (!FilterParser.TryParse(
                Content,
                OptionNames.Content,
                OptionValueProviders.PatternOptionsWithoutPartProvider,
                out Filter? contentFilter,
                allowNull: true))
            {
                return false;
            }

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

            if (!TryParseAsEnum(
                Conflict,
                OptionNames.Conflict,
                out ConflictResolution conflictResolution,
                defaultValue: ConflictResolution.Ask,
                provider: OptionValueProviders.ConflictResolutionProvider_WithoutSuffix))
            {
                return false;
            }

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle? contentDisplayStyle,
                pathDisplayStyle: out PathDisplayStyle? pathDisplayStyle,
                lineDisplayOptions: out LineDisplayOptions lineDisplayOptions,
                lineContext: out LineContext lineContext,
                displayParts: out DisplayParts displayParts,
                fileProperties: out ImmutableArray<FileProperty> fileProperties,
                indent: out string? indent,
                separator: out string? separator,
                noAlign: out bool noAlign,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider_Rename))
            {
                return false;
            }

            if (pathDisplayStyle == PathDisplayStyle.Relative
                && options.Paths.Length > 1
                && options.SortOptions != null)
            {
                pathDisplayStyle = PathDisplayStyle.Full;
            }

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle ?? ContentDisplayStyle.Omit,
                pathDisplayStyle: pathDisplayStyle ?? PathDisplayStyle.Full,
                lineOptions: lineDisplayOptions,
                lineContext: lineContext,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator,
                alignColumns: !noAlign);

            options.HighlightOptions = highlightOptions;
            options.SearchTarget = GetSearchTarget();
            options.ReplaceOptions = replaceOptions;
            options.AskMode = (Ask) ? AskMode.File : AskMode.None;
            options.DryRun = DryRun;
            options.NameFilter = nameFilter;
            options.NamePart = namePart;
            options.ContentFilter = contentFilter;
            options.MaxMatchingFiles = MaxCount;
            options.ConflictResolution = conflictResolution;
            options.Interactive = Interactive;

            return true;
        }
    }
}
