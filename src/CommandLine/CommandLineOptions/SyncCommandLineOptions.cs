// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;
using Orang.FileSystem;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    //TODO: --allowed-time-diff <SECONDS>
    [Verb("sync", HelpText = "Synchronizes content of two directories.")]
    internal sealed class SyncCommandLineOptions : CommonCopyCommandLineOptions
    {
        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask for a permission to synchronize file or directory.")]
        public bool Ask { get; set; }

        [Option(
            longName: OptionNames.Conflict,
            HelpText = "Action to choose if a file or directory exists in one directory and it is missing in the second directory.",
            MetaValue = MetaValues.SyncConflictResolution)]
        public string Conflict { get; set; } = null!;

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: OptionNames.DryRun,
            HelpText = "Display which files or directories should be copied/deleted "
                + "but do not actually copy/delete any file or directory.")]
        public bool DryRun { get; set; }

        //TODO: rename Right > Second
        [Option(
            shortName: OptionShortNames.Right,
            longName: OptionNames.Right,
            Required = true,
            HelpText = "A right (second) directory to be synchronized with the left (first) directory.",
            MetaValue = MetaValues.DirectoryPath)]
        public string Right { get; set; } = null!;

        public bool TryParse(SyncCommandOptions options)
        {
            var baseOptions = (CommonCopyCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (SyncCommandOptions)baseOptions;

            if (options.Paths.Length > 1)
            {
                Logger.WriteError("More than one source directory cannot be synchronized.");
                return false;
            }

            if (!TryParseAsEnumFlags(
                Compare,
                OptionNames.Compare,
                out FileCompareOptions compareOptions,
                FileCompareOptions.Attributes | FileCompareOptions.Content | FileCompareOptions.ModifiedTime | FileCompareOptions.Size,
                OptionValueProviders.FileCompareOptionsProvider))
            {
                return false;
            }

            if (!TryEnsureFullPath(Right, out string? rightDirectory))
                return false;

            if (!TryParseAsEnum(
                Conflict,
                OptionNames.Conflict,
                out SyncConflictResolution conflictResolution,
                defaultValue: SyncConflictResolution.LeftWins,
                provider: OptionValueProviders.SyncConflictResolutionProvider))
            {
                return false;
            }

            options.SearchTarget = SearchTarget.All;
            options.CompareOptions = compareOptions;
            options.DryRun = DryRun;
            options.Target = rightDirectory;
            options.ConflictResolution = conflictResolution;
            options.AskMode = (Ask) ? AskMode.File : AskMode.None;

            return true;
        }
    }
}
