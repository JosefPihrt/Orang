// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("move", HelpText = "Searches the file system for files and directories and move them to a destination directory.")]
    internal sealed class MoveCommandLineOptions : CommonCopyCommandLineOptions
    {
        [Option(
            longName: OptionNames.Conflict,
            HelpText = "Defines how to resolve conflict when a file/directory already exists.",
            MetaValue = MetaValues.ConflictResolution)]
        public string Conflict { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files or directories should be moved but do not actually move any file or directory.")]
        public bool DryRun { get; set; }

        [Option(
            longName: OptionNames.Flat,
            HelpText = "Move files directly into target directory.")]
        public bool Flat { get; set; }

        [Option(
            longName: OptionNames.Target,
            Required = true,
            HelpText = "A directory to move files and directories to.",
            MetaValue = MetaValues.DirectoryPath)]
        public string Target { get; set; } = null!;

        public bool TryParse(MoveCommandOptions options)
        {
            var baseOptions = (CommonCopyCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (MoveCommandOptions)baseOptions;

            if (!TryParseAsEnumFlags(
                Compare,
                OptionNames.Compare,
                out FileCompareOptions compareOptions,
                FileCompareOptions.None,
                OptionValueProviders.FileCompareOptionsProvider))
            {
                return false;
            }

            if (!TryEnsureFullPath(Target, out string? target))
                return false;

            if (!TryParseAsEnum(
                Conflict,
                OptionNames.Conflict,
                out ConflictResolution conflictResolution,
                defaultValue: ConflictResolution.Ask,
                provider: OptionValueProviders.ConflictResolutionProvider))
            {
                return false;
            }

            options.CompareOptions = compareOptions;
            options.DryRun = DryRun;
            options.Flat = Flat;
            options.ConflictResolution = conflictResolution;
            options.Target = target;

            return true;
        }
    }
}
