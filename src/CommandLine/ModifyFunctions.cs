// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    [Flags]
    internal enum ModifyFunctions
    {
        None = 0,
        Distinct = 1,
        Sort = 1 << 1,
        SortDescending = 1 << 2,
        Enumerable = Distinct | Sort | SortDescending,
        Except = 1 << 3,
        Intersect = 1 << 4,
        ExceptIntersect = Except | Intersect,
        RemoveEmpty = 1 << 5,
        RemoveWhiteSpace = 1 << 6,
        TrimStart = 1 << 7,
        TrimEnd = 1 << 8,
        ToLower = 1 << 9,
        ToUpper = 1 << 10
    }
}
