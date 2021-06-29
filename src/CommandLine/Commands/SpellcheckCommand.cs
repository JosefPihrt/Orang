// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using Orang.Spelling;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal sealed class SpellcheckCommand : CommonReplaceCommand<SpellcheckCommandOptions>
    {
        public SpellcheckCommand(SpellcheckCommandOptions options) : base(options)
        {
            SpellcheckState = (SpellcheckState)options.Replacer;
        }

        protected override SpellcheckState SpellcheckState { get; }

        protected override bool ReportProgress => true;

        protected override bool CanUseStreamWriter => false;

        protected override bool CanUseEmptyWriter => false;

        protected override void ExecuteCore(SearchContext context)
        {
            base.ExecuteCore(context);

            if (context.TerminationReason == TerminationReason.Canceled)
                return;
        }

        protected override List<ICapture>? GetCaptures(List<Capture> groups, CancellationToken cancellationToken)
        {
            var captures = new List<ICapture>();
            List<TextSpan>? filteredSpans = null;

            foreach (Capture capture in groups)
            {
                foreach (SpellingMatch spellingMatch in SpellcheckState.Spellchecker.AnalyzeText(capture.Value))
                {
                    var captureInfo = new SpellingCapture(
                        spellingMatch.Value,
                        capture.Index + spellingMatch.Index,
                        containingValue: spellingMatch.Parent,
                        containingValueIndex: spellingMatch.ParentIndex);

                    if (filteredSpans == null)
                        filteredSpans = GetFilteredSpans(groups, cancellationToken);

                    var captureSpan = new TextSpan(captureInfo.Index, captureInfo.Length);

                    foreach (TextSpan filteredSpan in filteredSpans)
                    {
                        if (filteredSpan.IntersectsWith(captureSpan))
                            continue;
                    }

                    captures.Add(captureInfo);
                }
            }

            return captures;
        }

        private List<TextSpan> GetFilteredSpans(List<Capture> groups, CancellationToken cancellationToken)
        {
            ImmutableArray<TextSpan>.Builder? allSpans = null;

            foreach (Capture group in groups)
            {
                foreach (Filter filter in SpellcheckState.Filters)
                {
                    Match? match = filter.Match(group.Value);

                    if (match != null)
                    {
                        var captures = new List<Capture>();

                        MaxReason _ = CaptureFactory.GetCaptures(
                            ref captures,
                            match,
                            filter.GroupNumber,
                            count: 0,
                            filter.Predicate,
                            cancellationToken);

                        if (allSpans == null)
                            allSpans = ImmutableArray.CreateBuilder<TextSpan>();

                        foreach (Capture capture in captures)
                            allSpans.Add(new TextSpan(capture.Index, capture.Length));
                    }
                }
            }

            if (allSpans == null)
                return new List<TextSpan>();

            allSpans.Sort((x, y) =>
            {
                int diff = x.Start.CompareTo(y.Start);

                return (diff != 0) ? diff : -x.Length.CompareTo(y.Length);
            });

            TextSpan span = allSpans[0];

            var spans = new List<TextSpan>() { span };

            for (int i = 1; i < allSpans.Count; i++)
            {
                TextSpan span2 = allSpans[i];

                if (span.Start == span2.Start)
                    continue;

                if (span2.Start <= span.End + 1)
                {
                    span = TextSpan.FromBounds(span.Start, span2.End);
                    spans[^1] = span;
                }
                else
                {
                    span = span2;
                    spans.Add(span);
                }
            }

            return spans;
        }

        protected override void WriteBeforeSummary()
        {
            if (!ShouldLog(Verbosity.Normal))
                return;

            var isFirst = true;
            bool isDetailed = ShouldLog(Verbosity.Detailed);
            StringComparer comparer = StringComparer.InvariantCulture;
            string contentIndent = (Options.OmitContent) ? "" : ((Options.OmitPath) ? "  " : "    ");

            if (ShouldLog(Verbosity.Normal))
            {
                foreach (IGrouping<string, SpellingFixResult> grouping in SpellcheckState.Results
                    .Where(f => !f.HasFix && f.ContainingValue != null)
                    .GroupBy(f => f.ContainingValue!, comparer)
                    .OrderBy(f => f.Key, comparer))
                {
                    if (isFirst)
                    {
                        WriteLine(Verbosity.Normal);
                        WriteLine("Words containing unknown words:", Verbosity.Normal);
                        isFirst = false;
                    }

                    WriteLine(grouping.Key, Verbosity.Normal);

                    if (isDetailed)
                        WriteMatchingLines(grouping, comparer, contentIndent, Colors.Match, displayContainingValue: true);
                }
            }

            isFirst = true;

            foreach (IGrouping<string, SpellingFixResult> grouping in SpellcheckState.Results
                .Where(f => !f.HasFix)
                .GroupBy(f => f.Value, comparer)
                .OrderBy(f => f.Key, comparer))
            {
                if (isFirst)
                {
                    WriteLine(Verbosity.Normal);
                    WriteLine("Unknown words:", Verbosity.Normal);
                    isFirst = false;
                }

                Write(grouping.Key, Verbosity.Normal);

                if (SpellcheckState.Data.Fixes.TryGetValue(grouping.Key, out ImmutableHashSet<SpellingFix>? possibleFixes))
                {
                    ImmutableArray<SpellingFix> fixes = possibleFixes
                        .Where(
                            f => (TextUtility.GetTextCasing(f.Value) != TextCasing.Undefined
                                || string.Equals(grouping.Key, f.Value, StringComparison.OrdinalIgnoreCase)))
                        .ToImmutableArray();

                    if (fixes.Any())
                    {
                        Write(": ", Colors.ContextLine, Verbosity.Normal);

                        Write(
                            string.Join(", ", fixes.Select(f => TextUtility.SetTextCasing(f.Value, TextUtility.GetTextCasing(grouping.Key)))),
                            Colors.Replacement,
                            Verbosity.Normal);
                    }
                }

                WriteLine(Verbosity.Normal);

                if (isDetailed)
                    WriteMatchingLines(grouping, comparer, contentIndent, Colors.Match);
            }

            WriteResults(SpellingFixKind.Predefined, "Auto fixes:", contentIndent, comparer, isDetailed: isDetailed);
            WriteResults(SpellingFixKind.User, "User-applied fixes:", contentIndent, comparer, isDetailed: isDetailed);
        }

        private void WriteResults(
            SpellingFixKind kind,
            string heading,
            string indent,
            StringComparer comparer,
            bool isDetailed)
        {
            var isFirst = true;

            foreach (IGrouping<string, SpellingFixResult> grouping in SpellcheckState.Results
                .Where(f => f.Kind == kind)
                .OrderBy(f => f.Value)
                .ThenBy(f => f.Replacement)
                .GroupBy(f => $"{f.Value}: {f.Replacement}"))
            {
                if (isFirst)
                {
                    WriteLine(Verbosity.Normal);
                    WriteLine(heading, Verbosity.Normal);
                    isFirst = false;
                }

                WriteLine(grouping.Key, Verbosity.Normal);

                if (isDetailed)
                    WriteMatchingLines(grouping, comparer, indent, Colors.Replacement);
            }
        }

        private void WriteMatchingLines(
            IGrouping<string, SpellingFixResult> grouping,
            StringComparer comparer,
            string indent,
            ConsoleColors colors,
            bool displayContainingValue = false)
        {
            foreach (IGrouping<string?, SpellingFixResult> grouping2 in grouping
                .GroupBy(f => f.FilePath)
                .OrderBy(f => f.Key, comparer))
            {
                if (!Options.OmitPath)
                {
                    Write("  ", Verbosity.Detailed);
                    WriteLine(grouping2.Key, Colors.Matched_Path, Verbosity.Detailed);
                }

                if (!Options.OmitContent)
                {
                    foreach (SpellingFixResult result in grouping2.OrderBy(f => f.LineNumber))
                    {
                        Write(indent, Verbosity.Detailed);

                        if (result.LineNumber != null)
                        {
                            Write(result.LineNumber.Value.ToString(), Colors.LineNumber, Verbosity.Detailed);
                            Write(" ", Verbosity.Detailed);
                        }

                        int lineStartIndex = result.LineStartIndex;
                        int lineEndIndex = result.LineEndIndex;

                        string value;
                        int index;
                        int endIndex;

                        if (displayContainingValue)
                        {
                            value = result.ContainingValue!;
                            index = result.ContainingValueIndex;
                            endIndex = result.ContainingValueIndex + value.Length;
                        }
                        else
                        {
                            value = result.Replacement ?? result.Value;
                            index = result.Index;
                            endIndex = result.Index + result.Length;
                        }

                        Write(result.Input.AsSpan(lineStartIndex, index - lineStartIndex), Verbosity.Detailed);
                        Out?.Write(">>>", Verbosity.Detailed);
                        Write(value, colors, Verbosity.Detailed);
                        Out?.Write("<<<", Verbosity.Detailed);
                        WriteLine(result.Input.AsSpan(endIndex, lineEndIndex - endIndex), Verbosity.Detailed);
                    }
                }
            }
        }

#if DEBUG
        public void SaveNewValues(
            SpellingData spellingData,
            FixList originalFixList,
            List<SpellingFixResult> results,
            string? newWordsPath = null,
            string? newFixesPath = null,
            string? outputPath = null,
            CancellationToken cancellationToken = default)
        {
            if (newFixesPath != null)
            {
                Dictionary<string, List<SpellingFix>> fixes = spellingData.Fixes.Items.ToDictionary(
                    f => f.Key,
                    f => f.Value.ToList(),
                    WordList.DefaultComparer);

                if (fixes.Count > 0)
                {
                    if (File.Exists(newFixesPath))
                    {
                        foreach (KeyValuePair<string, ImmutableHashSet<SpellingFix>> kvp in FixList.LoadFile(newFixesPath!).Items)
                        {
                            if (fixes.TryGetValue(kvp.Key, out List<SpellingFix>? list))
                            {
                                list.AddRange(kvp.Value);
                            }
                            else
                            {
                                fixes[kvp.Key] = kvp.Value.ToList();
                            }
                        }
                    }

                    foreach (KeyValuePair<string, ImmutableHashSet<SpellingFix>> kvp in originalFixList.Items)
                    {
                        if (fixes.TryGetValue(kvp.Key, out List<SpellingFix>? list))
                        {
                            list.RemoveAll(f => kvp.Value.Contains(f, SpellingFixComparer.InvariantCultureIgnoreCase));

                            if (list.Count == 0)
                                fixes.Remove(kvp.Key);
                        }
                    }
                }

                ImmutableDictionary<string, ImmutableHashSet<SpellingFix>> newFixes = fixes.ToImmutableDictionary(
                    f => f.Key.ToLowerInvariant(),
                    f => f.Value
                        .Select(f => f.WithValue(f.Value.ToLowerInvariant()))
                        .Distinct(SpellingFixComparer.InvariantCultureIgnoreCase)
                        .ToImmutableHashSet(SpellingFixComparer.InvariantCultureIgnoreCase));

                if (newFixes.Count > 0)
                    FixList.Save(newFixesPath, newFixes);
            }

            const StringComparison comparison = StringComparison.InvariantCulture;
            StringComparer comparer = StringComparer.FromComparison(comparison);

            if (newWordsPath != null)
            {
                HashSet<string> newValues = spellingData.IgnoredValues
                    .Concat(results.Select(f => f.Value))
                    .Except(spellingData.Fixes.Items.Select(f => f.Key), WordList.DefaultComparer)
                    .ToHashSet(comparer);

                var possibleNewFixes = new List<string>();

                foreach (string value in newValues)
                {
                    string valueLower = value.ToLowerInvariant();

                    ImmutableArray<string> possibleFixes = SpellingFixProvider.SwapLetters(valueLower, spellingData);

                    if (possibleFixes.Length == 0
                        && value.Length >= 8)
                    {
                        possibleFixes = SpellingFixProvider.Fuzzy(valueLower, spellingData, cancellationToken);
                    }

                    possibleNewFixes.AddRange(possibleFixes.Select(f => FixList.GetItemText(value, f)));
                }

                IEnumerable<string> compoundWords = results
                    .Select(f => f.ContainingValue)
                    .Where(f => f != null)
                    .Select(f => f!);

                WordList.Save(newWordsPath, newValues.Concat(compoundWords).Concat(possibleNewFixes), comparer, merge: true);
            }

            if (outputPath != null
                && results.Count > 0)
            {
                using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
                {
                    foreach (IGrouping<string, SpellingFixResult> grouping in results
                        .GroupBy(f => f.Value, comparer)
                        .OrderBy(f => f.Key, comparer))
                    {
                        writer.WriteLine(grouping.Key);

                        foreach (IGrouping<string?, SpellingFixResult> grouping2 in grouping
                            .GroupBy(f => f.FilePath)
                            .OrderBy(f => f.Key, comparer))
                        {
                            writer.Write("  ");
                            writer.WriteLine(grouping2.Key);

                            foreach (SpellingFixResult result in grouping2.OrderBy(f => f.LineNumber))
                            {
                                writer.Write("    ");

                                if (result.LineNumber != null)
                                {
                                    writer.Write(result.LineNumber.Value);
                                    writer.Write(" ");
                                }

                                int endIndex = result.Index + result.Length;

                                writer.Write(result.Input.AsSpan(result.LineStartIndex, result.Index - result.LineStartIndex));
                                writer.Write(">>>");
                                writer.Write(result.Value, Colors.Match);
                                writer.Write("<<<");
                                writer.WriteLine(result.Input.AsSpan(endIndex, result.LineEndIndex - endIndex));
                            }
                        }
                    }
                }
            }
        }
#endif

    }
}
