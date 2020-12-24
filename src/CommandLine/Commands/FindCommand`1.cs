﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        private AggregateManager? _aggregate;
        private ModifyManager? _modify;

        public FindCommand(TOptions options) : base(options)
        {
        }

        public override bool CanEndProgress
        {
            get
            {
                return !Options.OmitPath
                    || (ContentFilter != null && ConsoleOut.Verbosity > Verbosity.Minimal);
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
                    isPathWritten: false);
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
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

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
                    isPathWritten: isPathDisplayed);
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
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);
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
            Match contentMatch,
            string contentText,
            ContentWriterOptions writerOptions,
            FileMatch? fileMatch,
            string? baseDirectoryPath,
            bool isPathWritten)
        {
            SearchTelemetry telemetry = context.Telemetry;

            ContentWriter? contentWriter = null;
            List<Capture>? captures = null;

            try
            {
                captures = ListCache<Capture>.GetInstance();

                GetCaptures(
                    contentMatch!,
                    writerOptions.GroupNumber,
                    context,
                    isPathWritten: isPathWritten,
                    predicate: ContentFilter!.Predicate,
                    captures: captures);

                bool hasAnyFunction = Options.ModifyOptions.HasAnyFunction;

                if (hasAnyFunction
                    || Options.AskMode == AskMode.Value
                    || ShouldLog(Verbosity.Normal))
                {
                    if (hasAnyFunction)
                        (_modify ??= new ModifyManager(Options)).Reset();

                    contentWriter = ContentWriter.CreateFind(
                        contentDisplayStyle: Options.ContentDisplayStyle,
                        input: contentText,
                        options: writerOptions,
                        storage: (hasAnyFunction) ? _modify?.FileStorage : _aggregate?.Storage,
                        outputInfo: Options.CreateOutputInfo(contentText, contentMatch!, ContentFilter),
                        writer: (hasAnyFunction) ? null : ContentTextWriter.Default,
                        ask: AskMode == AskMode.Value);
                }
                else
                {
                    contentWriter = new EmptyContentWriter(writerOptions);
                }

                WriteMatches(contentWriter, captures, context);

                if (hasAnyFunction)
                {
                    if (AggregateOnly)
                    {
                        IEnumerable<string> values = _modify!.FileValues!.Modify(
                            Options.ModifyOptions,
                            filter: Options.ModifyOptions.Functions & ~(ModifyFunctions.Enumerable | ModifyFunctions.Except_Intersect_GroupBy));

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

                if (captures != null)
                    ListCache<Capture>.Free(captures);
            }
        }
    }
}
