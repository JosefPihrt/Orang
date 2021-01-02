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
        Except_Intersect = Except | Intersect,
        Group = 1 << 5,
        Except_Intersect_Group = Except_Intersect | Group,
        RemoveEmpty = 1 << 6,
        RemoveWhiteSpace = 1 << 7,
        TrimStart = 1 << 8,
        TrimEnd = 1 << 9,
        ToLower = 1 << 10,
        ToUpper = 1 << 11,
    }
}
