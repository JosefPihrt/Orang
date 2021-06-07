// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.CommandLine
{
    [Flags]
    internal enum SortFlags
    {
        None = 0,
        Ascending = 1,
        Descending = 1 << 1,
        Name = 1 << 2,
        CreationTime = 1 << 3,
        ModifiedTime = 1 << 4,
        Size = 1 << 5,
        CultureInvariant = 1 << 6,
    }
}
