// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Orang.Operations;
using static Orang.FileSystem.FileSystemHelpers;

#pragma warning disable RCS1223

namespace Orang.FileSystem
{
    //TODO: FileSearch, Search
    public partial class FileSystemSearch
    {
        public FileSystemSearch(
            FileSystemFilter filter,
            NameFilter? directoryFilter = null,
            IProgress<SearchProgress>? searchProgress = null,
            FileSystemSearchOptions? options = null)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));

            if (directoryFilter?.Part == FileNamePart.Extension)
                throw new ArgumentException($"Directory filter has invalid part '{FileNamePart.Extension}'.", nameof(directoryFilter));

            DirectoryFilter = directoryFilter;
            SearchProgress = searchProgress;
            Options = options ?? FileSystemSearchOptions.Default;
        }

        public FileSystemFilter Filter { get; }

        public NameFilter? DirectoryFilter { get; }

        public FileSystemSearchOptions Options { get; }

        public IProgress<SearchProgress>? SearchProgress { get; }

        internal bool DisallowEnumeration { get; set; }

        internal bool MatchPartOnly { get; set; }

        internal bool CanRecurseMatch { get; set; } = true;

        private FileNamePart Part => Filter.Part;

        private Filter? Name => Filter.Name;

        private Filter? Extension => Filter.Extension;

        private Filter? Content => Filter.Content;

        private FilePropertyFilter? Properties => Filter.Properties;

        private FileAttributes Attributes => Filter.Attributes;

        private FileAttributes AttributesToSkip => Filter.AttributesToSkip;

        private FileEmptyOption EmptyOption => Filter.EmptyOption;

        private SearchTarget SearchTarget => Options.SearchTarget;

        private bool RecurseSubdirectories => Options.RecurseSubdirectories;

        public IEnumerable<FileMatch> Find(
            string directoryPath,
            CancellationToken cancellationToken = default)
        {
            return Find(directoryPath, default(INotifyDirectoryChanged), cancellationToken);
        }

        internal IEnumerable<FileMatch> Find(
            string directoryPath,
            INotifyDirectoryChanged? notifyDirectoryChanged = null,
            CancellationToken cancellationToken = default)
        {
            var enumerationOptions = new EnumerationOptions()
            {
                AttributesToSkip = AttributesToSkip,
                IgnoreInaccessible = Options.IgnoreInaccessible,
                MatchCasing = MatchCasing.PlatformDefault,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = false,
                ReturnSpecialDirectories = false
            };

            var directories = new Queue<Directory>();
            Queue<Directory>? subdirectories = (RecurseSubdirectories) ? new Queue<Directory>() : null;

            string? currentDirectory = null;

            if (notifyDirectoryChanged != null)
            {
                notifyDirectoryChanged.DirectoryChanged += (object sender, DirectoryChangedEventArgs e) => currentDirectory = e.NewName;
            }

            bool? isMatch = (DirectoryFilter?.IsNegative == false)
                ? IncludeDirectory(directoryPath)
                : default(bool?);

            var directory = new Directory(directoryPath, isMatch);

            while (true)
            {
                Report(directory.Path, SearchProgressKind.SearchDirectory, isDirectory: true);

                if (SearchTarget != SearchTarget.Directories
                    && directory.IsMatch != false)
                {
                    IEnumerator<string> fi = null!;

                    try
                    {
                        fi = (DisallowEnumeration)
                            ? ((IEnumerable<string>)GetFiles(directory.Path, enumerationOptions)).GetEnumerator()
                            : EnumerateFiles(directory.Path, enumerationOptions).GetEnumerator();
                    }
                    catch (Exception ex) when (IsWellKnownException(ex))
                    {
                        Debug.Fail(ex.ToString());
                        Report(directory.Path, SearchProgressKind.SearchDirectory, isDirectory: true, ex);
                    }

                    if (fi != null)
                    {
                        using (fi)
                        {
                            while (fi.MoveNext())
                            {
                                FileMatch? match = MatchFile(fi.Current);

                                if (match != null)
                                    yield return match;

                                cancellationToken.ThrowIfCancellationRequested();
                            }
                        }
                    }
                }

                IEnumerator<string> di = null!;

                try
                {
                    di = (DisallowEnumeration)
                        ? ((IEnumerable<string>)GetDirectories(directory.Path, enumerationOptions)).GetEnumerator()
                        : EnumerateDirectories(directory.Path, enumerationOptions).GetEnumerator();
                }
                catch (Exception ex) when (IsWellKnownException(ex))
                {
                    Debug.Fail(ex.ToString());
                    Report(directory.Path, SearchProgressKind.SearchDirectory, isDirectory: true, ex);
                }

                if (di != null)
                {
                    using (di)
                    {
                        while (di.MoveNext())
                        {
                            currentDirectory = di.Current;

                            isMatch = (DirectoryFilter?.IsNegative == false && directory.IsMatch == true)
                                ? true
                                : default(bool?);

                            if (directory.IsMatch != false
                                && SearchTarget != SearchTarget.Files
                                && Part != FileNamePart.Extension)
                            {
                                if (isMatch == null
                                    && DirectoryFilter != null)
                                {
                                    isMatch = IncludeDirectory(currentDirectory);
                                }

                                if (isMatch != false)
                                {
                                    FileMatch? match = MatchDirectory(currentDirectory);

                                    if (match != null)
                                    {
                                        yield return match;

                                        if (!CanRecurseMatch)
                                            currentDirectory = null;
                                    }
                                }
                            }

                            if (currentDirectory != null
                                && RecurseSubdirectories)
                            {
                                if (isMatch == null
                                    && DirectoryFilter != null)
                                {
                                    isMatch = IncludeDirectory(currentDirectory);
                                }

                                if (isMatch != false
                                    || !DirectoryFilter!.IsNegative)
                                {
                                    subdirectories!.Enqueue(new Directory(currentDirectory, isMatch));
                                }
                            }

                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }

                    if (RecurseSubdirectories)
                    {
                        while (subdirectories!.Count > 0)
                            directories.Enqueue(subdirectories.Dequeue());
                    }
                }

                if (directories.Count > 0)
                {
                    directory = directories.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        public FileMatch? MatchFile(string path)
        {
            (FileMatch? fileMatch, Exception? exception) = MatchFileImpl(path);

            Report(path, SearchProgressKind.File, exception: exception);

            return fileMatch;
        }

        private (FileMatch? fileMatch, Exception? exception) MatchFileImpl(string path)
        {
            if (Extension?.IsMatch(FileNameSpan.FromFile(path, FileNamePart.Extension)) == false)
                return default;

            FileNameSpan span = FileNameSpan.FromFile(path, Part);
            Match? match = null;

            if (Name != null)
            {
                match = Name.Match(span, MatchPartOnly);

                if (match == null)
                    return default;
            }

            FileEmptyOption emptyOption = EmptyOption;
            bool isEmpty = false;
            FileInfo? fileInfo = null;

            if (Attributes != 0
                || emptyOption != FileEmptyOption.None
                || Properties != null)
            {
                try
                {
                    fileInfo = new FileInfo(path);

                    if (Attributes != 0
                        && (fileInfo.Attributes & Attributes) != Attributes)
                    {
                        return default;
                    }

                    if (Properties != null)
                    {
                        if (Properties.SizePredicate?.Invoke(fileInfo.Length) == false)
                            return default;

                        if (Properties.CreationTimePredicate?.Invoke(fileInfo.CreationTime) == false)
                            return default;

                        if (Properties.ModifiedTimePredicate?.Invoke(fileInfo.LastWriteTime) == false)
                            return default;
                    }

                    if (emptyOption != FileEmptyOption.None)
                    {
                        if (fileInfo.Length == 0)
                        {
                            if (emptyOption == FileEmptyOption.NonEmpty)
                                return default;

                            isEmpty = true;
                        }
                        else if (fileInfo.Length > 4)
                        {
                            if (emptyOption == FileEmptyOption.Empty)
                                return default;

                            emptyOption = FileEmptyOption.None;
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    return (null, ex);
                }
            }

            if (Content != null
                || emptyOption != FileEmptyOption.None)
            {
                FileContent fileContent = default;

                if (isEmpty)
                {
                    fileContent = new FileContent("", Options.DefaultEncoding, hasBom: false);
                }
                else
                {
                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            Encoding? bomEncoding = DetectEncoding(stream);

                            if (emptyOption != FileEmptyOption.None)
                            {
                                if (bomEncoding?.Preamble.Length == stream.Length)
                                {
                                    if (emptyOption == FileEmptyOption.NonEmpty)
                                        return default;
                                }
                                else if (emptyOption == FileEmptyOption.Empty)
                                {
                                    return default;
                                }
                            }

                            if (Content != null)
                            {
                                using (var reader = new StreamReader(
                                    stream: stream,
                                    encoding: bomEncoding ?? Options.DefaultEncoding,
                                    detectEncodingFromByteOrderMarks: bomEncoding == null))
                                {
                                    string content = reader.ReadToEnd();

                                    fileContent = new FileContent(content, reader.CurrentEncoding, hasBom: bomEncoding != null);
                                }
                            }
                        }
                    }
                    catch (Exception ex) when (ex is IOException
                        || ex is UnauthorizedAccessException)
                    {
                        return (null, ex);
                    }
                }

                if (Content != null)
                {
                    Match? contentMatch = Content.Match(fileContent.Text);

                    if (contentMatch == null)
                        return default;

                    return (new FileMatch(span, match, fileContent, contentMatch, fileInfo), null);
                }
            }

            return (new FileMatch(span, match, fileInfo), null);
        }

        public FileMatch? MatchDirectory(string path)
        {
            (FileMatch? fileMatch, Exception? exception) = MatchDirectoryImpl(path);

            Report(path, SearchProgressKind.Directory, isDirectory: true, exception);

            return fileMatch;
        }

        private (FileMatch? fileMatch, Exception? exception) MatchDirectoryImpl(string path)
        {
            FileNameSpan span = FileNameSpan.FromDirectory(path, Part);
            Match? match = null;

            if (Name != null)
            {
                match = Name.Match(span, MatchPartOnly);

                if (match == null)
                    return default;
            }

            DirectoryInfo? directoryInfo = null;

            if (Attributes != 0
                || EmptyOption != FileEmptyOption.None
                || Properties != null)
            {
                try
                {
                    directoryInfo = new DirectoryInfo(path);

                    if (Attributes != 0
                        && (directoryInfo.Attributes & Attributes) != Attributes)
                    {
                        return default;
                    }

                    if (Properties != null)
                    {
                        if (Properties.CreationTimePredicate?.Invoke(directoryInfo.CreationTime) == false)
                            return default;

                        if (Properties.ModifiedTimePredicate?.Invoke(directoryInfo.LastWriteTime) == false)
                            return default;
                    }

                    if (EmptyOption == FileEmptyOption.Empty)
                    {
                        if (!IsEmptyDirectory(path))
                            return default;
                    }
                    else if (EmptyOption == FileEmptyOption.NonEmpty)
                    {
                        if (IsEmptyDirectory(path))
                            return default;
                    }
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    return (null, ex);
                }
            }

            return (new FileMatch(span, match, directoryInfo, isDirectory: true), null);
        }

        private bool IncludeDirectory(string path)
        {
            FileNameSpan name = FileNameSpan.FromDirectory(path, DirectoryFilter!.Part);

            return DirectoryFilter.Name.IsMatch(name);
        }

        private void Report(string path, SearchProgressKind kind, bool isDirectory = false, Exception? exception = null)
        {
            SearchProgress?.Report(path, kind, isDirectory: isDirectory, exception);
        }

        private static bool IsWellKnownException(Exception ex)
        {
            return ex is IOException
                || ex is ArgumentException
                || ex is UnauthorizedAccessException
                || ex is SecurityException
                || ex is NotSupportedException;
        }

        public void Replace(
            string directoryPath,
            ReplaceOptions? replaceOptions = null,
            IProgress<OperationProgress>? progress = null,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (Filter.Content == null)
                throw new InvalidOperationException("Content filter is not defined.");

            if (Filter.Content.IsNegative)
                throw new InvalidOperationException("Content filter cannot be negative.");

            var command = new ReplaceOperation()
            {
                Search = this,
                ReplaceOptions = replaceOptions ?? ReplaceOptions.Empty,
                Progress = progress,
                DryRun = dryRun,
                CancellationToken = cancellationToken,
                MaxMatchingFiles = 0,
                MaxMatchesInFile = 0,
                MaxTotalMatches = 0,
            };

            command.Execute(directoryPath);
        }

        public void Delete(
            string directoryPath,
            DeleteOptions? deleteOptions = null,
            IProgress<OperationProgress>? progress = null,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            var command = new DeleteOperation()
            {
                Search = this,
                DeleteOptions = deleteOptions ?? DeleteOptions.Default,
                Progress = progress,
                DryRun = dryRun,
                CancellationToken = cancellationToken,
                MaxMatchingFiles = 0,
            };

            bool canRecurseMatch = CanRecurseMatch;

            try
            {
                CanRecurseMatch = false;

                command.Execute(directoryPath);
            }
            finally
            {
                CanRecurseMatch = canRecurseMatch;
            }
        }

        public void Rename(
            string directoryPath,
            RenameOptions renameOptions,
            IDialogProvider<OperationProgress>? dialogProvider = null,
            IProgress<OperationProgress>? progress = null,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (renameOptions == null)
                throw new ArgumentNullException(nameof(renameOptions));

            if (Part == FileNamePart.FullName)
                throw new InvalidOperationException($"Invalid file name part '{nameof(FileNamePart.FullName)}'.");

            if (Filter.Name == null)
                throw new InvalidOperationException("Name filter is not defined.");

            if (Filter.Name.IsNegative)
                throw new InvalidOperationException("Name filter cannot be negative.");

            VerifyConflictResolution(renameOptions.ConflictResolution, dialogProvider);

            var command = new RenameOperation()
            {
                Search = this,
                RenameOptions = renameOptions,
                Progress = progress,
                DryRun = dryRun,
                CancellationToken = cancellationToken,
                DialogProvider = dialogProvider,
                MaxMatchingFiles = 0,
            };

            bool disallowEnumeration = DisallowEnumeration;
            bool matchPartOnly = MatchPartOnly;

            try
            {
                DisallowEnumeration = !dryRun;
                MatchPartOnly = true;

                command.Execute(directoryPath);
            }
            finally
            {
                DisallowEnumeration = disallowEnumeration;
                MatchPartOnly = matchPartOnly;
            }
        }

        public void Copy(
            string directoryPath,
            string destinationPath,
            CopyOptions? copyOptions = null,
            IDialogProvider<OperationProgress>? dialogProvider = null,
            IProgress<OperationProgress>? progress = null,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            copyOptions ??= CopyOptions.Default;

            VerifyCopyMoveArguments(directoryPath, destinationPath, copyOptions, dialogProvider);

            var command = new CopyOperation()
            {
                Search = this,
                DestinationPath = destinationPath,
                CopyOptions = copyOptions,
                Progress = progress,
                DryRun = dryRun,
                CancellationToken = cancellationToken,
                DialogProvider = dialogProvider,
                MaxMatchingFiles = 0,
                MaxMatchesInFile = 0,
                MaxTotalMatches = 0,
                ConflictResolution = copyOptions.ConflictResolution,
            };

            command.Execute(directoryPath);
        }

        public void Move(
            string directoryPath,
            string destinationPath,
            CopyOptions? copyOptions = null,
            IDialogProvider<OperationProgress>? dialogProvider = null,
            IProgress<OperationProgress>? progress = null,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            copyOptions ??= CopyOptions.Default;

            VerifyCopyMoveArguments(directoryPath, destinationPath, copyOptions, dialogProvider);

            var command = new MoveOperation()
            {
                Search = this,
                DestinationPath = destinationPath,
                CopyOptions = copyOptions,
                Progress = progress,
                DryRun = dryRun,
                CancellationToken = cancellationToken,
                DialogProvider = dialogProvider,
                MaxMatchingFiles = 0,
                MaxMatchesInFile = 0,
                MaxTotalMatches = 0,
                ConflictResolution = copyOptions.ConflictResolution,
            };

            command.Execute(directoryPath);
        }

        private static void VerifyCopyMoveArguments(
            string directoryPath,
            string destinationPath,
            CopyOptions copyOptions,
            IDialogProvider<OperationProgress>? dialogProvider)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (destinationPath == null)
                throw new ArgumentNullException(nameof(destinationPath));

            if (!System.IO.Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            if (!System.IO.Directory.Exists(destinationPath))
                throw new DirectoryNotFoundException($"Directory not found: {destinationPath}");

            string sourcePathNormalized = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            string destinationPathNormalize = destinationPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (IsSubdirectory(sourcePathNormalized, destinationPathNormalize)
                || IsSubdirectory(destinationPathNormalize, sourcePathNormalized))
            {
                throw new ArgumentException("Source directory cannot be subdirectory of a destination directory or vice versa.", nameof(directoryPath));
            }

            VerifyConflictResolution(copyOptions.ConflictResolution, dialogProvider);
        }

        private static void VerifyConflictResolution(ConflictResolution conflictResolution, IDialogProvider<OperationProgress>? dialogProvider)
        {
            if (conflictResolution == ConflictResolution.Ask
                && dialogProvider == null)
            {
                throw new ArgumentNullException(nameof(dialogProvider), $"'{nameof(dialogProvider)}' cannot be null when {nameof(ConflictResolution)} is set to {nameof(ConflictResolution.Ask)}.");
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        private readonly struct Directory
        {
            public Directory(string path, bool? isMatch = null)
            {
                Path = path;
                IsMatch = isMatch;
            }

            public string Path { get; }

            public bool? IsMatch { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplay => Path;
        }
    }
}
