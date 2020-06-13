// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1223

namespace Orang.FileSystem
{
    public class FilePropertyFilter
    {
        public FilePropertyFilter(
            Func<DateTime, bool>? creationTimePredicate = null,
            Func<DateTime, bool>? modifiedTimePredicate = null,
            Func<long, bool>? sizePredicate = null)
        {
            CreationTimePredicate = creationTimePredicate;
            ModifiedTimePredicate = modifiedTimePredicate;
            SizePredicate = sizePredicate;
        }

        public Func<DateTime, bool>? CreationTimePredicate { get; }

        public Func<DateTime, bool>? ModifiedTimePredicate { get; }

        public Func<long, bool>? SizePredicate { get; }
    }
}
