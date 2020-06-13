// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal abstract class SearchResultComparer : Comparer<SearchResult>
    {
        public static SearchResultComparer Name { get; } = new SearchResultNameComparer();

        public static SearchResultComparer CreationTime { get; } = new SearchResultCreationTimeComparer();

        public static SearchResultComparer ModifiedTime { get; } = new SearchResultModifiedTimeComparer();

        public static SearchResultComparer Size { get; } = new SearchResultSizeComparer();

        public static SearchResultComparer Match { get; } = new SearchResultMatchComparer();

        public override abstract int Compare(SearchResult x, SearchResult y);

        private class SearchResultNameComparer : SearchResultComparer
        {
            public override int Compare(SearchResult x, SearchResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                string path1 = x.Path;
                string path2 = y.Path;

                int i1 = -1;
                int i2 = -1;

                while (true)
                {
                    int l1 = FileSystemHelpers.IndexOfDirectorySeparator(path1, ++i1) - i1;
                    int l2 = FileSystemHelpers.IndexOfDirectorySeparator(path2, ++i2) - i2;

                    bool isLast1 = i1 + l1 == path1.Length;
                    bool isLast2 = i2 + l2 == path2.Length;

                    if (isLast1)
                    {
                        if (isLast2)
                        {
                            if (x.IsDirectory)
                            {
                                if (!y.IsDirectory)
                                    return -1;
                            }
                            else if (y.IsDirectory)
                            {
                                return 1;
                            }
                        }
                        else if (!x.IsDirectory)
                        {
                            return 1;
                        }
                    }
                    else if (isLast2
                        && !y.IsDirectory)
                    {
                        return -1;
                    }

                    int diff = string.Compare(path1, i1, path2, i2, Math.Min(l1, l2), StringComparison.CurrentCulture);

                    if (diff != 0)
                        return diff;

                    if (l1 != l2)
                    {
                        diff = l1 - l2;

                        if (diff != 0)
                            return diff;
                    }

                    i1 += l1;
                    i2 += l2;

                    if (isLast1)
                        return (isLast2) ? 0 : -1;

                    if (isLast2)
                        return 1;
                }
            }
        }

        private class SearchResultCreationTimeComparer : SearchResultComparer
        {
            public override int Compare(SearchResult x, SearchResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                return x.FileSystemInfo.CreationTime.CompareTo(y.FileSystemInfo.CreationTime);
            }
        }

        private class SearchResultModifiedTimeComparer : SearchResultComparer
        {
            public override int Compare(SearchResult x, SearchResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                return x.FileSystemInfo.LastWriteTime.CompareTo(y.FileSystemInfo.LastWriteTime);
            }
        }

        private class SearchResultSizeComparer : SearchResultComparer
        {
            public override int Compare(SearchResult x, SearchResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                int diff = x.GetSize().CompareTo(y.GetSize());

                if (diff != 0)
                    return diff;

                return Name.Compare(x, y);
            }
        }

        private class SearchResultMatchComparer : SearchResultComparer
        {
            public override int Compare(SearchResult x, SearchResult y)
            {
                return string.Compare(x.FileMatch.NameMatch!.Value, y.FileMatch.NameMatch!.Value, StringComparison.CurrentCulture);
            }
        }
    }
}
