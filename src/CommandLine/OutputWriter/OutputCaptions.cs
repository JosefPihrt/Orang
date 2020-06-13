// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang
{
    internal class OutputCaptions
    {
        public static OutputCaptions Default { get; } = new OutputCaptions(
            match: "Match",
            group: "Group",
            capture: "Capture",
            index: "Index",
            length: "Length",
            split: "Split",
            shortMatch: "M",
            shortGroup: "G",
            shortCapture: "C",
            shortIndex: "I",
            shortLength: "L",
            splitChar: "S");

        public OutputCaptions(
            string? match = null,
            string? group = null,
            string? capture = null,
            string? index = null,
            string? length = null,
            string? split = null,
            string? shortMatch = null,
            string? shortGroup = null,
            string? shortCapture = null,
            string? shortIndex = null,
            string? shortLength = null,
            string? splitChar = null)
        {
            Match = match ?? Default.Match;
            Group = group ?? Default.Group;
            Capture = capture ?? Default.Capture;
            Index = index ?? Default.Index;
            Length = length ?? Default.Length;
            Split = split ?? Default.Split;
            ShortMatch = shortMatch ?? Default.ShortMatch;
            ShortGroup = shortGroup ?? Default.ShortGroup;
            ShortCapture = shortCapture ?? Default.ShortCapture;
            ShortIndex = shortIndex ?? Default.ShortIndex;
            ShortLength = shortLength ?? Default.ShortLength;
            ShortSplit = splitChar ?? Default.ShortSplit;
        }

        public string Match { get; }

        public string Group { get; }

        public string Capture { get; }

        public string Index { get; }

        public string Length { get; }

        public string Split { get; }

        public string ShortMatch { get; }

        public string ShortGroup { get; }

        public string ShortCapture { get; }

        public string ShortIndex { get; }

        public string ShortLength { get; }

        public string ShortSplit { get; }
    }
}
