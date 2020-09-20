// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal abstract class CommonFindCommand<TOptions> : FileSystemCommand<TOptions> where TOptions : CommonFindCommandOptions
    {
        private OutputSymbols? _symbols;
        private ContentWriterOptions? _fileWriterOptions;
        private ContentWriterOptions? _directoryWriterOptions;

        protected CommonFindCommand(TOptions options) : base(options)
        {
        }

        public Filter? ContentFilter => Options.ContentFilter;

        private OutputSymbols Symbols => _symbols ??= OutputSymbols.Create(Options.HighlightOptions);

        protected AskMode AskMode { get; set; }

        public override bool CanEndProgress
        {
            get
            {
                return !Options.OmitPath
                    || (ContentFilter != null && ConsoleOut.Verbosity > Verbosity.Minimal);
            }
        }

        protected ContentWriterOptions FileWriterOptions
        {
            get
            {
                if (_fileWriterOptions == null)
                {
                    string indent = (Options.OmitPath) ? "" : Options.Indent;

                    _fileWriterOptions = CreateContentWriterOptions(indent);
                }

                return _fileWriterOptions;
            }
        }

        protected ContentWriterOptions DirectoryWriterOptions
        {
            get
            {
                if (_directoryWriterOptions == null)
                {
                    string indent = GetIndent();

                    _directoryWriterOptions = CreateContentWriterOptions(indent);
                }

                return _directoryWriterOptions;

                string GetIndent()
                {
                    return Options.PathDisplayStyle switch
                    {
                        PathDisplayStyle.Full => Options.Indent,
                        PathDisplayStyle.Relative => (Options.IncludeBaseDirectory) ? Options.DoubleIndent : Options.Indent,
                        PathDisplayStyle.Omit => "",
                        _ => throw new InvalidOperationException(),
                    };
                }
            }
        }

        protected virtual ContentWriterOptions CreateContentWriterOptions(string indent)
        {
            int groupNumber = ContentFilter!.GroupNumber;

            GroupDefinition? groupDefinition = (groupNumber >= 0)
                ? new GroupDefinition(groupNumber, ContentFilter.GroupName)
                : default(GroupDefinition?);

            return new ContentWriterOptions(
                format: Options.Format,
                groupDefinition,
                symbols: Symbols,
                highlightOptions: Options.HighlightOptions,
                indent: indent);
        }

        protected override void ExecuteCore(SearchContext context)
        {
            context.Telemetry.MatchingLineCount = -1;

            if (ConsoleOut.Verbosity >= Verbosity.Minimal)
                AskMode = Options.AskMode;

            base.ExecuteCore(context);
        }

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            FileMatch? fileMatch = MatchFile(filePath);

            if (fileMatch != null)
            {
                if (ContentFilter != null)
                {
                    ExecuteMatchWithContent(fileMatch, context, FileWriterOptions);
                }
                else
                {
                    ExecuteMatch(fileMatch, context);
                }
            }
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            foreach (FileMatch fileMatch in GetMatches(directoryPath, context))
            {
                if (ContentFilter != null)
                {
                    ExecuteMatchWithContent(fileMatch, context, DirectoryWriterOptions, directoryPath);
                }
                else
                {
                    ExecuteMatch(fileMatch, context, directoryPath);
                }

                if (context.TerminationReason == TerminationReason.Canceled)
                    break;

                if (context.TerminationReason == TerminationReason.MaxReached)
                    break;
            }
        }

        protected void ExecuteMatchWithContent(
            FileMatch fileMatch,
            SearchContext context,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath = null)
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
                var searchResult = new SearchResult(fileMatch, baseDirectoryPath, writerOptions);

                context.Results.Add(searchResult);
            }
            else
            {
                EndProgress(context);

                ExecuteMatchWithContentCore(fileMatch, context, writerOptions, baseDirectoryPath);
            }
        }

        protected abstract void ExecuteMatchWithContentCore(
            FileMatch fileMatch,
            SearchContext context,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null);

        protected override void ExecuteResult(SearchResult result, SearchContext context, ColumnWidths? columnWidths)
        {
            if (ContentFilter != null)
            {
                ExecuteMatchWithContentCore(result.FileMatch, context, result.WriterOptions!, result.BaseDirectoryPath, columnWidths);
            }
            else
            {
                ExecuteMatchCore(result.FileMatch, context, result.BaseDirectoryPath, columnWidths);
            }
        }

        protected void AskToContinue(SearchContext context, string indent)
        {
            if (AskMode == AskMode.File)
            {
                try
                {
                    if (ConsoleHelpers.AskToContinue(indent))
                        AskMode = AskMode.None;
                }
                catch (OperationCanceledException)
                {
                    context.TerminationReason = TerminationReason.Canceled;
                }
            }
        }

        protected void WriteMatches(ContentWriter writer, IEnumerable<Capture> captures, SearchContext context)
        {
            try
            {
                writer.WriteMatches(captures, context.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                context.TerminationReason = TerminationReason.Canceled;
            }
        }

        protected MaxReason GetCaptures(
            Match match,
            int groupNumber,
            SearchContext context,
            bool isPathWritten,
            Func<Capture, bool>? predicate,
            List<Capture> captures)
        {
            int maxMatchesInFile = Options.MaxMatchesInFile;
            int maxTotalMatches = Options.MaxTotalMatches;

            int count = 0;

            if (maxMatchesInFile > 0)
            {
                if (maxTotalMatches > 0)
                {
                    maxTotalMatches -= context.Telemetry.MatchCount;
                    count = Math.Min(maxMatchesInFile, maxTotalMatches);
                }
                else
                {
                    count = maxMatchesInFile;
                }
            }
            else if (maxTotalMatches > 0)
            {
                maxTotalMatches -= context.Telemetry.MatchCount;
                count = maxTotalMatches;
            }

            Debug.Assert(count >= 0, count.ToString());

            MaxReason maxReason = CaptureFactory.GetCaptures(ref captures, match, groupNumber, count, predicate, context.CancellationToken);

            if ((maxReason == MaxReason.CountEqualsMax || maxReason == MaxReason.CountExceedsMax)
                && maxTotalMatches > 0
                && (maxMatchesInFile == 0 || maxTotalMatches <= maxMatchesInFile))
            {
                context.TerminationReason = TerminationReason.MaxReached;
            }

            if (isPathWritten)
            {
                Verbosity verbosity = ConsoleOut.Verbosity;

                if (verbosity >= Verbosity.Detailed
                    || Options.IncludeCount)
                {
                    verbosity = (Options.IncludeCount) ? Verbosity.Minimal : Verbosity.Detailed;

                    ConsoleOut.Write("  ", Colors.Message_OK, verbosity);
                    ConsoleOut.Write(captures.Count.ToString("n0"), Colors.Message_OK, verbosity);
                    ConsoleOut.WriteIf(maxReason == MaxReason.CountExceedsMax, "+", Colors.Message_OK, verbosity);
                }

                ConsoleOut.WriteLine(Verbosity.Minimal);

                if (Out != null)
                {
                    verbosity = Out.Verbosity;

                    if (verbosity >= Verbosity.Detailed
                        || Options.IncludeCount)
                    {
                        verbosity = (Options.IncludeCount) ? Verbosity.Minimal : Verbosity.Detailed;

                        Out.Write("  ", verbosity);
                        Out.Write(captures.Count.ToString("n0"), verbosity);
                        Out.WriteIf(maxReason == MaxReason.CountExceedsMax, "+", verbosity);
                    }

                    Out.WriteLine(Verbosity.Minimal);
                }
            }

            return maxReason;
        }

        protected override void WritePath(SearchContext context, FileMatch fileMatch, string? baseDirectoryPath, string indent, ColumnWidths? columnWidths)
        {
            if (ContentFilter != null)
            {
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths, Colors.Match_Path);
            }
            else
            {
                base.WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);
            }
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget, verbosity);

            if (!ShouldLog(verbosity))
                return;

            WriteLine(verbosity);

            if (ContentFilter != null)
            {
                WriteCount("Matches", telemetry.MatchCount, Colors.Message_OK, verbosity);
                Write("  ", Colors.Message_OK, verbosity);

                if (telemetry.MatchingLineCount > 0)
                {
                    WriteCount("Matching lines", telemetry.MatchingLineCount, Colors.Message_OK, verbosity);
                    Write("  ", Colors.Message_OK, verbosity);
                }

                if (telemetry.MatchingFileCount > 0)
                    WriteCount("Matching files", telemetry.MatchingFileCount, Colors.Message_OK, verbosity);
            }
            else
            {
                if (Options.SearchTarget != SearchTarget.Directories)
                    WriteCount("Matching files", telemetry.MatchingFileCount, Colors.Message_OK, verbosity);

                if (Options.SearchTarget != SearchTarget.Files)
                {
                    if (Options.SearchTarget != SearchTarget.Directories)
                        Write("  ", Colors.Message_OK, verbosity);

                    WriteCount("Matching directories", telemetry.MatchingDirectoryCount, Colors.Message_OK, verbosity);
                }
            }

            WriteLine(verbosity);
        }
    }
}
