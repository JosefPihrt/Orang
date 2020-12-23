// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Orang.CommandLine;

namespace Orang.Aggregation
{
    internal class ListResultStorage : IResultStorage
    {
        public ListResultStorage()
            : this(new List<string>())
        {
        }

        public ListResultStorage(List<string> list)
        {
            Values = list;
        }

        public List<string> Values { get; }

        public int Count => Values.Count;

        public void Add(string value)
        {
            Values.Add(value);
        }

        public void Add(string value, int start, int length)
        {
            Values.Add(value.Substring(start, length));
        }
    }
}
