// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

#pragma warning disable IDE0068

namespace Orang.CommandLine
{
    internal sealed class ReplaceCommand : CommonFindCommand<ReplaceCommandOptions>
    {
        private OutputSymbols? _symbols;

        public ReplaceCommand(ReplaceCommandOptions options) : base(options)
        {
            Debug.Assert(!options.ContentFilter!.IsNegative);
        }

        protected override bool CanDisplaySummary => Options.Input == null;

        private OutputSymbols Symbols => _symbols ??= OutputSymbols.Create(Options.HighlightOptions);

        protected override void ExecuteCore(SearchContext context)
        {
            context.Telemetry.MatchingLineCount = -1;

            if (Options.Input != null)
            {
                ExecuteInput(context, Options.Input);
            }
            else
            {
                base.ExecuteCore(context);
            }
        }

        private void ExecuteInput(SearchContext context, string input)
        {
            int count = 0;
            var maxReason = MaxReason.None;
            Filter contentFilter = ContentFilter!;
            Match? match = contentFilter.Match(input);

            if (match != null)
            {
                ContentWriter? contentWriter = null;
                List<Capture>? groups = null;

                try
                {
                    groups = ListCache<Capture>.GetInstance();

                    maxReason = GetCaptures(
                        match,
                        FileWriterOptions.GroupNumber,
                        context,
                        isPathDisplayed: false,
                        predicate: contentFilter.Predicate,
                        captures: groups);

                    if (ShouldLog(Verbosity.Normal))
                    {
                        MatchOutputInfo? outputInfo = Options.CreateOutputInfo(input, match, contentFilter);

                        contentWriter = ContentWriter.CreateReplace(
                            Options.ContentDisplayStyle,
                            input,
                            Options.ReplaceOptions,
                            FileWriterOptions,
                            outputInfo: outputInfo);
                    }
                    else
                    {
                        contentWriter = new EmptyContentWriter(FileWriterOptions);
                    }

                    WriteMatches(contentWriter, groups, context);
                    count = contentWriter.MatchCount;
                }
                finally
                {
                    contentWriter?.Dispose();

                    if (groups != null)
                        ListCache<Capture>.Free(groups);
                }
            }

            if (ShouldLog(Verbosity.Detailed)
                || Options.IncludeSummary)
            {
                Verbosity verbosity = (Options.IncludeSummary) ? Verbosity.Minimal : Verbosity.Detailed;

                WriteLine(verbosity);
                WriteCount("Replacements", count, Colors.Message_OK, verbosity);
                WriteIf(maxReason == MaxReason.CountExceedsMax, "+", Colors.Message_OK, verbosity);
                WriteLine(verbosity);
            }
        }

        protected override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            throw new NotSupportedException();
        }

