// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    [Flags]
    internal enum ModifierOptions
    {
        None = 0,
        FromFile = 1,
        FromDll = 1 << 1,
    }
}
