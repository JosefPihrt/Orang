// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal sealed class SyncCommand :
        CommonCopyCommand<SyncCommandOptions>,
        INotifyDirectoryChanged
    {
        private bool _isSecondToFirst;
        private HashSet<string>? _destinationPaths;
        private HashSet<string>? _ignoredPaths;
        private DirectoryData? _directoryData;

        public SyncCommand(SyncCommandOptions options) : base(options)
        {
        }

        public bool DryRun => Options.DryRun;

        new public SyncConflictResolution ConflictResolution
        {
            get { return Options.ConflictResolution; }
            private set { Options.ConflictResolution = value; }
        }

        public override bool CanEndProgress => false;

        public event EventHandler<DirectoryChangedEventArgs>? DirectoryChanged;

        private void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }

        protected override string GetQuestionText(bool isDirectory)
        {
            return (isDirectory) ? "Sync directory?" : "Sync file?";
        }

        protected override void OnSearchCreating(FileSystemSearch search)
        {
            search.CanRecurseMatch = true;
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            bool secondExists = Directory.Exists(Options.Target);

            if (secondExists)
                _destinationPaths = new HashSet<string>(FileSystemHelpers.Comparer);

            try
            {
                _isSecondToFirst = false;

                if (ConflictResolution == SyncConflictResolution.FirstWins
                    && !secondExists)
                {
                    CreateDirectory(Options.Target);
                }

                base.ExecuteDirectory(directoryPath, context);
            }
            finally
            {
                _isSecondToFirst = true;
            }

            if (secondExists)
            {
                _ignoredPaths = _destinationPaths;
                _destinationPaths = null;

                string secondDirectory = directoryPath;
                directoryPath = Options.Target;

                Options.Paths = ImmutableArray.Create(new PathInfo(directoryPath, PathOrigin.None));
                Options.Target = secondDirectory;

                if (ConflictResolution == SyncConflictResolution.FirstWins)
                {
                    ConflictResolution = SyncConflictResolution.SecondWins;
                }
                else if (ConflictResolution == SyncConflictResolution.SecondWins)
                {
                    ConflictResolution = SyncConflictResolution.FirstWins;
                }

                base.ExecuteDirectory(directoryPath, context);
            }

            _ignoredPaths = null;
        }

        protected override DialogResult? ExecuteOperation(
            SearchContext context,
            string sourcePath,
            string destinationPath,
            bool isDirectory,
            string indent)
        {
            if (_ignoredPaths?.Contains(sourcePath) == true)
                return null;

            string? renamePath = null;

            ExecuteOperation();

            _destinationPaths?.Add(renamePath ?? destinationPath);

            return null;

            void ExecuteOperation()
            {
                bool fileExists = File.Exists(destinationPath);
                bool directoryExists = !fileExists && Directory.Exists(destinationPath);
                bool? preferLeft = null;
                FileCompareOptions? diffProperty = null;

                if (isDirectory)
                {
                    if (directoryExists)
                    {
                        Debug.Assert(!_isSecondToFirst);

                        if (_isSecondToFirst)
                            return;

                        if (FileSystemHelpers.AttributeEquals(sourcePath, destinationPath, Options.NoCompareAttributes))
                            return;
                    }
                }
                else if (fileExists)
                {
                    if (_isSecondToFirst)
                        return;

                    if ((Options.CompareOptions & FileCompareOptions.ModifiedTime) != 0)
                    {
                        int diff = FileSystemHelpers.CompareLastWriteTimeUtc(
                            sourcePath,
                            destinationPath,
                            Options.AllowedTimeDiff);

                        if (diff > 0)
                        {
                            preferLeft = true;
                        }
                        else if (diff < 0)
                        {
                            preferLeft = false;
                        }
                    }
                }

                if (preferLeft == null)
                {
                    if (!_isSecondToFirst
                        && !isDirectory)
                    {
                        if (fileExists)
                        {
                            if (Options.CompareOptions != FileCompareOptions.None)
                            {
                                diffProperty = FileSystemHelpers.CompareFiles(
                                    sourcePath,
                                    destinationPath,
                                    Options.CompareOptions,
                                    Options.NoCompareAttributes,
                                    Options.AllowedTimeDiff);

                                if (diffProperty == FileCompareOptions.None)
                                    return;
                            }
                        }
                        else if (Options.DetectRename
                            && (Options.CompareOptions & FileCompareOptions.Content) != 0)
                        {
                            if (_directoryData == null)
                            {
                                _directoryData = new DirectoryData();
                                _directoryData.Load(Path.GetDirectoryName(destinationPath)!);
                            }
                            else if (!FileSystemHelpers.IsParentDirectory(_directoryData.Path!, destinationPath))
                            {
                                _directoryData.Load(Path.GetDirectoryName(destinationPath)!);
                            }

                            if (_directoryData.Files.Count > 0)
                                renamePath = _directoryData.FindRenamedFile(sourcePath);
                        }
                    }

                    if (ConflictResolution == SyncConflictResolution.Ask)
                    {
                        if (base.CanEndProgress)
                            EndProgress(context);

                        if (renamePath != null)
                        {
                            WritePathPrefix(sourcePath, "FIL", default, indent);
                            WritePathPrefix(renamePath, "FIL", default, indent);
                        }
                        else
                        {
                            string firstPrefix = GetPrefix(invert: _isSecondToFirst);
                            string secondPrefix = GetPrefix(invert: !_isSecondToFirst);

                            WritePathPrefix((_isSecondToFirst) ? destinationPath : sourcePath, firstPrefix, default, indent);
                            WritePathPrefix((_isSecondToFirst) ? sourcePath : destinationPath, secondPrefix, default, indent);
                        }

                        DialogResult dialogResult = ConsoleHelpers.Ask("Prefer first directory?", indent);

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
                                    ConflictResolution = SyncConflictResolution.FirstWins;
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
                                    ConflictResolution = SyncConflictResolution.SecondWins;
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

                        if (_isSecondToFirst)
                        {
                            preferLeft = !preferLeft;
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(destinationPath)!;

                            CreateDirectory(directoryPath);
                        }
                    }
                    else if (ConflictResolution == SyncConflictResolution.FirstWins)
                    {
                        preferLeft = true;
                    }
                    else if (ConflictResolution == SyncConflictResolution.SecondWins)
                    {
                        preferLeft = false;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown enum value '{ConflictResolution}'.");
                    }
                }
                else if (Options.AskMode == AskMode.File
                    && !AskToExecute(context, GetQuestionText(isDirectory), indent))
                {
                    return;
                }

                preferLeft ??= !_isSecondToFirst;

                if (isDirectory)
                {
                    ExecuteDirectoryOperations(context, sourcePath, destinationPath, fileExists, directoryExists, preferLeft.Value, indent);
                }
                else if (renamePath != null)
                {
                    RenameFile(context, (preferLeft.Value) ? sourcePath : renamePath, (preferLeft.Value) ? renamePath : sourcePath, indent);
                }
                else
                {
                    ExecuteFileOperations(
                        context,
                        sourcePath,
                        destinationPath,
                        fileExists,
                        directoryExists,
                        preferLeft.Value,
                        diffProperty,
                        indent);
                }

                string GetPrefix(bool invert)
                {
                    if (invert)
                    {
                        if (isDirectory)
                        {
                            return (directoryExists) ? "DIR" : "---";
                        }
                        else
                        {
                            return (fileExists) ? "FIL" : "---";
                        }
                    }
                    else
                    {
                        return (isDirectory) ? "DIR" : "FIL";
                    }
                }
            }
        }

        private void ExecuteDirectoryOperations(
            SearchContext context,
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
                    WritePath(context, destinationPath, OperationKind.Update, indent);
                    UpdateAttributes(sourcePath, destinationPath);
                    context.Telemetry.UpdatedCount++;
                }
                else
                {
                    if (fileExists)
                    {
                        // create directory (overwrite existing file)
                        WritePath(context, destinationPath, OperationKind.Delete, indent);
                        DeleteFile(destinationPath);
                        context.Telemetry.DeletedCount++;
                    }
                    else
                    {
                        // create directory
                    }

                    WritePath(context, destinationPath, OperationKind.Add, indent);
                    CreateDirectory(destinationPath);
                    context.Telemetry.AddedCount++;
                }
            }
            else if (directoryExists)
            {
                // update directory's attributes
                WritePath(context, sourcePath, OperationKind.Update, indent);
                UpdateAttributes(destinationPath, sourcePath);
                context.Telemetry.UpdatedCount++;
            }
            else
            {
                WritePath(context, sourcePath, OperationKind.Delete, indent);
                DeleteDirectory(sourcePath);
                context.Telemetry.DeletedCount++;

                if (fileExists)
                {
                    // copy file (and overwrite existing directory)
                    WritePath(context, sourcePath, OperationKind.Add, indent);
                    CopyFile(destinationPath, sourcePath);
                    context.Telemetry.AddedCount++;
                }
                else
                {
                    // delete directory
                }
            }
        }

        private void ExecuteFileOperations(
            SearchContext context,
            string sourcePath,
            string destinationPath,
            bool fileExists,
            bool directoryExists,
            bool preferLeft,
            FileCompareOptions? diffProperty,
            string indent)
        {
            if (preferLeft)
            {
                if (fileExists)
                {
                    WritePath(context, destinationPath, OperationKind.Update, indent);

                    if (diffProperty == FileCompareOptions.Attributes)
                    {
                        // update attributes
                        File.SetAttributes(destinationPath, File.GetAttributes(sourcePath));
                    }
                    else
                    {
                        // copy file (and overwrite existing file)
                        DeleteFile(destinationPath);
                    }
                }
                else if (directoryExists)
                {
                    // copy file (and overwrite existing directory)
                    WritePath(context, destinationPath, OperationKind.Delete, indent);
                    DeleteDirectory(destinationPath);
                    context.Telemetry.DeletedCount++;
                }
                else
                {
                    // copy file
                }

                if (!fileExists)
                    WritePath(context, destinationPath, OperationKind.Add, indent);

                if (!fileExists
                    || diffProperty != FileCompareOptions.Attributes)
                {
                    CopyFile(sourcePath, destinationPath);
                }

                if (fileExists)
                {
                    context.Telemetry.UpdatedCount++;
                }
                else
                {
                    context.Telemetry.AddedCount++;
                }
            }
            else
            {
                WritePath(context, sourcePath, (fileExists) ? OperationKind.Update : OperationKind.Delete, indent);

                if (!fileExists
                    || diffProperty != FileCompareOptions.Attributes)
                {
                    DeleteFile(sourcePath);

                    if (!fileExists)
                        context.Telemetry.DeletedCount++;
                }

                if (fileExists)
                {
                    if (diffProperty == FileCompareOptions.Attributes)
                    {
                        // update attributes
                        File.SetAttributes(sourcePath, File.GetAttributes(destinationPath));
                    }
                    else
                    {
                        // copy file (and overwrite existing file)
                        CopyFile(destinationPath, sourcePath);
                    }

                    context.Telemetry.UpdatedCount++;
                }
                else if (directoryExists)
                {
                    // create directory (and overwrite existing file)
                    WritePath(context, sourcePath, OperationKind.Add, indent);
                    CreateDirectory(sourcePath);
                    context.Telemetry.AddedCount++;
                }
                else
                {
                    // delete file
                }
            }
        }

        private void RenameFile(
            SearchContext context,
            string sourcePath,
            string destinationPath,
            string indent)
        {
            string renamePath = Path.Combine(Path.GetDirectoryName(destinationPath)!, Path.GetFileName(sourcePath));

            WritePath(context, destinationPath, OperationKind.Rename, indent);
            WritePath(context, renamePath, OperationKind.None, indent);
            DeleteFile(destinationPath);
            CopyFile(sourcePath, renamePath);

            context.Telemetry.RenamedCount++;
        }

        private void DeleteDirectory(string path)
        {
            if (!DryRun)
                Directory.Delete(path, recursive: true);

            OnDirectoryChanged(new DirectoryChangedEventArgs(path, newName: null));
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
            throw new NotSupportedException();
        }

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            throw new InvalidOperationException("File cannot be synchronized.");
        }

        private void WritePath(SearchContext context, string path, OperationKind kind, string indent)
        {
            if (!ShouldLog(Verbosity.Minimal))
                return;

            if (base.CanEndProgress)
                EndProgress(context);

            switch (kind)
            {
                case OperationKind.None:
                    {
                        WritePathPrefix(path, "   ", default, indent);
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
                case OperationKind.Rename:
                    {
                        WritePathPrefix(path, "REN", Colors.Sync_Rename, indent);
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
            if (ShouldLog(Verbosity.Minimal))
            {
                Write(indent, Verbosity.Minimal);
                Write(prefix, colors, Verbosity.Minimal);
                Write(" ", Verbosity.Minimal);
                WriteLine(path, verbosity: Verbosity.Minimal);
            }
        }

        protected override void WriteError(SearchContext context, Exception ex, string path, string indent)
        {
            if (base.CanEndProgress)
                EndProgress(context);

            Write(indent, Verbosity.Minimal);
            Write("ERR", Colors.Sync_Error, Verbosity.Minimal);
            Write(" ", Verbosity.Minimal);
            Write(ex.Message, verbosity: Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);
#if DEBUG
            WriteLine($"{indent}PATH: {path}");
            WriteLine($"{indent}STACK TRACE:");
            WriteLine(ex.StackTrace);
#endif
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            base.WriteSummary(telemetry, verbosity);

            ConsoleColors colors = (DryRun) ? Colors.Message_DryRun : default;

            WriteCount("Added", telemetry.AddedCount, colors, verbosity: verbosity);
            Write("  ", verbosity);
            WriteCount("Updated", telemetry.UpdatedCount, colors, verbosity: verbosity);
            Write("  ", verbosity);
            WriteCount("Renamed", telemetry.RenamedCount, colors, verbosity: verbosity);
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
            Rename,
            Delete,
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        private sealed class DirectoryData
        {
            public string? Path { get; private set; }

            public List<FileData> Files { get; } = new List<FileData>();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplay => $"{Path}  Files = {Files.Count}";

            public void Load(string path)
            {
                Path = path;

                Files.Clear();

                if (Directory.Exists(path))
                    Files.AddRange(Directory.EnumerateFiles(path).Select(f => new FileData(f, File.GetLastWriteTimeUtc(f))));
            }

            public string? FindRenamedFile(string filePath)
            {
                string? renameFile = null;

                DateTime modifiedTime = File.GetLastWriteTimeUtc(filePath);

                long size = -1;

                FileStream? fs = null;

                try
                {
                    foreach (FileData fileData in Files)
                    {
                        if (modifiedTime == fileData.ModifiedTime)
                        {
                            if (size == -1)
                            {
                                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                                size = fs.Length;
                            }

                            using (var fs2 = new FileStream(fileData.Path, FileMode.Open, FileAccess.Read))
                            {
                                if (fs!.Length == fs2.Length)
                                {
                                    if (fs.Position != 0)
                                        fs.Position = 0;

                                    if (StreamComparer.Default.ByteEquals(fs, fs2))
                                    {
                                        if (renameFile == null)
                                        {
                                            renameFile = fileData.Path;
                                        }
                                        else
                                        {
                                            return null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    fs?.Dispose();
                }

                return renameFile;
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        private readonly struct FileData
        {
            public FileData(string path, DateTime modifiedTime)
            {
                Path = path;
                ModifiedTime = modifiedTime;
            }

            public string Path { get; }

            public DateTime ModifiedTime { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplay => $"{ModifiedTime}  {Path}";
        }
    }
}
