// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.FileSystem
{
    // Stop, Throw
    public enum ConflictResolution
    {
        Overwrite = 0,
        Skip = 1,
        Ask = 2,
        Suffix = 3,
    }
}
