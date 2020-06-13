// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Orang.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal class ContentWriterOptions
    {
        public ContentWriterOptions(
            OutputDisplayFormat format,
            GroupDefinition? groupDefinition = null,
            OutputSymbols? symbols = null,
            HighlightOptions highlightOptions = HighlightOptions.Match | HighlightOptions.Replacement,
            string? indent = null)
        {
            Format = format;
            GroupDefinition = groupDefinition;
            Symbols = symbols ?? OutputSymbols.Empty;
            HighlightOptions = highlightOptions;
            Indent = indent ?? OutputDisplayFormat.DefaultIndent;
        }

        public OutputDisplayFormat Format { get; }

        public GroupDefinition? GroupDefinition { get; }

        public OutputSymbols Symbols { get; }

        public HighlightOptions HighlightOptions { get; }

        public string Indent { get; }

        public int GroupNumber => GroupDefinition?.Number ?? -1;

        public string? GroupName => GroupDefinition?.Name;

        public bool HighlightMatch => (HighlightOptions & HighlightOptions.DefaultOrMatch) != 0;

        public bool HighlightReplacement => (HighlightOptions & HighlightOptions.Replacement) != 0;

        public bool HighlightEmptyMatch => (HighlightOptions & HighlightOptions.EmptyMatch) != 0;

        public bool HighlightEmptyReplacement => (HighlightOptions & HighlightOptions.EmptyReplacement) != 0;

        public bool HighlightBoundary => (HighlightOptions & HighlightOptions.Boundary) != 0;

        public bool IncludeLineNumber => Format.Includes(LineDisplayOptions.IncludeLineNumber);

        public bool TrimLine => Format.Includes(LineDisplayOptions.TrimLine);

        public string? Separator => Format.Separator;

        public int ContextBefore => Format.LineContext.Before;

        public int ContextAfter => Format.LineContext.After;
    }
}
