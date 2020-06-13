// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.FileSystem
{
    public enum OperationKind : byte
    {
        Replace = 0,
        Rename = 1,
        Delete = 2,
        Copy = 3,
        Move = 4,
    }
}
