// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal sealed class FindCommand<TOptions> : CommonFindCommand<TOptions> where TOptions : FindCommandOptions
    {
        private OutputSymbols? _symbols;
        private IResultStorage? _storage;
        private List<int>? _storageIndexes;
        private IResultStorage? _fileStorage;
        private List<string>? _fileValues;

        public FindCommand(TOptions options) : base(options)
        {
        }

        private OutputSymbols Symbols => _symbols ??= OutputSymbols.Create(Options.HighlightOptions);

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
            bool aggregate = (Options.ModifyOptions.Functions & ModifyFunctions.ExceptIntersect) != 0
                || (Options.ModifyOptions.Aggregate
                    && (Options.ModifyOptions.Method != null
                        || (Options.ModifyOptions.Functions & ModifyFunctions.Enumerable) != 0));

            if (aggregate)
            {
                _storage = new ListResultStorage();

                if ((Options.ModifyOptions.Functions & ModifyFunctions.ExceptIntersect) != 0)
                    _storageIndexes = new List<int>();
            }

            base.ExecuteCore(context);

            if (aggregate)
                WriteAggregatedValues();
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context)
        {
            base.ExecuteDirectory(directoryPath, context);

            if (ContentFilter == null)
                _storageIndexes?.Add(_storage!.Count);
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
                WriteContent(fileMatch, writerOptions, context);
            }

            AskToContinue(context, indent);
        }

        protected override void ExecuteMatchCore(FileMatch fileMatch, SearchContext context, string? baseDirectoryPath = null, ColumnWidths? columnWidths = null)
        {
            string indent = GetPathIndent(baseDirectoryPath);

            if (!Options.OmitPath)
                WritePath(context, fileMatch, baseDirectoryPath, indent, columnWidths);

            if (_storage != null
                && fileMatch.NameMatch != null
                && !object.ReferenceEquals(fileMatch.NameMatch, Match.Empty))
            {
                _storage.Add(fileMatch.NameMatch.Value);
            }

            AskToContinue(context, indent);
        }

        private void WriteContent(
            FileMatch fileMatch,
            ContentWriterOptions writerOptions,
            SearchContext context)
        {
            SearchTelemetry telemetry = context.Telemetry;

            ContentWriter? contentWriter = null;
            List<Capture>? captures = null;

            try
            {
                captures = ListCache<Capture>.GetInstance();

                GetCaptures(fileMatch.ContentMatch!, writerOptions.GroupNumber, context, isPathWritten: !Options.OmitPath, predicate: ContentFilter!.Predicate, captures: captures);

                bool hasAnyFunction = Options.ModifyOptions.HasAnyFunction;

                if (hasAnyFunction
                    || Options.AskMode == AskMode.Value
                    || ShouldLog(Verbosity.Normal))
                {
                    if (hasAnyFunction)
                    {
                        if (_fileValues == null)
                        {
                            _fileValues = new List<string>();
                        }
                        else
                        {
                            _fileValues.Clear();
                        }

                        if (_fileStorage == null)
                            _fileStorage = new ListResultStorage(_fileValues);
                    }

                    contentWriter = ContentWriter.CreateFind(
                        contentDisplayStyle: Options.ContentDisplayStyle,
                        input: fileMatch.ContentText,
                        options: writerOptions,
                        storage: (hasAnyFunction) ? _fileStorage : _storage,
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
                    ConsoleColors colors = (Options.HighlightMatch) ? Colors.Match : default;
                    ConsoleColors boundaryColors = (Options.HighlightBoundary) ? Colors.MatchBoundary : default;

                    var valueWriter = new ValueWriter(ContentTextWriter.Default, writerOptions.Indent, includeEndingIndent: false);

                    foreach (string value in _fileValues!.Modify(Options.ModifyOptions))
                    {
                        Write(writerOptions.Indent, Verbosity.Normal);
                        valueWriter.Write(value, Symbols, colors, boundaryColors);
                        WriteLine(Verbosity.Normal);

                        _storage?.Add(value);
                        telemetry.MatchCount++;
                    }

                    _storageIndexes?.Add(_storage!.Count);
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

        private void WriteAggregatedValues()
        {
            int count = 0;
            ModifyOptions modifyOptions = Options.ModifyOptions;
            List<string> allValues = ((ListResultStorage)_storage!).Values;

            if (_storageIndexes?.Count > 1)
            {
                Debug.Assert((modifyOptions.Functions & ModifyFunctions.ExceptIntersect) != 0, modifyOptions.Functions.ToString());

                if ((modifyOptions.Functions & ModifyFunctions.Except) != 0)
                {
                    if (_storageIndexes.Count > 2)
                        throw new InvalidOperationException($"'Except' operation cannot be applied on more than two {((ContentFilter != null) ? "files" : "directories")}.");

                    int index = _storageIndexes[0];

                    allValues = allValues
                        .Take(index)
                        .Except(GetRange(index, allValues.Count))
                        .ToList();
                }
                else if ((modifyOptions.Functions & ModifyFunctions.Intersect) != 0)
                {
                    allValues = Intersect();
                }
            }

            using (IEnumerator<string> en = allValues
                .Modify(modifyOptions, filter: ModifyFunctions.Enumerable)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    OutputSymbols symbols = OutputSymbols.Create(Options.HighlightOptions);
                    ConsoleColors colors = ((Options.HighlightOptions & HighlightOptions.Match) != 0) ? Colors.Match : default;
                    ConsoleColors boundaryColors = (Options.HighlightBoundary) ? Colors.MatchBoundary : default;
                    var valueWriter = new ValueWriter(new ContentTextWriter(Verbosity.Minimal), includeEndingIndent: false);

                    ConsoleOut.WriteLineIf(ShouldWriteLine(ConsoleOut.Verbosity));
                    Out?.WriteLineIf(ShouldWriteLine(Out.Verbosity));

                    do
                    {
                        valueWriter.Write(en.Current, symbols, colors, boundaryColors);
                        WriteLine(Verbosity.Minimal);
                        count++;

                    } while (en.MoveNext());

                    if (ShouldLog(Verbosity.Detailed)
                        || Options.IncludeSummary)
                    {
                        WriteLine(Verbosity.Minimal);
                        WriteCount("Values", count, verbosity: Verbosity.Minimal);
                        WriteLine(Verbosity.Minimal);
                    }
                }
            }

            List<string> Intersect()
            {
                var list = new List<string>(GetRange(0, _storageIndexes[0]));

                for (int i = 1; i < _storageIndexes.Count; i++)
                {
                    IEnumerable<string> second = GetRange(_storageIndexes[i - 1], _storageIndexes[i]);

                    list = list.Intersect(second, modifyOptions.StringComparer).ToList();
                }

                return list;
            }

            IEnumerable<string> GetRange(int start, int end)
            {
                for (int i = start; i < end; i++)
                    yield return allValues[i];
            }

            bool ShouldWriteLine(Verbosity verbosity)
            {
                if (verbosity > Verbosity.Minimal)
                    return true;

                return verbosity == Verbosity.Minimal
                    && (Options.PathDisplayStyle != PathDisplayStyle.Omit || Options.IncludeSummary);
            }
        }
    }
}