        protected override void ExecuteMatchWithContentCore(
            FileMatch fileMatch,
            SearchContext context,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            if (!Options.OmitPath)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

            SearchTelemetry telemetry = context.Telemetry;

            ContentWriter? contentWriter = null;
            TextWriter? textWriter = null;
            List<Capture>? groups = null;

            try
            {
                groups = ListCache<Capture>.GetInstance();

                GetCaptures(
                    fileMatch.ContentMatch!,
                    writerOptions.GroupNumber,
                    context,
                    isPathDisplayed: !Options.OmitPath,
                    predicate: Options.ContentFilter!.Predicate,
                    captures: groups);

                int fileMatchCount = 0;
                int fileReplacementCount = 0;

                if (!Options.DryRun)
                {
                    if (Options.AskMode == AskMode.File)
                    {
                        textWriter = new StringWriter();
                    }
                    else if (Options.AskMode == AskMode.None)
                    {
                        textWriter = new StreamWriter(fileMatch.Path, false, fileMatch.Encoding);
                    }
                }

                if (Options.AskMode == AskMode.Value
                    || ShouldLog(Verbosity.Normal))
                {
                    MatchOutputInfo? outputInfo = Options.CreateOutputInfo(
                        fileMatch.ContentText,
                        fileMatch.ContentMatch!,
                        ContentFilter!);

                    if (Options.AskMode == AskMode.Value)
                    {
                        Lazy<TextWriter>? lazyWriter = (Options.DryRun)
                            ? null
                            : new Lazy<TextWriter>(() => new StreamWriter(fileMatch.Path, false, fileMatch.Encoding));

                        contentWriter = AskReplacementWriter.Create(
                            Options.ContentDisplayStyle,
                            fileMatch.ContentText,
                            Options.ReplaceOptions,
                            lazyWriter,
                            writerOptions,
                            outputInfo);
                    }
                    else
                    {
                        contentWriter = ContentWriter.CreateReplace(
                            Options.ContentDisplayStyle,
                            fileMatch.ContentText,
                            Options.ReplaceOptions,
                            writerOptions,
                            textWriter,
                            outputInfo);
                    }
                }
                else if (Options.DryRun)
                {
                    contentWriter = new EmptyContentWriter(FileWriterOptions);
                }
                else
                {
                    contentWriter = new TextWriterContentWriter(
                        fileMatch.ContentText,
                        Options.ReplaceOptions,
                        textWriter!,
                        writerOptions);
                }

                WriteMatches(contentWriter, groups, context);

                fileMatchCount = contentWriter.MatchCount;

                fileReplacementCount = (contentWriter is AskReplacementWriter askReplacementWriter)
                    ? askReplacementWriter.ReplacementCount
                    : fileMatchCount;

                telemetry.MatchCount += fileMatchCount;

                if (contentWriter.MatchingLineCount >= 0)
                {
                    if (telemetry.MatchingLineCount == -1)
                        telemetry.MatchingLineCount = 0;

                    telemetry.MatchingLineCount += contentWriter.MatchingLineCount;
                }

                if (Options.AskMode == AskMode.File)
                {
                    if (context.TerminationReason == TerminationReason.Canceled)
                    {
                        fileReplacementCount = 0;
                    }
                    else
                    {
                        try
                        {
                            if (Options.DryRun)
                            {
                                if (ConsoleHelpers.AskToContinue(indent))
                                    Options.AskMode = AskMode.None;
                            }
                            else if (ConsoleHelpers.AskToExecute("Replace content?", indent))
                            {
                                File.WriteAllText(fileMatch.Path, textWriter!.ToString(), fileMatch.Encoding);
                            }
                            else
                            {
                                fileReplacementCount = 0;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            context.TerminationReason = TerminationReason.Canceled;
                            fileReplacementCount = 0;
                        }
                    }
                }
                else if (Options.AskMode == AskMode.Value)
                {
                    if (((AskReplacementWriter)contentWriter).ContinueWithoutAsking)
                        Options.AskMode = AskMode.None;
                }

                telemetry.ProcessedMatchCount += fileReplacementCount;

                if (fileReplacementCount > 0)
                    telemetry.ProcessedFileCount++;
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                WriteFileError(ex, indent: indent);
            }
            finally
            {
                contentWriter?.Dispose();

                if (groups != null)
                    ListCache<Capture>.Free(groups);
            }
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget, verbosity);

            if (!ShouldLog(verbosity))
                return;

            WriteLine(verbosity);

            string matchCount = telemetry.MatchCount.ToString("n0");
            string matchingFileCount = telemetry.MatchingFileCount.ToString("n0");
            string processedMatchCount = telemetry.ProcessedMatchCount.ToString("n0");
            string processedFileCount = telemetry.ProcessedFileCount.ToString("n0");

            const string matchesTitle = "Matches";
            const string matchingFilesTitle = "Matching files";
            const string replacementsTitle = "Replacements";
            const string replacedFilesTitle = "Replaced files";

            int width1 = Math.Max(matchesTitle.Length, replacementsTitle.Length);
            int width2 = Math.Max(matchCount.Length, processedMatchCount.Length);
            int width3 = Math.Max(matchingFilesTitle.Length, replacedFilesTitle.Length);
            int width4 = Math.Max(matchingFileCount.Length, processedFileCount.Length);

            ConsoleColors colors = Colors.Message_OK;

            WriteCount(matchesTitle, matchCount, width1, width2, colors, verbosity);
            Write("  ", colors, verbosity);

            int matchingLinesWidth = 0;
            if (telemetry.MatchingLineCount >= 0)
            {
                matchingLinesWidth += WriteCount("Matching lines", telemetry.MatchingLineCount, colors, verbosity);
                Write("  ", colors, verbosity);
                matchingLinesWidth += 2;
            }

            WriteCount(matchingFilesTitle, matchingFileCount, width3, width4, colors, verbosity);
            WriteLine(verbosity);

            colors = (Options.DryRun) ? Colors.Message_DryRun : Colors.Message_Change;

            WriteCount(replacementsTitle, processedMatchCount, width1, width2, colors, verbosity);
            Write("  ", colors, verbosity);
            Write(' ', matchingLinesWidth, colors, verbosity);
            WriteCount(replacedFilesTitle, processedFileCount, width3, width4, colors, verbosity);

            WriteLine(verbosity);
        }

        protected override ContentWriterOptions CreateContentWriterOptions(string indent)
        {
            return new ContentWriterOptions(
                format: Options.Format,
                symbols: Symbols,
                highlightOptions: Options.HighlightOptions,
                indent: indent);
        }
    }
}
