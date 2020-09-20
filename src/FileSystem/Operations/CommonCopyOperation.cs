// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;

namespace Orang.Operations
{
    internal abstract class CommonCopyOperation : CommonFindOperation
    {
        protected CommonCopyOperation()
        {
        }

        public string DestinationPath { get; set; } = null!;

        public IDialogProvider<OperationProgress>? DialogProvider { get; set; }

        public CopyOptions CopyOptions { get; set; } = null!;

        public ConflictResolution ConflictResolution { get; set; }

        protected abstract void ExecuteOperation(string sourcePath, string destinationPath);

        protected override void ExecuteMatch(
            FileMatch fileMatch,
            string directoryPath)
        {
            string sourcePath = fileMatch.Path;
            string destinationPath;

            if (fileMatch.IsDirectory
                || (directoryPath != null && !CopyOptions.Flat))
            {
                Debug.Assert(sourcePath.StartsWith(directoryPath, FileSystemHelpers.Comparison));

                string relativePath = sourcePath.Substring(directoryPath.Length + 1);

                destinationPath = Path.Combine(DestinationPath, relativePath);
            }
            else
            {
                string fileName = Path.GetFileName(sourcePath);

                destinationPath = Path.Combine(DestinationPath, fileName);
            }

            try
            {
                ExecuteOperation(fileMatch, destinationPath, fileMatch.IsDirectory);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                Report(fileMatch, destinationPath, ex);
            }
        }

        protected virtual void ExecuteOperation(FileMatch fileMatch, string destinationPath, bool isDirectory)
        {
            string sourcePath = fileMatch.Path;
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
                if (CopyOptions.CompareOptions != FileCompareOptions.None
                    && FileSystemHelpers.FileEquals(sourcePath, destinationPath, CopyOptions.CompareOptions))
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

            if (ask
                && ConflictResolution == ConflictResolution.Ask)
            {
                DialogResult dialogResult = DialogProvider!.GetResult(new OperationProgress(fileMatch, destinationPath, OperationKind));

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
                            throw new OperationCanceledException();
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown enum value '{dialogResult}'.");
                        }
                }
            }

            Report(fileMatch, destinationPath);

            if (isDirectory)
            {
                if (directoryExists)
                {
                    if (!DryRun)
                        FileSystemHelpers.UpdateAttributes(sourcePath, destinationPath);
                }
                else
                {
                    if (fileExists
                        && !DryRun)
                    {
                        File.Delete(destinationPath);
                    }

                    if (!DryRun)
                        Directory.CreateDirectory(destinationPath);
                }

                Telemetry.ProcessedDirectoryCount++;
            }
            else
            {
                if (fileExists)
                {
                    if (!DryRun)
                        File.Delete(destinationPath);
                }
                else if (directoryExists)
                {
                    if (!DryRun)
                        Directory.Delete(destinationPath, recursive: true);
                }
                else if (!DryRun)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                }

                if (!DryRun)
                    ExecuteOperation(sourcePath, destinationPath);

                Telemetry.ProcessedFileCount++;
            }
        }
    }
}
