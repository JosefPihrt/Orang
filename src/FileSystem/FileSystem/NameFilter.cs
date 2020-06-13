// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

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
    }
}
