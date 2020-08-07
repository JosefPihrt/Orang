// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Orang.FileSystem;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal abstract class FileSystemCommand<TOptions> : AbstractCommand<TOptions> where TOptions : FileSystemCommandOptions
    {
        protected FileSystemCommand(TOptions options) : base(options)
        {
        }

        private FileSystemSearch? _search;
        private ProgressReporter? _progressReporter;

        protected FileSystemSearch Search => _search ?? (_search = CreateSearch());

        private ProgressReporter? ProgressReporter => _progressReporter ?? (_progressReporter = CreateProgressReporter());

        public Filter? NameFilter => Options.NameFilter;

        protected virtual bool CanDisplaySummary => true;

        public virtual bool CanEndProgress => !Options.OmitPath;

        public virtual bool CanUseResults => true;

        protected virtual FileSystemSearch CreateSearch()
        {
            var filter = new FileSystemFilter(
                name: Options.NameFilter,
                part: Options.NamePart,
                extension: Options.ExtensionFilter,
                content: Options.ContentFilter,
                properties: Options.FilePropertyFilter,
                attributes: Options.Attributes,
                attributesToSkip: Options.AttributesToSkip,
                emptyOption: Options.EmptyOption);

            NameFilter? directoryFilter = null;

            if (Options.DirectoryFilter != null)
            {
                directoryFilter = new NameFilter(
                    name: Options.DirectoryFilter,
                    part: Options.DirectoryNamePart);
            }

            var options = new FileSystemSearchOptions(
                searchTarget: Options.SearchTarget,
                recurseSubdirectories: Options.RecurseSubdirectories,
                defaultEncoding: Options.DefaultEncoding);

            return new FileSystemSearch(
                filter: filter,
                directoryFilter: directoryFilter,
                searchProgress: ProgressReporter,
                options: options);
        }

        protected abstract void ExecuteDirectory(string directoryPath, SearchContext context);

        protected abstract void ExecuteFile(string filePath, SearchContext context);

        protected abstract void ExecuteMatchCore(FileMatch fileMatch, SearchContext context, string? baseDirectoryPath, ColumnWidths? columnWidths);

        protected abstract void ExecuteResult(SearchResult result, SearchContext context, ColumnWidths? columnWidths);

        protected abstract void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity);

        protected sealed override CommandResult ExecuteCore(CancellationToken cancellationToken = default)
        {
            List<SearchResult>? results = null;

            if (CanUseResults)
            {
                if (Options.SortOptions != null || Options.Format.FileProperties.Any())
                {
                    results = new List<SearchResult>();
                }
            }

            var context = new SearchContext(new SearchTelemetry(), progress: ProgressReporter, results: results, cancellationToken: cancellationToken);

            ExecuteCore(context);

            if (context.TerminationReason == TerminationReason.Canceled)
                return CommandResult.Canceled;

            return (context.Telemetry.MatchingFileCount > 0) ? CommandResult.Success : CommandResult.NoMatch;
        }

        private ProgressReporter? CreateProgressReporter()
        {
            ProgressReportMode consoleReportMode;
            if (ConsoleOut.ShouldWrite(Verbosity.Diagnostic))
            {
                consoleReportMode = ProgressReportMode.Path;
            }
            else if (Options.Progress)
            {
                consoleReportMode = ProgressReportMode.Dot;
            }
            else
            {
                consoleReportMode = ProgressReportMode.None;
            }

            ProgressReportMode fileReportMode;
            if (Out?.ShouldWrite(Verbosity.Diagnostic) == true)
            {
                fileReportMode = ProgressReportMode.Path;
            }
            else
            {
                fileReportMode = ProgressReportMode.None;
            }

            if (fileReportMode == ProgressReportMode.None)
            {
                if (consoleReportMode == ProgressReportMode.None)
                {
                    return (ShouldWriteSummary()) ? new ProgressReporter(GetPathIndent()) : null;
                }
                else if (consoleReportMode == ProgressReportMode.Dot)
                {
                    return new DotProgressReporter(GetPathIndent());
                }
            }

            return new DiagnosticProgressReporter(consoleReportMode, fileReportMode, Options, GetPathIndent());
        }

        protected virtual void ExecuteCore(SearchContext context)
        {
            bool canceled = false;
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (PathInfo pathInfo in Options.Paths)
            {
                ExecuteCore(pathInfo.Path, context);

                if (context.TerminationReason == TerminationReason.MaxReached)
                    break;

                if (context.TerminationReason == TerminationReason.Canceled
                    || context.CancellationToken.IsCancellationRequested)
                {
                    canceled = true;
                    OperationCanceled();
                    break;
                }
            }

            if (context.Results != null)
            {
                if (context.Progress?.ProgressReported == true
                    && ConsoleOut.Verbosity >= Verbosity.Minimal)
                {
                    ConsoleOut.WriteLine();
                    context.Progress.ProgressReported = false;
                }

                if (!canceled
                    && context.Results.Count > 0)
                {
                    ExecuteResults(context);
                }
            }

            stopwatch.Stop();

            if (ShouldWriteSummary())
            {
                if (context.Progress != null)
                {
                    context.Telemetry.SearchedDirectoryCount = context.Progress.SearchedDirectoryCount;
                    context.Telemetry.FileCount = context.Progress.FileCount;
                    context.Telemetry.DirectoryCount = context.Progress.DirectoryCount;
                }

                context.Telemetry.Elapsed = stopwatch.Elapsed;

                WriteSummary(context.Telemetry, (Options.IncludeSummary) ? Verbosity.Quiet : Verbosity.Detailed);
            }
        }

        private bool ShouldWriteSummary()
        {
            if (CanDisplaySummary)
            {
                if (ShouldLog(Verbosity.Detailed)
                    || Options.IncludeSummary)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExecuteResults(SearchContext context)
        {
            IEnumerable<SearchResult> results = context.Results!;
            SortOptions? sortOptions = Options.SortOptions;

            if (sortOptions?.Descriptors.Any() == true)
            {
                PathDisplayStyle pathDisplayStyle = Options.PathDisplayStyle;

                if (pathDisplayStyle == PathDisplayStyle.Match
                    && NameFilter == null)
                {
                    pathDisplayStyle = PathDisplayStyle.Full;
                }

                results = SortHelpers.SortResults(context.Results!, sortOptions.Descriptors, pathDisplayStyle);

                if (sortOptions.MaxCount > 0)
                    results = results.Take(sortOptions.MaxCount);
            }

            ImmutableArray<FileProperty> fileProperties = Options.Format.FileProperties;
            ColumnWidths? columnWidths = null;

            if (fileProperties.Any())
            {
                List<SearchResult> resultList = results.ToList();

                int maxNameWidth = resultList.Max(f => f.Path.Length);
                int maxSizeWidth = 0;

                if (fileProperties.Contains(FileProperty.Size))
                {
                    long maxSize = 0;

                    foreach (SearchResult result in resultList)
                    {
                        long size = result.GetSize();

                        if (result.IsDirectory)
                        {
                            if (context.DirectorySizeMap == null)
                                context.DirectorySizeMap = new Dictionary<string, long>();

                            context.DirectorySizeMap[result.Path] = size;
                        }

                        if (size > maxSize)
                            maxSize = size;
                    }

                    maxSizeWidth = maxSize.ToString("n0").Length;
                }

                columnWidths = new ColumnWidths(maxNameWidth, maxSizeWidth);

                results = resultList;
            }

            int i = 0;

            try
            {
                foreach (SearchResult result in results)
                {
                    ExecuteResult(result, context, columnWidths);
                    i++;

                    if (context.TerminationReason == TerminationReason.Canceled)
                        break;

                    context.CancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                context.TerminationReason = TerminationReason.Canceled;
            }

            if (context.TerminationReason == TerminationReason.Canceled
                || context.CancellationToken.IsCancellationRequested)
            {
                OperationCanceled();
            }

            if (Options.Format.FileProperties.Contains(FileProperty.Size)
                && context.Telemetry.FilesTotalSize == 0)
            {
                foreach (SearchResult result in results.Take(i))
                    context.Telemetry.FilesTotalSize += result.GetSize();
            }
        }

        private void ExecuteCore(string path, SearchContext context)
        {
            if (Directory.Exists(path))
            {
                ProgressReporter? progress = context.Progress;

                progress?.SetBaseDirectoryPath(path);

                if (Options.DisplayRelativePath
                    && Options.IncludeBaseDirectory)
                {
                    WriteLine(path, Colors.BasePath, Verbosity.Minimal);
                }

                try
                {
                    ExecuteDirectory(path, context);
                }
                catch (OperationCanceledException)
                {
                    context.TerminationReason = TerminationReason.Canceled;
                }

                if (progress?.ProgressReported == true)
                {
                    ConsoleOut.WriteLine();
                    progress.ProgressReported = false;
                }

                progress?.SetBaseDirectoryPath(null);
            }
            else if (File.Exists(path))
            {
                try
                {
                    ExecuteFile(path, context);
                }
                catch (OperationCanceledException)
                {
                    context.TerminationReason = TerminationReason.Canceled;
                }
            }
            else
            {
                string message = $"File or directory not found: {path}";

                WriteLine(message, Colors.Message_Warning, Verbosity.Minimal);
            }
        }

        protected void EndProgress(SearchContext context)
        {
            if (context.Progress?.ProgressReported == true
                && ConsoleOut.Verbosity >= Verbosity.Minimal
                && context.Results == null
                && CanEndProgress)
            {
                ConsoleOut.WriteLine();
                context.Progress.ProgressReported = false;
            }
        }

        protected void ExecuteMatch(FileMatch fileMatch, SearchContext context, string? baseDirectoryPath = null)
        {
            if (fileMatch.IsDirectory)
            {
                context.Telemetry.MatchingDirectoryCount++;
            }
            else
            {
                context.Telemetry.MatchingFileCount++;
            }

            if (Options.MaxMatchingFiles == context.Telemetry.MatchingFileDirectoryCount)
                context.TerminationReason = TerminationReason.MaxReached;

            if (context.Results != null)
            {
                var searchResult = new SearchResult(fileMatch, baseDirectoryPath);

                context.Results.Add(searchResult);
            }
            else
            {
                EndProgress(context);

                ExecuteMatchCore(fileMatch, context, baseDirectoryPath, columnWidths: null);
            }
        }

        protected IEnumerable<FileMatch> GetMatches(
            string directoryPath,
            SearchContext context)
        {
            return GetMatches(
                directoryPath: directoryPath,
                context: context,
                notifyDirectoryChanged: default(INotifyDirectoryChanged));
        }

        protected IEnumerable<FileMatch> GetMatches(
            string directoryPath,
            SearchContext context,
            INotifyDirectoryChanged? notifyDirectoryChanged)
        {
            return Search.Find(
                directoryPath: directoryPath,
                notifyDirectoryChanged: notifyDirectoryChanged,
                cancellationToken: context.CancellationToken);
        }

        protected FileMatch? MatchFile(string filePath)
        {
            return Search.MatchFile(filePath);
        }

        protected string GetPathIndent(string? baseDirectoryPath)
        {
            return (baseDirectoryPath != null) ? GetPathIndent() : "";
        }

        private string GetPathIndent()
        {
            return (Options.DisplayRelativePath && Options.IncludeBaseDirectory)
                ? Options.Indent
                : "";
        }

        protected virtual void WritePath(SearchContext context, FileMatch fileMatch, string? baseDirectoryPath, string indent, ColumnWidths? columnWidths)
        {
            WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths, Colors.Match);

            WriteLine(Verbosity.Minimal);
        }

        protected void WritePath(SearchContext context, FileMatch fileMatch, string? baseDirectoryPath, string indent, ColumnWidths? columnWidths, ConsoleColors matchColors)
        {
            if (Options.PathDisplayStyle == PathDisplayStyle.Match
                && fileMatch.NameMatch != null
                && !object.ReferenceEquals(fileMatch.NameMatch, Match.Empty))
            {
                if (ShouldLog(Verbosity.Minimal))
                {
                    Write(indent, Verbosity.Minimal);
                    Write(fileMatch.NameMatch.Value, (Options.HighlightMatch) ? matchColors : default, Verbosity.Minimal);
                }
            }
            else
            {
                LogHelpers.WritePath(
                    fileMatch,
                    baseDirectoryPath,
                    relativePath: Options.DisplayRelativePath,
                    colors: Colors.Matched_Path,
                    matchColors: (Options.HighlightMatch) ? matchColors : default,
                    indent: indent,
                    verbosity: Verbosity.Minimal);
            }

            WriteProperties(context, fileMatch, columnWidths);
        }

        protected void WriteProperties(SearchContext context, FileMatch fileMatch, ColumnWidths? columnWidths)
        {
            if (!ShouldLog(Verbosity.Minimal))
                return;

            if (columnWidths == null
                && (CanUseResults || !Options.Format.FileProperties.Any()))
            {
                return;
            }

            StringBuilder sb = StringBuilderCache.GetInstance();

            if (columnWidths != null)
                sb.Append(' ', columnWidths.NameWidth - fileMatch.Path.Length);

            foreach (FileProperty fileProperty in Options.Format.FileProperties)
            {
                if (fileProperty == FileProperty.Size)
                {
                    sb.Append("  ");

                    long size = (fileMatch.IsDirectory)
                        ? (context.DirectorySizeMap?[fileMatch.Path] ?? FileSystemHelpers.GetDirectorySize(fileMatch.Path))
                        : new FileInfo(fileMatch.Path).Length;

                    string sizeText = size.ToString("n0");

                    if (columnWidths != null)
                        sb.Append(' ', columnWidths.SizeWidth - sizeText.Length);

                    sb.Append(sizeText);

                    context.Telemetry.FilesTotalSize += size;
                }
                else if (fileProperty == FileProperty.CreationTime)
                {
                    sb.Append("  ");
                    sb.Append(File.GetCreationTime(fileMatch.Path).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (fileProperty == FileProperty.ModifiedTime)
                {
                    sb.Append("  ");
                    sb.Append(File.GetLastWriteTime(fileMatch.Path).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    throw new InvalidOperationException($"Unknown enum value '{fileProperty}'.");
                }
            }

            Write(StringBuilderCache.GetStringAndFree(sb), Verbosity.Minimal);
        }
    }
}
