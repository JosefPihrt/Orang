// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Orang.Runtime
{
    public static class Modifier
    {
        public static IEnumerable<string> Modify(IEnumerable<string> values)
        {
            return values.Select(f => Helpers.FirstCharToUpper(f));
        }
    }
}
