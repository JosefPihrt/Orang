// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Orang.Text.RegularExpressions;

namespace Orang.FileSystem
{
    internal static class ReplaceHelpers
    {
        public static List<ReplaceItem> GetReplaceItems(
            Match match,
            ReplaceOptions replaceOptions,
            CancellationToken cancellationToken = default)
        {
            List<Match> matches = GetMatches(match);

            int offset = 0;
            List<ReplaceItem> replaceItems = ListCache<ReplaceItem>.GetInstance();

            if (replaceItems.Capacity < matches.Count)
                replaceItems.Capacity = matches.Count;

            foreach (Match match2 in matches)
            {
                string value = replaceOptions.Replace(match2);

                replaceItems.Add(new ReplaceItem(match2, value, match2.Index + offset));

                offset += value.Length - match2.Length;

                cancellationToken.ThrowIfCancellationRequested();
            }

            ListCache<Match>.Free(matches);

            return replaceItems;

            static List<Match> GetMatches(Match match)
            {
                List<Match> matches = ListCache<Match>.GetInstance();

                do
                {
                    matches.Add(match);

                    match = match.NextMatch();

                } while (match.Success);

                if (matches.Count > 1
                    && matches[0].Index > matches[1].Index)
                {
                    matches.Reverse();
                }

                return matches;
            }
        }

        public static List<ReplaceItem> GetReplaceItems(
            Match match,
            ReplaceOptions replaceOptions,
            Func<string, bool>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            List<Capture> captures = ListCache<Capture>.GetInstance();

            MaxReason _ = CaptureFactory.GetCaptures(
                ref captures,
                match,
                predicate: predicate,
                cancellationToken: cancellationToken);

            int offset = 0;
            List<ReplaceItem> replaceItems = ListCache<ReplaceItem>.GetInstance();

            if (replaceItems.Capacity < captures.Count)
                replaceItems.Capacity = captures.Count;

            foreach (Match match2 in captures)
            {
                string value = replaceOptions.Replace(match2);

                replaceItems.Add(new ReplaceItem(match2, value, match2.Index + offset));

                offset += value.Length - match2.Length;

                cancellationToken.ThrowIfCancellationRequested();
            }

            ListCache<Capture>.Free(captures);

            return replaceItems;
        }

        public static string GetNewPath(FileMatch fileMatch, List<ReplaceItem> replaceItems)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            string path = fileMatch.Path;
            FileNameSpan span = fileMatch.NameSpan;

            sb.Append(path, 0, span.Start);

            int lastPos = span.Start;

            foreach (ReplaceItem item in replaceItems)
            {
                Match match = item.Match;

                sb.Append(path, lastPos, span.Start + match.Index - lastPos);
                sb.Append(item.Value);

                lastPos = span.Start + match.Index + match.Length;
            }

            sb.Append(path, lastPos, path.Length - lastPos);

            return StringBuilderCache.GetStringAndFree(sb);
        }
    }
}
