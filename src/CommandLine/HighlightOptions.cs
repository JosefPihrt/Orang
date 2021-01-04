// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    [Flags]
    internal enum HighlightOptions
    {
        None = 0,
        Default = 1,
        Match = 1 << 1,
        DefaultOrMatch = Default | Match,
        Replacement = 1 << 2,
        Split = 1 << 3,
        EmptyMatch = 1 << 4,
        EmptyReplacement = 1 << 5,
        EmptySplit = 1 << 6,
        Empty = EmptyMatch | EmptyReplacement | EmptySplit,
        Boundary = 1 << 7,
        Tab = 1 << 8,
        CarriageReturn = 1 << 9,
        Linefeed = 1 << 10,
        Newline = CarriageReturn | Linefeed,
        Space = 1 << 11,
        Character = Tab | CarriageReturn | Linefeed | Space
    }
}
