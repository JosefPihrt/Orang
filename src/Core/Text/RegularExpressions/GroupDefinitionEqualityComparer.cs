// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Orang.Text.RegularExpressions
{
    internal abstract class GroupDefinitionEqualityComparer : EqualityComparer<GroupDefinition>
    {
        public static GroupDefinitionEqualityComparer ByNumber { get; } = new GroupDefinitionNumberEqualityComparer();

        public override abstract bool Equals(GroupDefinition x, GroupDefinition y);

        public override abstract int GetHashCode(GroupDefinition obj);

        private class GroupDefinitionNumberEqualityComparer : GroupDefinitionEqualityComparer
        {
            public override bool Equals(GroupDefinition x, GroupDefinition y)
            {
                return x.Number == y.Number;
            }

            public override int GetHashCode(GroupDefinition obj)
            {
                return obj.Number.GetHashCode();
            }
        }
    }
}
