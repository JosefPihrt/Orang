// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class RenameCommand : DeleteOrRenameCommand<RenameCommandOptions>
    {
        public RenameCommand(RenameCommandOptions options) : base(options)
        {
            Debug.Assert(options.NameFilter?.IsNegative == false);
        }

        public ConflictResolution ConflictResolution
        {
            get { return Options.ConflictResolution; }
            protected set { Options.ConflictResolution = value; }
        }

        public override bool CanUseResults => false;

        protected override void OnSearchCreating(FileSystemSearch search)
        {
            search.DisallowEnumeration = !Options.DryRun;
            search.MatchPartOnly = true;

            base.OnSearchCreating(search);
        }

        protected override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath,
            ColumnWidths? columnWidths)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            List<ReplaceItem> replaceItems = ReplaceHelpers.GetReplaceItems(
                fileMatch.NameMatch!,
                Options.ReplaceOptions,
                NameFilter!.Predicate,
                context.CancellationToken);

            string path = fileMatch.Path;
            string newPath = ReplaceHelpers.GetNewPath(fileMatch, replaceItems);
            bool changed = !string.Equals(path, newPath, StringComparison.Ordinal);

            if (Options.Interactive
                || (!Options.OmitPath && changed))
            {
                LogHelpers.WritePath(
                    fileMatch,
                    replaceItems,
                    baseDirectoryPath,
                    relativePath: Options.DisplayRelativePath,
                    colors: Colors.Matched_Path,
                    matchColors: (Options.HighlightMatch) ? Colors.Match : default,
                    replaceColors: (Options.HighlightReplacement) ? Colors.Replacement : default,
                    indent: indent,
                    verbosity: Verbosity.Minimal);

                WriteProperties(context, fileMatch, columnWidths);
                WriteLine(Verbosity.Minimal);
            }

            ListCache<ReplaceItem>.Free(replaceItems);

            if (Options.Interactive)
            {
                string newName = Path.GetFileName(newPath);

                string newName2 = ConsoleHelpers.ReadUserInput(newName, indent + "New name: ");

                newPath = newPath.Substring(0, newPath.Length - newName.Length) + newName2;

                changed = !string.Equals(path, newPath, StringComparison.Ordinal);
            }

            if (!changed)
                return;

            if (FileSystemHelpers.ContainsInvalidFileNameChars(newPath, FileSystemHelpers.GetFileNameIndex(path)))
            {
                WriteWarning($"{indent}New file name contains invalid character(s).", verbosity: Verbosity.Normal);
                return;
            }

            bool fileExists = File.Exists(newPath);
            bool directoryExists = !fileExists && Directory.Exists(newPath);

            if (fileExists
                || directoryExists)
            {
                if (ConflictResolution == ConflictResolution.Skip)
                    return;
            }

            string question;
            if (fileExists)
            {
                question = "Rename (and delete existing file)?";
            }
            else if (directoryExists)
            {
                question = "Rename (and delete existing directory)?";
            }
            else
            {
                question = "Rename?";
            }

            if ((fileExists || directoryExists)
                && ConflictResolution == ConflictResolution.Ask)
            {
                if (!AskToOverwrite(context, question, indent))
                    return;
            }
            else if (Options.AskMode == AskMode.File)
            {
                if (!AskToExecute(context, question, indent))
                    return;
            }

            var renamed = false;

            try
            {
                if (!Options.DryRun)
                {
                    if (fileExists)
                    {
                        File.Delete(newPath);
                    }
                    else if (directoryExists)
                    {
                        Directory.Delete(newPath, recursive: true);
                    }

                    if (fileMatch.IsDirectory)
                    {
                        Directory.Move(path, newPath);
                    }
                    else
                    {
                        File.Move(path, newPath);
                    }

                    renamed = true;
                }

                if (fileMatch.IsDirectory)
                {
                    context.Telemetry.ProcessedDirectoryCount++;
                }
                else
                {
                    context.Telemetry.ProcessedFileCount++;
                }
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                WriteFileError(ex, indent: indent);
            }

            if (fileMatch.IsDirectory
                && renamed)
            {
                OnDirectoryChanged(new DirectoryChangedEventArgs(path, newPath));
            }
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget, verbosity);
            WriteProcessedFilesAndDirectories(
                telemetry,
                Options.SearchTarget,
                "Renamed files",
                "Renamed directories",
                Options.DryRun,
                verbosity);
        }

        private bool AskToOverwrite(SearchContext context, string question, string indent)
        {
            DialogResult result = ConsoleHelpers.Ask(question, indent);

            switch (result)
            {
                case DialogResult.Yes:
                    {
                        return true;
                    }
                case DialogResult.YesToAll:
                    {
                        ConflictResolution = ConflictResolution.Overwrite;
                        return true;
                    }
                case DialogResult.No:
                case DialogResult.None:
                    {
                        return false;
                    }
                case DialogResult.NoToAll:
                    {
                        ConflictResolution = ConflictResolution.Skip;
                        return false;
                    }
                case DialogResult.Cancel:
                    {
                        context.TerminationReason = TerminationReason.Canceled;
                        return false;
                    }
                default:
                    {
                        throw new InvalidOperationException($"Unknown enum value '{result}'.");
                    }
            }
        }
    }
}
