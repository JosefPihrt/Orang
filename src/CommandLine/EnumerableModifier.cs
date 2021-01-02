// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Orang
{
    internal delegate IEnumerable<T> EnumerableModifier<T>(IEnumerable<T> enumerable);
}