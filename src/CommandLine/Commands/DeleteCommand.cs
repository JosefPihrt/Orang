﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;

namespace Orang.CommandLine
{
    internal class DeleteCommand : CommonFindCommand<DeleteCommandOptions>, INotifyDirectoryChanged
    {
        public DeleteCommand(DeleteCommandOptions options) : base(options)
        {
        }

        public event EventHandler<DirectoryChangedEventArgs> DirectoryChanged;

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            context.Telemetry.FileCount++;

            FileSystemFinderResult result = MatchFile(filePath, context.Progress);

            if (result != null)
                ProcessResult(result, context);
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            foreach (FileSystemFinderResult result in Find(directoryPath, context, notifyDirectoryChanged: this))
            {
                Debug.Assert(result.Path.StartsWith(directoryPath, FileSystemHelpers.Comparison), $"{directoryPath}\r\n{result.Path}");

                ProcessResult(result, context, directoryPath);

                if (context.TerminationReason == TerminationReason.Canceled)
                    break;

                if (context.TerminationReason == TerminationReason.MaxReached)
                    break;
            }
        }

        private void ProcessResult(
            FileSystemFinderResult result,
            SearchContext context,
            string baseDirectoryPath = null)
        {
            Debug.Assert(baseDirectoryPath == null || result.Path.StartsWith(baseDirectoryPath, FileSystemHelpers.Comparison), $"{baseDirectoryPath}\r\n{result.Path}");

            if (!result.IsDirectory
                && Options.ContentFilter != null)
            {
                string indent = GetPathIndent(baseDirectoryPath);

                string input = ReadFile(result.Path, baseDirectoryPath, Options.DefaultEncoding, context, indent);

                if (input == null)
                    return;

                if (!Options.ContentFilter.IsMatch(input))
                    return;
            }

            ExecuteOrAddResult(result, context, baseDirectoryPath);
        }

        protected override void ExecuteResult(SearchResult result, SearchContext context, ColumnWidths columnWidths)
        {
            ExecuteResult(result.Result, context, result.BaseDirectoryPath, columnWidths);
        }

        protected override void ExecuteResult(
            FileSystemFinderResult result,
            SearchContext context,
            string baseDirectoryPath = null,
            ColumnWidths columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            if (!Options.OmitPath)
                WritePath(context, result, baseDirectoryPath, indent, columnWidths);

            bool deleted = false;

            if (!Options.Ask || AskToDelete())
            {
                try
                {
                    if (!Options.DryRun)
                    {
                        FileSystemHelpers.Delete(
                            result,
                            contentOnly: Options.ContentOnly,
                            includingBom: Options.IncludingBom,
                            filesOnly: Options.FilesOnly,
                            directoriesOnly: Options.DirectoriesOnly);

                        deleted = true;
                    }

                    if (result.IsDirectory)
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
            }

            if (result.IsDirectory
                && deleted)
            {
                OnDirectoryChanged(new DirectoryChangedEventArgs(result.Path, null));
            }

            bool AskToDelete()
            {
                DialogResult result = ConsoleHelpers.Ask((Options.ContentOnly) ? "Delete content?" : "Delete?", indent);

                switch (result)
                {
                    case DialogResult.Yes:
                        {
                            return true;
                        }
                    case DialogResult.YesToAll:
                        {
                            Options.Ask = false;
                            return true;
                        }
                    case DialogResult.No:
                    case DialogResult.None:
                        {
                            return false;
                        }
                    case DialogResult.NoToAll:
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

        protected virtual void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget, verbosity);

            string filesTitle = (Options.ContentOnly)
                ? "Deleted files content"
                : "Deleted files";

            string directoriesTitle = (Options.ContentOnly)
                ? "Deleted directories content"
                : "Deleted directories";

            WriteProcessedFilesAndDirectories(telemetry, Options.SearchTarget, filesTitle, directoriesTitle, Options.DryRun, verbosity);
        }
    }
}
