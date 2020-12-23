// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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

        protected override void ExecuteCore(SearchContext context)
        {
            _aggregate = AggregateManager.TryCreate(Options);

            base.ExecuteCore(context);

            _aggregate?.WriteAggregatedValues(context.CancellationToken);
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

            if (!Options.OmitPath)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

            if (ContentFilter!.IsNegative
                || fileMatch.IsDirectory)
            {
                WriteLineIf(!Options.OmitPath, Verbosity.Minimal);
            }
            else
            {
                WriteContent(fileMatch, writerOptions, baseDirectoryPath, context);
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

            if (!Options.OmitPath)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

            if (_aggregate?.Storage != null
                && fileMatch.NameMatch != null
                && !object.ReferenceEquals(fileMatch.NameMatch, Match.Empty))
            {
                _aggregate.Storage.Add(fileMatch.NameMatch.Value);
            }

            AskToContinue(context, indent);
        }

        private void WriteContent(
            FileMatch fileMatch,
            ContentWriterOptions writerOptions,
            string? baseDirectoryPath,
            SearchContext context)
        {
            SearchTelemetry telemetry = context.Telemetry;

            ContentWriter? contentWriter = null;
            List<Capture>? captures = null;

            try
            {
                captures = ListCache<Capture>.GetInstance();

                GetCaptures(
                    fileMatch.ContentMatch!,
                    writerOptions.GroupNumber,
                    context,
                    isPathWritten: !Options.OmitPath,
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
                        input: fileMatch.ContentText,
                        options: writerOptions,
                        storage: (hasAnyFunction) ? _modify?.FileStorage : _aggregate?.Storage,
                        outputInfo: Options.CreateOutputInfo(fileMatch.ContentText, fileMatch.ContentMatch!, ContentFilter),
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
                    _modify!.WriteValues(writerOptions.Indent, telemetry, _aggregate);

                    _aggregate?.Sections?.Add(new StorageSection(fileMatch, baseDirectoryPath, _aggregate.Storage.Count));
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
