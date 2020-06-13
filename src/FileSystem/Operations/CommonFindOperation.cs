// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Orang.Operations
{
    internal abstract class CommonFindOperation : FileSystemOperation
    {
        protected CommonFindOperation()
        {
        }

        public int MaxTotalMatches { get; set; }

        public int MaxMatchesInFile { get; set; }

        protected MaxReason GetCaptures(
            Match match,
            int groupNumber,
            Func<Capture, bool>? predicate,
            List<Capture> captures)
        {
            int maxMatchesInFile = MaxMatchesInFile;
            int maxTotalMatches = MaxTotalMatches;

            int count = 0;

            if (maxMatchesInFile > 0)
            {
                if (maxTotalMatches > 0)
                {
                    maxTotalMatches -= Telemetry.MatchCount;
                    count = Math.Min(maxMatchesInFile, maxTotalMatches);
                }
                else
                {
                    count = maxMatchesInFile;
                }
            }
            else if (maxTotalMatches > 0)
            {
                maxTotalMatches -= Telemetry.MatchCount;
                count = maxTotalMatches;
            }

            Debug.Assert(count >= 0, count.ToString());

            MaxReason maxReason = CaptureFactory.GetCaptures(ref captures, match, groupNumber, count, predicate, CancellationToken);

            if ((maxReason == MaxReason.CountEqualsMax || maxReason == MaxReason.CountExceedsMax)
                && maxTotalMatches > 0
                && (maxMatchesInFile == 0 || maxTotalMatches <= maxMatchesInFile))
            {
                TerminationReason = FileSystem.TerminationReason.MaxReached;
            }

            return maxReason;
        }
    }
}
