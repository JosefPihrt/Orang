// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Orang.Aggregation;
using Orang.FileSystem;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal sealed class FindCommand<TOptions> : CommonFindCommand<TOptions> where TOptions : FindCommandOptions
    {
        private static OutputCaptions? _splitCaptions;

        private AggregateManager? _aggregate;
        private ModifyManager? _modify;

        public FindCommand(TOptions options) : base(options)
        {
        }

        private static OutputCaptions SplitCaptions
            => _splitCaptions ??= OutputCaptions.Default.Update(shortMatch: OutputCaptions.Default.ShortSplit);

        public override bool CanEndProgress
        {
            get
            {
                return !Options.OmitPath
                    || (ContentFilter != null
                        && Options.ContentDisplayStyle != ContentDisplayStyle.Omit
                        && ConsoleOut.Verbosity > Verbosity.Minimal);
            }
        }

        public bool AggregateOnly => Options.AggregateOnly;

        protected override bool CanDisplaySummary => !AggregateOnly && base.CanDisplaySummary;

        protected override void ExecuteCore(SearchContext context)
        {
            if (Options.Input != null)
            {
                ExecuteInput(context, Options.Input);
            }
            else
            {
                _aggregate = AggregateManager.TryCreate(Options);

                base.ExecuteCore(context);

                _aggregate?.WriteAggregatedValues(context.CancellationToken);
            }
        }

        private void ExecuteInput(SearchContext context, string input)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Match? match = ContentFilter!.Match(input);

            ContentWriterOptions writerOptions = CreateContentWriterOptions("");

            if (match != null)
            {
                WriteContent(
                    context,
                    match,
                    input,
                    writerOptions,
                    fileMatch: null,
                    baseDirectoryPath: null,
                    isPathDisplayed: false);
            }

            stopwatch.Stop();

            if (ShouldWriteSummary())
            {
                context.Telemetry.Elapsed = stopwatch.Elapsed;

                Verbosity verbosity = (Options.IncludeSummary) ? Verbosity.Quiet : Verbosity.Detailed;

                WriteLine(verbosity);
                WriteContentSummary(context.Telemetry, verbosity);
                WriteLine(verbosity);
            }
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            base.ExecuteDirectory(directoryPath, context);

            if (ContentFilter == null)
                _aggregate?.Sections?.Add(new StorageSection(null!, null, _aggregate.Storage.Count));
        }

        protected override void ExecuteMatchWithContentCore(
            FileMatch fileMatch,
            SearchContext context,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            bool isPathDisplayed = !Options.OmitPath && !AggregateOnly;

            if (isPathDisplayed)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths, includeNewline: false);

            if (ContentFilter!.IsNegative
                || fileMatch.IsDirectory)
            {
                WriteLineIf(isPathDisplayed, Verbosity.Minimal);
            }
            else
            {
                WriteContent(
                    context,
                    fileMatch.ContentMatch!,
                    fileMatch.ContentText,
                    writerOptions,
                    fileMatch,
                    baseDirectoryPath,
                    isPathDisplayed: isPathDisplayed);
            }

            AskToContinue(context, indent);
        }

        protected override void ExecuteMatchCore(
            FileMatch fileMatch,
            SearchContext context,
            string? baseDirectoryPath = null,
            ColumnWidths? columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            if (!Options.OmitPath
                && !AggregateOnly)
            {
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths, includeNewline: true);
            }

            if (_aggregate?.Storage != null
                && fileMatch.NameMatch != null
                && !object.ReferenceEquals(fileMatch.NameMatch, Match.Empty))
            {
                _aggregate.Storage.Add(fileMatch.NameMatch.Value);
                _aggregate.Sections?.Add(new StorageSection(fileMatch, baseDirectoryPath, 0));
            }

            AskToContinue(context, indent);
        }

        private void WriteContent(
            SearchContext context,
            Match match,
            string content,
            ContentWriterOptions writerOptions,
            FileMatch? fileMatch,
            string? baseDirectoryPath,
            bool isPathDisplayed)
        {
            SearchTelemetry telemetry = context.Telemetry;
            ContentWriter? contentWriter = null;

            try
            {
                if (Options.Split)
                {
                    contentWriter = WriteSplits(match, content, writerOptions, isPathDisplayed, context);
                }
                else
                {
                    contentWriter = WriteMatches(match, content, writerOptions, isPathDisplayed, context);
                }

                if (Options.ModifyOptions.HasAnyFunction)
                {
                    if (AggregateOnly)
                    {
                        IEnumerable<string> values = _modify!.FileValues!.Modify(
                            Options.ModifyOptions,
                            filter: Options.ModifyOptions.Functions
                                & ~(ModifyFunctions.Enumerable | ModifyFunctions.Except_Intersect_Group));

                        _aggregate?.Storage.AddRange(values);
                    }
                    else
                    {
                        _modify!.WriteValues(writerOptions.Indent, telemetry, _aggregate);
                    }

                    _aggregate?.Sections?.Add(new StorageSection(fileMatch!, baseDirectoryPath, _aggregate.Storage.Count));
                }
                else
                {
                    telemetry.MatchCount += contentWriter.MatchCount;

                    if (contentWriter.MatchingLineCount >= 0)
                    {
                        if (telemetry.MatchingLineCount == -1)
                            telemetry.MatchingLineCount = 0;

                        telemetry.MatchingLineCount += contentWriter.MatchingLineCount;
                    }
                }

                if (AskMode == AskMode.Value)
                {
                    if (contentWriter is AskValueContentWriter askValueContentWriter)
                    {
                        if (!askValueContentWriter.Ask)
                            AskMode = AskMode.None;
                    }
                    else if (contentWriter is AskLineContentWriter askLineContentWriter)
                    {
                        if (!askLineContentWriter.Ask)
                            AskMode = AskMode.None;
                    }
                }
            }
            finally
            {
                contentWriter?.Dispose();
            }
        }

        private ContentWriter WriteMatches(
            Match match,
            string content,
            ContentWriterOptions writerOptions,
            bool isPathDisplayed,
            SearchContext context)
        {
            ContentWriter? contentWriter;

            bool hasAnyFunction = Options.ModifyOptions.HasAnyFunction;

            if (hasAnyFunction
                || Options.AskMode == AskMode.Value
                || (Options.ContentDisplayStyle != ContentDisplayStyle.Omit
                    && ShouldLog(Verbosity.Normal)))
            {
                if (hasAnyFunction)
                    (_modify ??= new ModifyManager(Options)).Reset();

                contentWriter = ContentWriter.CreateFind(
                    contentDisplayStyle: Options.ContentDisplayStyle,
                    input: content,
                    options: writerOptions,
                    storage: (hasAnyFunction) ? _modify?.FileStorage : _aggregate?.Storage,
                    outputInfo: Options.CreateOutputInfo(content, match, ContentFilter!),
                    writer: (hasAnyFunction) ? null : ContentTextWriter.Default,
                    ask: AskMode == AskMode.Value);
            }
            else
            {
                contentWriter = new EmptyContentWriter(writerOptions);
            }

            WriteMatches(context, match, writerOptions, contentWriter, isPathDisplayed);

            return contentWriter;
        }

        private ContentWriter WriteSplits(
            Match match,
            string content,
            ContentWriterOptions writerOptions,
            bool isPathDisplayed,
            SearchContext context)
        {
            ContentWriter? contentWriter = null;
            List<CaptureInfo>? splits = null;

            try
            {
                splits = ListCache<CaptureInfo>.GetInstance();

                (int maxMatchesInFile, int maxTotalMatches, int count) = CalculateMaxCount(context);

                MaxReason maxReason = CaptureFactory.GetSplits(
                    match,
                    Options.ContentFilter!.Regex,
                    content,
                    count,
                    Options.ContentFilter.Predicate,
                    ref splits,
                    context.CancellationToken);

                if ((maxReason == MaxReason.CountEqualsMax || maxReason == MaxReason.CountExceedsMax)
                    && maxTotalMatches > 0
                    && (maxMatchesInFile == 0 || maxTotalMatches <= maxMatchesInFile))
                {
                    context.TerminationReason = TerminationReason.MaxReached;
                }

                if (isPathDisplayed)
                    LogHelpers.WriteFilePathEnd(splits.Count, maxReason, Options.IncludeCount);

                bool hasAnyFunction = Options.ModifyOptions.HasAnyFunction;

                if (hasAnyFunction
                    || Options.AskMode == AskMode.Value
                    || (Options.ContentDisplayStyle != ContentDisplayStyle.Omit
                        && ShouldLog(Verbosity.Normal)))
                {
                    if (hasAnyFunction)
                        (_modify ??= new ModifyManager(Options)).Reset();

                    MatchOutputInfo? outputInfo = (Options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail)
                        ? MatchOutputInfo.Create(splits, SplitCaptions)
                        : null;

                    contentWriter = ContentWriter.CreateFind(
                        contentDisplayStyle: Options.ContentDisplayStyle,
                        input: content,
                        options: writerOptions,
                        storage: (hasAnyFunction) ? _modify?.FileStorage : _aggregate?.Storage,
                        outputInfo: outputInfo,
                        writer: (hasAnyFunction) ? null : ContentTextWriter.Default,
                        ask: AskMode == AskMode.Value);
                }
                else
                {
                    contentWriter = new EmptyContentWriter(writerOptions);
                }

                contentWriter.WriteMatches(splits, context.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                context.TerminationReason = TerminationReason.Canceled;
            }
            finally
            {
                if (splits != null)
                    ListCache<CaptureInfo>.Free(splits);
            }

            return contentWriter!;
        }
    }
}
