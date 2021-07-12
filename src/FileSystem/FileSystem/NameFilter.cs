// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Orang.Text.RegularExpressions;

namespace Orang.FileSystem
{
    //TODO: NameFilter : Filter
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NameFilter
    {
        public NameFilter(
            Filter name,
            FileNamePart part = FileNamePart.Name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Part = part;
        }

        public Filter Name { get; }

        public FileNamePart Part { get; }

        public bool IsNegative => Name.IsNegative;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Part}  {Name}";

        internal static NameFilter CreateFromDirectoryPath(string path, bool isNegative = false)
        {
            string pattern = RegexEscape.Escape(path);

            var options = RegexOptions.ExplicitCapture;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                pattern = Regex.Replace(pattern, @"(/|\\\\)", @"(/|\\)", options);

            pattern = $@"\A{pattern}(?=(/";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pattern += @"|\\)";
            }
            else
            {
                pattern += ")";
            }

            pattern += @"|\z)";

            if (!FileSystemHelpers.IsCaseSensitive)
                options |= RegexOptions.IgnoreCase;

            var filter = new Filter(pattern, options, isNegative: isNegative);

            return new NameFilter(filter, FileNamePart.FullName);
        }
    }
}
