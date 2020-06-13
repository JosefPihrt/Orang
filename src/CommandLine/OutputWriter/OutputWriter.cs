// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Orang.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal class OutputWriter
    {
        public OutputWriter(HighlightOptions highlightOptions)
        {
            HighlightOptions = highlightOptions;
        }

        public HighlightOptions HighlightOptions { get; }

        public bool HighlightSplit => (HighlightOptions & HighlightOptions.Split) != 0;

        public bool HighlightBoundary => (HighlightOptions & HighlightOptions.Boundary) != 0;

        public int WriteMatches(MatchData matchData, MatchCommandOptions options, in CancellationToken cancellationToken = default)
        {
            string input = options.Input;

            MatchItemCollection matchItems = matchData.Items;

            if (matchItems.Count == 0)
                return default;

            int groupNumber = options.Filter.GroupNumber;

            OutputSymbols symbols = OutputSymbols.Create(HighlightOptions);

            if (options.ContentDisplayStyle == ContentDisplayStyle.Value
                || options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail)
            {
                bool addDetails = options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail;
                MatchOutputInfo? outputInfo = null;
                string? indent = null;

                if (addDetails)
                {
                    outputInfo = MatchOutputInfo.Create(matchData, groupNumber);
                    indent = new string(' ', outputInfo.Width);
                }

                int count = 0;

                if (options.ModifyOptions.HasAnyFunction)
                {
                    using (IEnumerator<string> en = matchItems
                        .SelectMany(f => f.GroupItems)
                        .Where(f => (groupNumber >= 0) ? groupNumber == f.Number : true)
                        .OrderBy(f => f.Index)
                        .ThenBy(f => f.Number)
                        .SelectMany(f => f.CaptureItems)
                        .Select(f => f.Value)
                        .Modify(options.ModifyOptions)
                        .GetEnumerator())
                    {
                        if (en.MoveNext())
                        {
                            var valueWriter = new OutputValueWriter(indent, HighlightOptions);

                            while (true)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                valueWriter.WriteMatch(en.Current, symbols, (HighlightOptions & HighlightOptions.Match) != 0);

                                count++;

                                if (en.MoveNext())
                                {
                                    Write(options.Separator);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    return count;
                }
                else
                {
                    var valueWriter = new OutputValueWriter(indent, HighlightOptions);
                    bool addSeparator = false;
                    int captureCount = 0;

                    for (int i = 0; i < matchItems.Count; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        MatchItem matchItem = matchItems[i];
                        bool omitMatchInfo = false;

                        foreach (GroupItem groupItem in matchItem.GroupItems
                            .Where(f => (groupNumber >= 0) ? groupNumber == f.Number : true)
                            .OrderBy(f => f.Index)
                            .ThenBy(f => f.Number))
                        {
                            bool omitGroupInfo = false;

                            for (int j = 0; j < groupItem.CaptureItems.Count; j++)
                            {
                                CaptureItem captureItem = groupItem.CaptureItems[j];

                                if (addSeparator)
                                    Write((addDetails) ? Environment.NewLine : options.Separator);

                                if (addDetails)
                                {
                                    Write(outputInfo!.GetText(
                                        captureItem,
                                        i + 1,
                                        j + 1,
                                        omitMatchInfo,
                                        omitGroupInfo));
                                }

                                valueWriter.WriteMatch(captureItem.Value, symbols);

                                omitMatchInfo = true;
                                omitGroupInfo = true;
                                addSeparator = true;
                                captureCount++;
                            }
                        }
                    }

                    return captureCount;
                }
            }
            else if (options.ContentDisplayStyle == ContentDisplayStyle.AllLines)
            {
                bool addDetails = options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail;

                MatchOutputInfo? outputInfo = (addDetails) ? MatchOutputInfo.Create(matchData, groupNumber) : null;

                var valueWriter = new OutputValueWriter(null, HighlightOptions);

                int captureCount = 0;
                int lastPos = 0;

                foreach (MatchItem matchItem in matchData.Items)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    valueWriter.Write(input, lastPos, matchItem.Match.Index - lastPos, symbols: null);
                    valueWriter.WriteMatch(matchItem.Match.Value, symbols);

                    captureCount += matchItem.GroupItems
                        .Where(f => (groupNumber >= 0) ? groupNumber == f.Number : true)
                        .Sum(f => f.CaptureItems.Count);

                    lastPos = matchItem.EndIndex;
                }

                valueWriter.Write(input, lastPos, input.Length - lastPos, symbols: null);

                return captureCount;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public int WriteSplits(SplitData splitData, SplitCommandOptions options, in CancellationToken cancellationToken = default)
        {
            OutputSymbols symbols = OutputSymbols.Create(HighlightOptions);
            ConsoleColors splitColors = (HighlightSplit) ? Colors.Split : default;
            ConsoleColors boundaryColors = (HighlightBoundary) ? Colors.SplitBoundary : default;

            if (options.ContentDisplayStyle == ContentDisplayStyle.Value
                || options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail)
            {
                if (options.ModifyOptions.HasAnyFunction)
                {
                    int count = 0;

                    using (IEnumerator<string> en = splitData.Items
                        .Select(f => f.Value)
                        .Modify(options.ModifyOptions)
                        .GetEnumerator())
                    {
                        if (en.MoveNext())
                        {
                            var valueWriter = new OutputValueWriter(null, HighlightOptions);

                            while (true)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                valueWriter.WriteSplit(en.Current, symbols, splitColors, boundaryColors);

                                count++;

                                if (en.MoveNext())
                                {
                                    Write(options.Separator);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    return count;
                }
                else
                {
                    bool addDetails = options.ContentDisplayStyle == ContentDisplayStyle.ValueDetail;
                    SplitOutputInfo? outputInfo = null;
                    string? indent = null;

                    if (addDetails)
                    {
                        outputInfo = SplitOutputInfo.Create(splitData);
                        indent = new string(' ', outputInfo.Width);
                    }

                    using (IEnumerator<SplitItem> en = splitData.Items.GetEnumerator())
                    {
                        if (en.MoveNext())
                        {
                            var valueWriter = new OutputValueWriter(indent, HighlightOptions);

                            while (true)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                SplitItem item = en.Current;

                                if (addDetails)
                                    Write(outputInfo!.GetText(item));

                                valueWriter.WriteSplit(item.Value, symbols, splitColors, boundaryColors);

                                if (en.MoveNext())
                                {
                                    Write((addDetails) ? Environment.NewLine : options.Separator);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (options.ContentDisplayStyle == ContentDisplayStyle.AllLines)
            {
                var valueWriter = new OutputValueWriter(null, HighlightOptions);

                string input = splitData.Input;

                int lastPos = 0;

                foreach (SplitItem item in splitData.Items)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    valueWriter.Write(input, lastPos, item.Index - lastPos, symbols: null);

                    valueWriter.WriteSplit(item.Value, symbols, splitColors, boundaryColors);

                    lastPos = item.Index + item.Length;
                }

                valueWriter.Write(input, lastPos, input.Length - lastPos, symbols: null);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return splitData.Count;
        }

        private void Write(string value)
        {
            Logger.Write(value);
        }

        private class OutputValueWriter : ValueWriter
        {
            public OutputValueWriter(string? indent, HighlightOptions highlightOptions) : base(new ContentTextWriter(Verbosity.Minimal), indent, includeEndingIndent: false)
            {
                HighlightOptions = highlightOptions;
            }

            public HighlightOptions HighlightOptions { get; }

            public void WriteMatch(string value, OutputSymbols symbols)
            {
                WriteMatch(value, symbols, (HighlightOptions & HighlightOptions.Match) != 0);
            }

            public void WriteMatch(string value, OutputSymbols symbols, bool highlightMatch)
            {
                if (value.Length > 0)
                {
                    Write(
                        value,
                        symbols,
                        colors: (highlightMatch) ? Colors.Match : default,
                        boundaryColors: ((HighlightOptions & HighlightOptions.Boundary) != 0) ? Colors.MatchBoundary : default);
                }
                else if ((HighlightOptions & HighlightOptions.EmptyMatch) != 0)
                {
                    Write("|", Colors.EmptyMatch);
                }
            }

            public void WriteSplit(string value, OutputSymbols symbols, in ConsoleColors colors, in ConsoleColors boundaryColors)
            {
                if (value.Length > 0)
                {
                    Write(
                        value,
                        symbols,
                        colors: colors,
                        boundaryColors: boundaryColors);
                }
                else if ((HighlightOptions & HighlightOptions.EmptySplit) != 0)
                {
                    Write("|", Colors.EmptySplit);
                }
            }

            protected override void WriteEndOfLine(string value, int index, int endIndex, OutputSymbols symbols)
            {
                if (index < endIndex)
                {
                    WriteNewLine(value, index);
                    Write(Indent);
                }
                else if (symbols.Linefeed == null)
                {
                    WriteNewLine(value, index);
                }
            }
        }
    }
}
