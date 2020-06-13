// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    [Flags]
    public enum ReplaceFunctions
    {
        None = 0,
        TrimStart = 1,
        TrimEnd = 1 << 1,
        Trim = TrimStart | TrimEnd,
        ToLower = 1 << 2,
        ToUpper = 1 << 3,
    }
}
