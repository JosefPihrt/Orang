// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal abstract class CommonCopyCommand<TOptions> :
        CommonFindCommand<TOptions> where TOptions : CommonCopyCommandOptions
    {
        protected CommonCopyCommand(TOptions options) : base(options)
        {
        }

        public string Target => Options.Target;

        public ConflictResolution ConflictResolution
        {
            get { return Options.ConflictResolution; }
            protected set { Options.ConflictResolution = value; }
        }

        protected HashSet<string>? IgnoredPaths { get; set; }

        protected abstract void ExecuteOperation(string sourcePath, string destinationPath);

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            if (Options.TargetNormalized == null)
                Options.TargetNormalized = Target.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            string pathNormalized = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (FileSystemHelpers.IsSubdirectory(Options.TargetNormalized, pathNormalized)
                || FileSystemHelpers.IsSubdirectory(pathNormalized, Options.TargetNormalized))
            {
                WriteWarning("Source directory cannot be subdirectory of a destination directory or vice versa.");
                return;
            }

            base.ExecuteDirectory(directoryPath, context);
        }

        protected override void ExecuteMatchWithContentCore(
            FileMatch fileMatch,
            SearchContext context,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            ExecuteMatchCore(fileMatch, context, baseDirectoryPath, columnWidths);
        }

        protected sealed override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            ExecuteOperation(fileMatch, context, baseDirectoryPath, GetPathIndent(baseDirectoryPath));

            if (context.TerminationReason != TerminationReason.Canceled)
                AskToContinue(context, GetPathIndent(baseDirectoryPath));
        }

        private void ExecuteOperation(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath,
            string indent)
        {
            string sourcePath = fileMatch.Path;
            string destinationPath;

            if (fileMatch.IsDirectory
                || (baseDirectoryPath != null && !Options.Flat))
            {
                Debug.Assert(sourcePath.StartsWith(baseDirectoryPath!, FileSystemHelpers.Comparison));

                string relativePath = sourcePath.Substring(baseDirectoryPath!.Length + 1);

                destinationPath = Path.Combine(Target, relativePath);
            }
            else
            {
                string fileName = Path.GetFileName(sourcePath);

                destinationPath = Path.Combine(Target, fileName);
            }

            if (IgnoredPaths?.Contains(sourcePath) == true)
                return;

            try
            {
                ExecuteOperation(context, sourcePath, destinationPath, fileMatch.IsDirectory, indent);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                WriteError(ex, sourcePath, indent: indent);
            }
        }

        protected virtual void ExecuteOperation(
            SearchContext context,
            string sourcePath,
            string destinationPath,
            bool isDirectory,
            string indent)
        {
            bool fileExists = File.Exists(destinationPath);
            bool directoryExists = !fileExists && Directory.Exists(destinationPath);
            var ask = false;

            if (isDirectory)
            {
                if (fileExists)
                {
                    ask = true;
                }
                else if (directoryExists)
                {
                    if (File.GetAttributes(sourcePath) == File.GetAttributes(destinationPath))
                        return;

                    ask = true;
                }
            }
            else if (fileExists)
            {
                if (Options.CompareOptions != FileCompareOptions.None
                    && FileSystemHelpers.FileEquals(sourcePath, destinationPath, Options.CompareOptions))
                {
                    return;
                }

                ask = true;
            }
            else if (directoryExists)
            {
                ask = true;
            }

            if (ask
                && ConflictResolution == ConflictResolution.Skip)
            {
                return;
            }

            if (!isDirectory
                && fileExists
                && ConflictResolution == ConflictResolution.Suffix)
            {
                destinationPath = FileSystemHelpers.CreateNewFilePath(destinationPath);
            }

            if (!Options.OmitPath)
            {
                LogHelpers.WritePath(
                    destinationPath,
                    basePath: Target,
                    relativePath: Options.DisplayRelativePath,
                    colors: Colors.Matched_Path,
                    indent: indent,
                    verbosity: Verbosity.Minimal);

                WriteLine(Verbosity.Minimal);
            }

            if (ask
                && ConflictResolution == ConflictResolution.Ask)
            {
                string question;
                if (directoryExists)
                {
                    question = (isDirectory) ? "Update directory attributes?" : "Overwrite directory?";
                }
                else
                {
                    question = "Overwrite file?";
                }

                DialogResult dialogResult = ConsoleHelpers.Ask(question, indent);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        {
                            break;
                        }
                    case DialogResult.YesToAll:
                        {
                            ConflictResolution = ConflictResolution.Overwrite;
                            break;
                        }
                    case DialogResult.No:
                    case DialogResult.None:
                        {
                            return;
                        }
                    case DialogResult.NoToAll:
                        {
                            ConflictResolution = ConflictResolution.Skip;
                            return;
                        }
                    case DialogResult.Cancel:
                        {
                            context.TerminationReason = TerminationReason.Canceled;
                            return;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown enum value '{dialogResult}'.");
                        }
                }
            }

            if (isDirectory)
            {
                if (directoryExists)
                {
                    if (!Options.DryRun)
                        FileSystemHelpers.UpdateAttributes(sourcePath, destinationPath);
                }
                else
                {
                    if (fileExists
                        && !Options.DryRun)
                    {
                        File.Delete(destinationPath);
                    }

                    if (!Options.DryRun)
                        Directory.CreateDirectory(destinationPath);
                }

                context.Telemetry.ProcessedDirectoryCount++;
            }
            else
            {
                if (fileExists)
                {
                    if (!Options.DryRun)
                        File.Delete(destinationPath);
                }
                else if (directoryExists)
                {
                    if (!Options.DryRun)
                        Directory.Delete(destinationPath, recursive: true);
                }
                else if (!Options.DryRun)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                }

                if (!Options.DryRun)
                    ExecuteOperation(sourcePath, destinationPath);

                context.Telemetry.ProcessedFileCount++;
            }
        }

        protected sealed override void WritePath(
            SearchContext context,
            FileMatch fileMatch,
            string? baseDirectoryPath,
            string indent,
            ColumnWidths? columnWidths)
        {
        }

        protected virtual void WriteError(
            Exception ex,
            string path,
            string indent)
        {
            LogHelpers.WriteFileError(ex, path, relativePath: Options.DisplayRelativePath, indent: indent);
        }
    }
}
