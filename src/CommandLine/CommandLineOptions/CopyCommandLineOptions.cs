// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("copy", HelpText = "Searches the file system for files and directories and copy them to a destination directory.")]
    internal sealed class CopyCommandLineOptions : CommonCopyCommandLineOptions
    {
        [Option(
            longName: OptionNames.Conflict,
            HelpText = "Defines how to resolve conflict when a file/directory already exists.",
            MetaValue = MetaValues.ConflictResolution)]
        public string OnConflict { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files or directories should be copied " +
                "but do not actually copy any file or directory.")]
        public bool DryRun { get; set; }

        [Option(
            longName: OptionNames.Flat,
            HelpText = "Copy files directly into target directory.")]
        public bool Flat { get; set; }

        [Option(
            longName: OptionNames.Target,
            Required = true,
            HelpText = "A directory to copy files and directories to.",
            MetaValue = MetaValues.DirectoryPath)]
        public string Target { get; set; } = null!;

        public bool TryParse(CopyCommandOptions options)
        {
            var baseOptions = (CommonCopyCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (CopyCommandOptions)baseOptions;

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
                OnConflict,
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
