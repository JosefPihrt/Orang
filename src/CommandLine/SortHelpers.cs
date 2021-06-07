// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Orang.CommandLine
{
    internal static class SortHelpers
    {
        public static IEnumerable<SearchResult> SortResults(
            List<SearchResult> results,
            SortOptions sortOptions,
            PathDisplayStyle pathDisplayStyle)
        {
            ImmutableArray<SortDescriptor> descriptors = sortOptions.Descriptors;

            if (!descriptors.Any())
                return results;

            IComparer<SearchResult> comparer = GetComparer(descriptors[0].Property, pathDisplayStyle, sortOptions.CultureInvariant);

            if (descriptors.Length == 1)
            {
                if (descriptors[0].Direction == SortDirection.Ascending)
                {
                    results.Sort(comparer);
                }
                else
                {
                    results.Sort((x, y) => -comparer.Compare(x, y));
                }

                return results;
            }
            else
            {
                IOrderedEnumerable<SearchResult> sorted = (descriptors[0].Direction == SortDirection.Ascending)
                    ? results.OrderBy(f => f, comparer)
                    : results.OrderByDescending(f => f, comparer);

                for (int i = 1; i < descriptors.Length; i++)
                {
                    comparer = GetComparer(descriptors[i].Property, pathDisplayStyle, sortOptions.CultureInvariant);

                    sorted = (descriptors[i].Direction == SortDirection.Ascending)
                        ? sorted.ThenBy(f => f, comparer)
                        : sorted.ThenByDescending(f => f, comparer);
                }

                return sorted;
            }
        }

        private static SearchResultComparer GetComparer(
            SortProperty property,
            PathDisplayStyle pathDisplayStyle,
            bool cultureInvariant)
        {
            switch (property)
            {
                case SortProperty.Name:
                    if (pathDisplayStyle == PathDisplayStyle.Match)
                    {
                        return (cultureInvariant) ? SearchResultComparer.Match_CultureInvariant : SearchResultComparer.Match;
                    }
                    else
                    {
                        return (cultureInvariant) ? SearchResultComparer.Name_CultureInvariant : SearchResultComparer.Name;
                    }

                case SortProperty.CreationTime:
                    return SearchResultComparer.CreationTime;
                case SortProperty.ModifiedTime:
                    return SearchResultComparer.ModifiedTime;
                case SortProperty.Size:
                    return SearchResultComparer.Size;
                default:
                    throw new ArgumentException($"Unknown enum value '{property}'.", nameof(property));
            }
        }
    }
}
