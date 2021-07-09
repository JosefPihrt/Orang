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
            if (ShouldLog(Verbosity.Minimal))
            {
                PathWriter = new PathWriter(
                    pathColors: default,
                    relativePath: Options.DisplayRelativePath);
            }
        }

        private string Target => Options.Target;

        public ConflictResolution ConflictResolution
        {
            get { return Options.ConflictResolution; }
            protected set { Options.ConflictResolution = value; }
        }

        protected HashSet<string>? IgnoredPaths { get; set; }

        private PathWriter? PathWriter { get; }

        protected override void OnSearchCreating(FileSystemSearch search)
        {
            if (Options.SearchTarget != SearchTarget.Files
                && !Options.StructureOnly)
            {
                search.CanRecurseMatch = false;
            }
        }

        protected abstract string GetQuestionText(bool isDirectory);

        protected abstract void ExecuteOperation(string sourcePath, string destinationPath);

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            string targetNormalized = Target.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string pathNormalized = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (FileSystemHelpers.IsSubdirectory(targetNormalized, pathNormalized)
                || FileSystemHelpers.IsSubdirectory(pathNormalized, targetNormalized))
            {
                WriteWarning("Source directory cannot be subdirectory of a destination directory or vice versa.");
                return;
            }

            base.ExecuteDirectory(directoryPath, context);
        }

        protected sealed override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            ExecuteOperation(fileMatch, context, baseDirectoryPath, GetPathIndent(baseDirectoryPath));
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

            try
            {
                ExecuteOperation(context, sourcePath, destinationPath, fileMatch.IsDirectory, indent);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                WriteError(context, ex, sourcePath, indent: indent);
            }
        }

        protected virtual DialogResult? ExecuteOperation(
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
                    if (Options.StructureOnly
                        && FileSystemHelpers.AttributeEquals(sourcePath, destinationPath, Options.NoCompareAttributes))
                    {
                        return null;
                    }

                    ask = true;
                }
            }
            else if (fileExists)
            {
                if (Options.CompareOptions != FileCompareOptions.None
                    && FileSystemHelpers.FileEquals(
                        sourcePath,
                        destinationPath,
                        Options.CompareOptions,
                        Options.NoCompareAttributes,
                        Options.AllowedTimeDiff))
                {
                    return null;
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
                return null;
            }

            if (ConflictResolution == ConflictResolution.Suffix
                && ((isDirectory && directoryExists)
                    || (!isDirectory && fileExists)))
            {
                destinationPath = FileSystemHelpers.CreateNewFilePath(destinationPath);
            }

            if (!Options.OmitPath)
            {
                PathWriter?.WritePath(destinationPath, Target, indent);

                WriteLine(Verbosity.Minimal);
            }

            if (ask
                && ConflictResolution == ConflictResolution.Ask)
            {
                string question;
                if (directoryExists)
                {
                    question = (isDirectory && Options.StructureOnly)
                        ? "Update directory attributes?"
                        : "Overwrite directory?";
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
                            return null;
                        }
                    case DialogResult.NoToAll:
                        {
                            ConflictResolution = ConflictResolution.Skip;
                            return DialogResult.NoToAll;
                        }
                    case DialogResult.Cancel:
                        {
                            context.TerminationReason = TerminationReason.Canceled;
                            return DialogResult.Cancel;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown enum value '{dialogResult}'.");
                        }
                }
            }
            else if (Options.AskMode == AskMode.File
                && !AskToExecute(context, GetQuestionText(isDirectory), indent))
            {
                return DialogResult.No;
            }

            if (isDirectory)
            {
                if (directoryExists)
                {
                    if (!Options.DryRun)
                    {
                        if (Options.StructureOnly)
                        {
                            FileSystemHelpers.UpdateAttributes(sourcePath, destinationPath);
                        }
                        else
                        {
                            CopyDirectory(sourcePath, destinationPath, indent, context);
                        }
                    }
                }
                else
                {
                    if (fileExists
                        && !Options.DryRun)
                    {
                        File.Delete(destinationPath);
                    }

                    if (!Options.DryRun)
                    {
                        if (Options.StructureOnly)
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            CopyDirectory(sourcePath, destinationPath, indent, context);
                        }
                    }
                }

                context.Telemetry.ProcessedDirectoryCount++;
            }
            else
            {
                if (!Options.DryRun)
                {
                    if (fileExists)
                    {
                        File.Delete(destinationPath);
                    }
                    else if (directoryExists)
                    {
                        Directory.Delete(destinationPath, recursive: true);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    }

                    ExecuteOperation(sourcePath, destinationPath);
                }

                context.Telemetry.ProcessedFileCount++;
            }

            return null;
        }

        private void CopyDirectory(
            string sourcePath,
            string destinationPath,
            string indent,
            SearchContext context)
        {
            var directories = new Stack<(string, string)>();

            directories.Push((sourcePath, destinationPath));

            CopyDirectory(directories, indent, context);
        }

        private void CopyDirectory(
            Stack<(string, string)> directories,
            string indent,
            SearchContext context)
        {
            while (directories.Count > 0)
            {
                (string sourcePath, string destinationPath) = directories.Pop();

                var isEmpty = true;

                using (IEnumerator<string> en = Directory.EnumerateFiles(sourcePath).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        isEmpty = false;

                        do
                        {
                            string newFilePath = Path.Combine(destinationPath, Path.GetFileName(en.Current));

                            DialogResult? result = ExecuteOperation(
                                context,
                                en.Current,
                                newFilePath,
                                isDirectory: false,
                                indent);

                            if (result == DialogResult.NoToAll
                                || result == DialogResult.Cancel)
                            {
                                return;
                            }

                        } while (en.MoveNext());
                    }
                }

                using (IEnumerator<string> en = Directory.EnumerateDirectories(sourcePath).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        isEmpty = false;

                        do
                        {
                            string newDirectoryPath = Path.Combine(destinationPath, Path.GetFileName(en.Current));

                            directories.Push((en.Current, newDirectoryPath));

                        } while (en.MoveNext());
                    }
                }

                if (isEmpty
                    && !Options.DryRun)
                {
                    Directory.CreateDirectory(destinationPath);
                }
            }
        }

        protected virtual void WriteError(
            SearchContext context,
            Exception ex,
            string path,
            string indent)
        {
            LogHelpers.WriteFileError(ex, path, indent: indent);
        }
    }
}
