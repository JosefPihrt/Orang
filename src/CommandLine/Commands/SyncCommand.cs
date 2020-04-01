﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal sealed class SyncCommand : CommonCopyCommand<SyncCommandOptions>
    {
        private bool _isRightToLeft;
        private HashSet<string> _destinationPaths;
        private HashSet<string> _ignoredPaths;

        public SyncCommand(SyncCommandOptions options) : base(options)
        {
        }

        public bool DryRun => Options.DryRun;

        new public SyncConflictResolution ConflictResolution
        {
            get { return Options.ConflictResolution; }
            private set { Options.ConflictResolution = value; }
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            _destinationPaths = new HashSet<string>(FileSystemHelpers.Comparer);

            try
            {
                _isRightToLeft = false;
                base.ExecuteDirectory(directoryPath, context);
            }
            finally
            {
                _isRightToLeft = true;
            }

            _ignoredPaths = _destinationPaths;
            _destinationPaths = null;

            string rightDirectory = directoryPath;
            directoryPath = Options.Target;

            Options.Paths = ImmutableArray.Create(new PathInfo(directoryPath, PathOrigin.None));
            Options.Target = rightDirectory;

            if (ConflictResolution == SyncConflictResolution.LeftWins)
            {
                ConflictResolution = SyncConflictResolution.RightWins;
            }
            else if (ConflictResolution == SyncConflictResolution.RightWins)
            {
                ConflictResolution = SyncConflictResolution.LeftWins;
            }

            base.ExecuteDirectory(directoryPath, context);

            _ignoredPaths = null;
        }

        protected override void ExecuteOperation(SearchContext context, string sourcePath, string destinationPath, bool isDirectory, string indent)
        {
            if (_ignoredPaths?.Contains(sourcePath) == true)
                return;

            ExecuteOperation();

            _destinationPaths?.Add(destinationPath);

            void ExecuteOperation()
            {
                bool fileExists = File.Exists(destinationPath);
                bool directoryExists = !fileExists && Directory.Exists(destinationPath);

                bool? preferLeft = null;

                if (isDirectory)
                {
                    if (directoryExists)
                    {
                        Debug.Assert(!_isRightToLeft);

                        if (_isRightToLeft)
                            return;

                        if (File.GetAttributes(sourcePath) == File.GetAttributes(destinationPath))
                            return;
                    }
                }
                else if (fileExists)
                {
                    Debug.Assert(!_isRightToLeft);

                    if (_isRightToLeft)
                        return;

                    int diff = File.GetLastWriteTimeUtc(sourcePath).CompareTo(File.GetLastWriteTimeUtc(destinationPath));

                    if (diff > 0)
                    {
                        preferLeft = true;
                    }
                    else if (diff < 0)
                    {
                        preferLeft = false;
                    }
                }

                if (preferLeft == null)
                {
                    if (!_isRightToLeft
                        && !isDirectory
                        && fileExists
                        && Options.CompareOptions != FileCompareOptions.None
                        && FileSystemHelpers.FileEquals(sourcePath, destinationPath, Options.CompareOptions))
                    {
                        return;
                    }

                    if (ConflictResolution == SyncConflictResolution.Ask)
                    {
                        string leftPrefix = GetPrefix(invert: _isRightToLeft);
                        string rightPrefix = GetPrefix(invert: !_isRightToLeft);

                        WritePathPrefix((_isRightToLeft) ? destinationPath : sourcePath, leftPrefix, default, indent);
                        WritePathPrefix((_isRightToLeft) ? sourcePath : destinationPath, rightPrefix, default, indent);

                        DialogResult dialogResult = ConsoleHelpers.Ask("Prefer left directory?", indent);

                        switch (dialogResult)
                        {
                            case DialogResult.Yes:
                                {
                                    preferLeft = true;
                                    break;
                                }
                            case DialogResult.YesToAll:
                                {
                                    preferLeft = true;
                                    ConflictResolution = SyncConflictResolution.LeftWins;
                                    break;
                                }
                            case DialogResult.No:
                                {
                                    preferLeft = false;
                                    break;
                                }
                            case DialogResult.NoToAll:
                                {
                                    preferLeft = false;
                                    ConflictResolution = SyncConflictResolution.RightWins;
                                    break;
                                }
                            case DialogResult.None:
                                {
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
                    else if (ConflictResolution == SyncConflictResolution.LeftWins)
                    {
                        preferLeft = true;
                    }
                    else if (ConflictResolution == SyncConflictResolution.RightWins)
                    {
                        preferLeft = false;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown enum value '{ConflictResolution}'.");
                    }
                }

                preferLeft ??= true;

                if (_isRightToLeft)
                    preferLeft = !preferLeft;

                if (isDirectory)
                {
                    ExecuteDirectoryOperations(context.Telemetry, sourcePath, destinationPath, fileExists, directoryExists, preferLeft.Value, indent);
                }
                else
                {
                    ExecuteFileOperations(context.Telemetry, sourcePath, destinationPath, fileExists, directoryExists, preferLeft.Value, indent);
                }

                string GetPrefix(bool invert)
                {
                    if (invert)
                    {
                        if (isDirectory)
                        {
                            return (directoryExists) ? "D" : "X";
                        }
                        else
                        {
                            return (fileExists) ? "F" : "X";
                        }
                    }
                    else
                    {
                        return (isDirectory) ? "D" : "F";
                    }
                }
            }
        }

        private void ExecuteDirectoryOperations(
            SearchTelemetry telemetry,
            string sourcePath,
            string destinationPath,
            bool fileExists,
            bool directoryExists,
            bool preferLeft,
            string indent)
        {
            if (preferLeft)
            {
                if (directoryExists)
                {
                    // update directory's attributes
                    WritePath(destinationPath, OperationKind.Update, indent);
                    UpdateAttributes(sourcePath, destinationPath);
                    telemetry.UpdatedCount++;
                }
                else
                {
                    if (fileExists)
                    {
                        // create directory (overwrite existing file)
                        WritePath(destinationPath, OperationKind.Delete, indent);
                        DeleteFile(destinationPath);
                        telemetry.DeletedCount++;
                    }
                    else
                    {
                        // create directory
                    }

                    WritePath(destinationPath, OperationKind.Add, indent);
                    CreateDirectory(destinationPath);
                    telemetry.AddedCount++;
                }
            }
            else if (directoryExists)
            {
                // update directory's attributes
                WritePath(sourcePath, OperationKind.Update, indent);
                UpdateAttributes(destinationPath, sourcePath);
                telemetry.UpdatedCount++;
            }
            else
            {
                WritePath(sourcePath, OperationKind.Delete, indent);
                DeleteDirectory(sourcePath);
                telemetry.DeletedCount++;

                if (fileExists)
                {
                    // copy file (and overwrite existing directory)
                    WritePath(sourcePath, OperationKind.Add, indent);
                    CopyFile(destinationPath, sourcePath);
                    telemetry.AddedCount++;
                }
                else
                {
                    // delete directory
                }
            }
        }

        private void ExecuteFileOperations(
            SearchTelemetry telemetry,
            string sourcePath,
            string destinationPath,
            bool fileExists,
            bool directoryExists,
            bool preferLeft,
            string indent)
        {
            if (preferLeft)
            {
                if (fileExists)
                {
                    // copy file (and overwrite existing file)
                    WritePath(destinationPath, OperationKind.Update, indent);
                    DeleteFile(destinationPath);
                }
                else if (directoryExists)
                {
                    // copy file (and overwrite existing directory)
                    WritePath(destinationPath, OperationKind.Delete, indent);
                    DeleteDirectory(destinationPath);
                    telemetry.DeletedCount++;
                }
                else
                {
                    // copy file
                }

                if (!fileExists)
                    WritePath(destinationPath, OperationKind.Add, indent);

                CopyFile(sourcePath, destinationPath);

                if (fileExists)
                {
                    telemetry.UpdatedCount++;
                }
                else
                {
                    telemetry.AddedCount++;
                }
            }
            else
            {
                WritePath(sourcePath, (fileExists) ? OperationKind.Update : OperationKind.Delete, indent);
                DeleteFile(sourcePath);

                if (!fileExists)
                    telemetry.DeletedCount++;

                if (fileExists)
                {
                    // copy file (and overwrite existing file)
                    CopyFile(destinationPath, sourcePath);
                    telemetry.UpdatedCount++;
                }
                else if (directoryExists)
                {
                    // create directory (and overwrite existing file)
                    WritePath(sourcePath, OperationKind.Add, indent);
                    CreateDirectory(sourcePath);
                    telemetry.AddedCount++;
                }
                else
                {
                    // delete file
                }
            }
        }

        private void DeleteDirectory(string path)
        {
            if (!DryRun)
                Directory.Delete(path, recursive: true);
        }

        private void CreateDirectory(string path)
        {
            if (!DryRun)
                Directory.CreateDirectory(path);
        }

        private void DeleteFile(string path)
        {
            if (!DryRun)
                File.Delete(path);
        }

        private void CopyFile(string sourcePath, string destinationPath)
        {
            if (!DryRun)
                File.Copy(sourcePath, destinationPath);
        }

        private void UpdateAttributes(string sourcePath, string destinationPath)
        {
            if (!DryRun)
                FileSystemHelpers.UpdateAttributes(sourcePath, destinationPath);
        }

        protected override void ExecuteOperation(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
        }

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            throw new InvalidOperationException("File cannot be synchronized.");
        }

        private void WritePath(string path, OperationKind kind, string indent)
        {
            if (!ShouldLog(Verbosity.Minimal))
                return;

            switch (kind)
            {
                case OperationKind.None:
                    {
                        Debug.Fail("");

                        LogHelpers.WritePath(path, indent: indent, verbosity: Verbosity.Minimal);
                        WriteLine(Verbosity.Minimal);
                        break;
                    }
                case OperationKind.Add:
                    {
                        WritePathPrefix(path, "ADD", Colors.Sync_Add, indent);
                        break;
                    }
                case OperationKind.Update:
                    {
                        WritePathPrefix(path, "UPD", Colors.Sync_Update, indent);
                        break;
                    }
                case OperationKind.Delete:
                    {
                        WritePathPrefix(path, "DEL", Colors.Sync_Delete, indent);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unkonwn enum value '{kind}'.", nameof(kind));
                    }
            }
        }

        private void WritePathPrefix(string path, string prefix, ConsoleColors colors, string indent)
        {
            Write(indent, Verbosity.Minimal);
            Write(prefix, colors, Verbosity.Minimal);
            Write(" ", Verbosity.Minimal);
            LogHelpers.WritePath(path, verbosity: Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);
        }

        protected override void WriteError(Exception ex, string path, string indent)
        {
            Write(indent, Verbosity.Minimal);
            Write("ERR", Colors.Sync_Error, Verbosity.Minimal);
            Write(" ", Verbosity.Minimal);
            Write(ex.Message, verbosity: Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);
#if DEBUG
            WriteLine($"{indent}STACK TRACE:");
            WriteLine(ex.StackTrace);
#endif
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            base.WriteSummary(telemetry, verbosity);

            ConsoleColors colors = (Options.DryRun) ? Colors.Message_DryRun : default;

            WriteCount("Added", telemetry.AddedCount, colors, verbosity: verbosity);
            Write("  ", verbosity);
            WriteCount("Updated", telemetry.UpdatedCount, colors, verbosity: verbosity);
            Write("  ", verbosity);
            WriteCount("Deleted", telemetry.DeletedCount, colors, verbosity: verbosity);
            Write("  ", verbosity);
            WriteLine(verbosity);
        }

        private enum OperationKind
        {
            None,
            Add,
            Update,
            Delete
        }
    }
}
