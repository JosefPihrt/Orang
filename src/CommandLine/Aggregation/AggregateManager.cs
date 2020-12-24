// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Orang.CommandLine;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.Aggregation
{
    internal sealed class AggregateManager
    {
        private AggregateManager(
            ListResultStorage storage,
            List<StorageSection>? sections,
            FindCommandOptions options)
        {
            Storage = storage;
            Sections = sections;
            Options = options;
        }

        public ListResultStorage Storage { get; set; }

        public List<StorageSection>? Sections { get; set; }

        public FindCommandOptions Options { get; }

        public ModifyOptions ModifyOptions => Options.ModifyOptions;

        public static AggregateManager? TryCreate(FindCommandOptions Options)
        {
            ModifyOptions modifyOptions = Options.ModifyOptions;

            bool shouldCreate = modifyOptions.HasFunction(ModifyFunctions.Except_Intersect_GroupBy)
                || (modifyOptions.Aggregate
                    && (modifyOptions.Method != null
                        || modifyOptions.HasFunction(ModifyFunctions.Enumerable)));

            if (!shouldCreate)
                return null;

            var storage = new ListResultStorage();

            List<StorageSection>? sections = null;

            if (modifyOptions.HasFunction(ModifyFunctions.Except_Intersect))
            {
                sections = new List<StorageSection>();
            }
            else if (Options.ContentFilter != null
                && modifyOptions.HasFunction(ModifyFunctions.GroupBy))
            {
                sections = new List<StorageSection>();
            }

            return new AggregateManager(storage, sections, Options);
        }

        public void WriteAggregatedValues(CancellationToken cancellationToken)
        {
            int count = 0;

            IEnumerable<string> values = Storage.Values;

            if (Sections?.Count > 1
                && (ModifyOptions.HasFunction(ModifyFunctions.Except_Intersect)))
            {
                if (ModifyOptions.HasFunction(ModifyFunctions.Except))
                {
                    values = ExceptOrIntersect(Storage.Values, (x, y, c) => x.Except(y, c));
                }
                else if (ModifyOptions.HasFunction(ModifyFunctions.Intersect))
                {
                    values = ExceptOrIntersect(Storage.Values, (x, y, c) => x.Intersect(y, c));
                }
            }

            Dictionary<string, IEnumerable<StorageSection>>? valuesMap = null;

            if (Sections?.Count > 0
                && ModifyOptions.HasFunction(ModifyFunctions.GroupBy))
            {
                valuesMap = GroupByValues(Storage.Values);

                values = valuesMap
                    .Select(f => f.Key)
                    .Modify(ModifyOptions, filter: ModifyFunctions.Enumerable & ~ModifyFunctions.Distinct);
            }
            else
            {
                values = values.Modify(ModifyOptions, filter: ModifyFunctions.Enumerable);
            }

            using (IEnumerator<string> en = values.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    OutputSymbols symbols = OutputSymbols.Create(Options.HighlightOptions);

                    ConsoleColors colors = ((Options.HighlightOptions & HighlightOptions.Match) != 0)
                        ? Colors.Match
                        : default;

                    ConsoleColors boundaryColors = (Options.HighlightBoundary) ? Colors.MatchBoundary : default;
                    var valueWriter = new ValueWriter(new ContentTextWriter(Verbosity.Minimal), includeEndingIndent: false);

                    ConsoleOut.WriteLineIf(ShouldWriteLine(ConsoleOut.Verbosity));
                    Out?.WriteLineIf(ShouldWriteLine(Out.Verbosity));

                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        valueWriter.Write(en.Current, symbols, colors, boundaryColors);
                        WriteLine(Verbosity.Minimal);

                        if (valuesMap != null)
                            WriteGroup(valuesMap[en.Current], cancellationToken);

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

            bool ShouldWriteLine(Verbosity verbosity)
            {
                if (verbosity > Verbosity.Minimal)
                    return true;

                return verbosity == Verbosity.Minimal
                    && (Options.PathDisplayStyle != PathDisplayStyle.Omit || Options.IncludeSummary);
            }
        }

        private List<string> ExceptOrIntersect(
            List<string> values,
            Func<IEnumerable<string>, IEnumerable<string>, IEqualityComparer<string>, IEnumerable<string>> operation)
        {
            var list = new List<string>(GetRange(values, 0, Sections![0].Count));

            for (int i = 1; i < Sections.Count; i++)
            {
                if (list.Count == 0)
                    break;

                IEnumerable<string> second = GetRange(
                    values,
                    Sections[i - 1].Count,
                    Sections[i].Count);

                list = operation(list, second, ModifyOptions.StringComparer).ToList();
            }

            return list;
        }

        private Dictionary<string, IEnumerable<StorageSection>> GroupByValues(List<string> values)
        {
            var sectionsValues = new List<(StorageSection section, IEnumerable<string> values)>();

            sectionsValues.Add((Sections![0], GetRange(values, 0, Sections[0].Count)));

            for (int i = 1; i < Sections.Count; i++)
            {
                IEnumerable<string> second = GetRange(
                    values,
                    Sections[i - 1].Count,
                    Sections[i].Count);

                sectionsValues.Add((Sections[i], second));
            }

            return sectionsValues
                .SelectMany(f => f.values.Select(value => (f.section, value)))
                .GroupBy(f => f.value, ModifyOptions.StringComparer)
                .ToDictionary(f => f.Key, f => f.Select(g => g.section), ModifyOptions.StringComparer);
        }

        private IEnumerable<string> GetRange(List<string> values, int start, int end)
        {
            for (int i = start; i < end; i++)
                yield return values[i];
        }

        private void WriteGroup(
            IEnumerable<StorageSection> sections,
            CancellationToken cancellationToken)
        {
            SortOptions? sortOptions = Options.SortOptions;

            if (sortOptions?.Descriptors.Any() == true)
            {
                List<SearchResult>? results = sections
                    .Select(f => new SearchResult(f.FileMatch, f.BaseDirectoryPath))
                    .ToList();

                PathDisplayStyle pathDisplayStyle = (Options.PathDisplayStyle == PathDisplayStyle.Relative)
                    ? PathDisplayStyle.Relative
                    : PathDisplayStyle.Full;

                sections = SortHelpers.SortResults(results, sortOptions.Descriptors, pathDisplayStyle)
                    .Select(f => new StorageSection(f.FileMatch, f.BaseDirectoryPath, -1));
            }

            foreach (IGrouping<StorageSection, StorageSection>? grouping in sections
                .GroupBy(f => f, StorageSectionComparer.Path))
            {
                cancellationToken.ThrowIfCancellationRequested();

                WritePath(
                    grouping.Key.FileMatch,
                    grouping.Key.BaseDirectoryPath,
                    relativePath: Options.DisplayRelativePath,
                    colors: Colors.Matched_Path,
                    matchColors: default,
                    indent: "  ",
                    verbosity: Verbosity.Minimal);

                int pathCount = grouping.Count();

                if (pathCount > 1)
                {
                    Write("  ", Colors.Message_OK, Verbosity.Minimal);
                    Write(pathCount.ToString("n0"), Colors.Message_OK, Verbosity.Minimal);
                }

                WriteLine(Verbosity.Minimal);
            }
        }
    }
}
