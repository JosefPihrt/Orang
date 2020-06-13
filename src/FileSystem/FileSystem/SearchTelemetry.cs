// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.FileSystem
{
    //TODO: MatchingLineCount
    internal class SearchTelemetry
    {
        internal SearchTelemetry()
        {
        }

        public int MatchingFileCount { get; internal set; }

        public int MatchingDirectoryCount { get; internal set; }

        internal int MatchingFileDirectoryCount => MatchingFileCount + MatchingDirectoryCount;

        public int ProcessedFileCount { get; internal set; }

        public int ProcessedDirectoryCount { get; internal set; }

        public int MatchCount { get; internal set; }

        public int ProcessedMatchCount { get; internal set; }

        internal void IncrementMatchingCount(bool isDirectory)
        {
            if (isDirectory)
            {
                MatchingDirectoryCount++;
            }
            else
            {
                MatchingFileCount++;
            }
        }

        internal void IncrementProcessedCount(bool isDirectory)
        {
            if (isDirectory)
            {
                ProcessedDirectoryCount++;
            }
            else
            {
                ProcessedFileCount++;
            }
        }
    }
}
