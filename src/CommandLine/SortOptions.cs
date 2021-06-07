// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Orang.CommandLine
{
    internal class SortOptions
    {
        public SortOptions(ImmutableArray<SortDescriptor> descriptors, bool cultureInvariant = false, int maxCount = 0)
        {
            Descriptors = descriptors;
            CultureInvariant = cultureInvariant;
            MaxCount = maxCount;
        }

        public ImmutableArray<SortDescriptor> Descriptors { get; }

        public bool CultureInvariant { get; }

        public int MaxCount { get; }
    }
}
