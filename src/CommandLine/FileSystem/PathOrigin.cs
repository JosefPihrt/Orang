// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.FileSystem
{
    internal enum PathOrigin
    {
        None = 0,
        Argument = 1,
        File = 2,
        RedirectedInput = 3,
        CurrentDirectory = 4,
        Option = 5,
    }
}
