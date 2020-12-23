// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Orang
{
    internal static class CaptureFactory
    {
        public static MaxReason GetCaptures(
            ref List<Capture> captures,
            Match match,
            int groupNumber = 0,
            int count = 0,
            Func<string, bool>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            MaxReason maxReason;
            if (groupNumber < 1)
            {
                maxReason = GetMatches(ref captures, match, count, predicate, cancellationToken);
            }
            else
            {
                maxReason = GetCapturesImpl(ref captures, match, groupNumber, count, predicate, cancellationToken);
            }

            if (captures.Count > 1
                && captures[0].Index > captures[1].Index)
            {
                captures.Reverse();
            }

            return maxReason;
        }

        private static MaxReason GetMatches(
            ref List<Capture> matches,
            Match match,
            int count,
            Func<string, bool>? predicate,
            CancellationToken cancellationToken)
        {
            do
            {
                matches.Add(match);

                if (predicate != null)
                {
                    Match m = match.NextMatch();

                    match = Match.Empty;

                    while (m.Success)
                    {
                        if (predicate(m.Value))
                        {
                            match = m;
                            break;
                        }

                        m = m.NextMatch();
                    }
                }
                else
                {
                    match = match.NextMatch();
                }

                if (matches.Count == count)
                {
                    return (match.Success)
                        ? MaxReason.CountExceedsMax
                        : MaxReason.CountEqualsMax;
                }

                cancellationToken.ThrowIfCancellationRequested();

            } while (match.Success);

            return MaxReason.None;
        }

        private static MaxReason GetCapturesImpl(
            ref List<Capture> captures,
            Match match,
            int groupNumber,
            int count,
            Func<string, bool>? predicate,
            CancellationToken cancellationToken)
        {
            Group group = match.Groups[groupNumber];

            do
            {
                Capture prevCapture = null!;

                foreach (Capture capture in group.Captures)
                {
                    if (prevCapture != null
                        && prevCapture.EndIndex() <= capture.EndIndex()
                        && prevCapture.Index >= capture.Index)
                    {
                        captures[^1] = capture;
                    }
                    else
                    {
                        captures.Add(capture);
                    }

                    if (captures.Count == count)
                        return (group.Success) ? MaxReason.CountExceedsMax : MaxReason.CountEqualsMax;

                    cancellationToken.ThrowIfCancellationRequested();

                    prevCapture = capture;
                }

                group = NextGroup();

            } while (group.Success);

            return MaxReason.None;

            Group NextGroup()
            {
                while (true)
                {
                    match = match.NextMatch();

                    if (!match.Success)
                        break;

                    Group g = match.Groups[groupNumber];

                    if (g.Success
                        && predicate?.Invoke(g.Value) != false)
                    {
                        return g;
                    }
                }

                return Match.Empty;
            }
        }
    }
}
