// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.CommandLine
{
    [Flags]
    internal enum ReplacementOptions
    {
        None = 0,
        Literal = 1,
        FromFile = 1 << 1,
        Escape = 1 << 2,
    }
}
