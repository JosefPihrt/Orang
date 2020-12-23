// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Orang.FileSystem;

namespace Orang.Aggregation
{
    internal abstract class StorageSectionComparer : IComparer<StorageSection>, IEqualityComparer<StorageSection>
    {
        public static StorageSectionComparer Path { get; } = new PathStorageSectionComparer();

        public abstract int Compare([AllowNull] StorageSection x, [AllowNull] StorageSection y);

        public abstract bool Equals([AllowNull] StorageSection x, [AllowNull] StorageSection y);

        public abstract int GetHashCode([DisallowNull] StorageSection obj);

        private class PathStorageSectionComparer : StorageSectionComparer
        {
            public override int Compare([AllowNull] StorageSection x, [AllowNull] StorageSection y)
            {
                return FileSystemHelpers.Comparer.Compare(x.FileMatch?.Path, y.FileMatch?.Path);
            }

            public override bool Equals([AllowNull] StorageSection x, [AllowNull] StorageSection y)
            {
                return FileSystemHelpers.Comparer.Equals(x.FileMatch?.Path, y.FileMatch?.Path);
            }

            public override int GetHashCode([DisallowNull] StorageSection obj)
            {
                string? path = obj.FileMatch?.Path;

                if (path == null)
                    return 0;

                return FileSystemHelpers.Comparer.GetHashCode(path);
            }
        }
    }
}
