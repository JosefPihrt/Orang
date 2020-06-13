// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Orang.Text.RegularExpressions
{
    internal class ReplaceItemCollection : ReadOnlyCollection<ReplaceItem>
    {
        internal ReplaceItemCollection(IList<ReplaceItem> list)
            : base(list)
        {
        }
    }
}
