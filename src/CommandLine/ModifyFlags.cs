// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.CommandLine
{
    [Flags]
    internal enum ModifyFlags
    {
        None = 0,
        Distinct = 1,
        Ascending = 1 << 1,
        Descending = 1 << 2,
        Except = 1 << 3,
        Intersect = 1 << 4,
        GroupBy = 1 << 5,
        Except_Intersect_GroupBy = Except | Intersect | GroupBy,
        RemoveEmpty = 1 << 6,
        RemoveWhiteSpace = 1 << 7,
        TrimStart = 1 << 8,
        TrimEnd = 1 << 9,
        Trim = TrimStart | TrimEnd,
        ToUpper = 1 << 10,
        ToLower = 1 << 11,
        Aggregate = 1 << 12,
        IgnoreCase = 1 << 13,
        CultureInvariant = 1 << 14,
        AggregateOnly = 1 << 15,
    }
}
