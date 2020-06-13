// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.FileSystem
{
    [Flags]
    public enum FileCompareOptions
    {
        None = 0,
        Size = 1,
        ModifiedTime = 1 << 1,
        Attributes = 1 << 2,
        Content = 1 << 3,
    }
}
