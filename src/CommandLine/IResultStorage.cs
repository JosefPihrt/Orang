// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal interface IResultStorage
    {
        void Add(string value);

        void Add(string value, int start, int length);

        int Count { get; }
    }
}
