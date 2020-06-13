// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal abstract class CommonRegexCommandOptions : AbstractCommandOptions
    {
        internal CommonRegexCommandOptions()
        {
        }

        public OutputDisplayFormat Format { get; internal set; } = null!;

        public bool IncludeSummary => Format.Includes(DisplayParts.Summary);

        public bool IncludeCount => Format.Includes(DisplayParts.Count);

        public HighlightOptions HighlightOptions { get; internal set; }

        public bool HighlightMatch => (HighlightOptions & HighlightOptions.DefaultOrMatch) != 0;

        public bool HighlightReplacement => (HighlightOptions & HighlightOptions.Replacement) != 0;

        public bool HighlightBoundary => (HighlightOptions & HighlightOptions.Boundary) != 0;
    }
}
