// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Orang.CommandLine
{
    internal abstract class SpellingFixResultEqualityComparer : IEqualityComparer<SpellingFixResult>
    {
        public static SpellingFixResultEqualityComparer ValueAndReplacement { get; } = new ValueAndFixedValueComparer();

        public abstract bool Equals(SpellingFixResult? x, SpellingFixResult? y);

        public abstract int GetHashCode(SpellingFixResult obj);

        private class ValueAndFixedValueComparer : SpellingFixResultEqualityComparer
        {
            public override bool Equals(SpellingFixResult? x, SpellingFixResult? y)
            {
                return StringComparer.CurrentCulture.Equals(x?.Value, y?.Value)
                    && StringComparer.CurrentCulture.Equals(x?.Replacement, y?.Replacement);
            }

            public override int GetHashCode(SpellingFixResult obj)
            {
                return HashCode.Combine(
                    StringComparer.CurrentCulture.GetHashCode(obj.Value),
                    StringComparer.CurrentCulture.GetHashCode(obj.Replacement));
            }
        }
    }
}
