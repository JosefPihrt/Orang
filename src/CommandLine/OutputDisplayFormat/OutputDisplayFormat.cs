// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Orang.CommandLine
{
    internal class OutputDisplayFormat
    {
        internal static readonly string DefaultIndent = "  ";

        public OutputDisplayFormat(
            ContentDisplayStyle contentDisplayStyle,
            PathDisplayStyle pathDisplayStyle = PathDisplayStyle.Full,
            LineDisplayOptions lineOptions = LineDisplayOptions.None,
            LineContext lineContext = default,
            DisplayParts displayParts = DisplayParts.None,
            IEnumerable<FileProperty>? fileProperties = null,
            string? indent = null,
            string? separator = null,
            bool includeBaseDirectory = false)
        {
            ContentDisplayStyle = contentDisplayStyle;
            PathDisplayStyle = pathDisplayStyle;
            LineOptions = lineOptions;
            LineContext = lineContext;
            DisplayParts = displayParts;
            FileProperties = fileProperties?.ToImmutableArray() ?? ImmutableArray<FileProperty>.Empty;
            Indent = indent ?? DefaultIndent;
            Separator = separator;
            IncludeBaseDirectory = includeBaseDirectory;
        }

        public ContentDisplayStyle ContentDisplayStyle { get; }

        public PathDisplayStyle PathDisplayStyle { get; }

        public LineDisplayOptions LineOptions { get; }

        public LineContext LineContext { get; }

        public DisplayParts DisplayParts { get; }

        public ImmutableArray<FileProperty> FileProperties { get; }

        public string Indent { get; }

        public string? Separator { get; }

        public bool IncludeBaseDirectory { get; }

        public bool Includes(LineDisplayOptions options) => (LineOptions & options) == options;

        public bool Includes(DisplayParts parts) => (DisplayParts & parts) == parts;
    }
}
