// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orang
{
    internal class ModifyOptions
    {
        public static ModifyOptions Default { get; } = new ModifyOptions();

        public ModifyOptions(
            ModifyFunctions functions = ModifyFunctions.None,
            bool aggregate = false,
            bool ignoreCase = false,
            bool cultureInvariant = false,
            ValueSortProperty sortProperty = ValueSortProperty.None,
            EnumerableModifier<string>? modifier = null)
        {
            Functions = functions;
            Aggregate = aggregate;
            IgnoreCase = ignoreCase;
            CultureInvariant = cultureInvariant;
            SortProperty = sortProperty;
            Modifier = modifier;
        }

        public ModifyFunctions Functions { get; }

        public bool Aggregate { get; }

        public bool IgnoreCase { get; }

        public bool CultureInvariant { get; }

        public ValueSortProperty SortProperty { get; }

        public EnumerableModifier<string>? Modifier { get; }

        public StringComparer StringComparer
        {
            get
            {
                if (IgnoreCase)
                {
                    return (CultureInvariant)
                        ? StringComparer.InvariantCultureIgnoreCase
                        : StringComparer.CurrentCultureIgnoreCase;
                }
                else
                {
                    return (CultureInvariant)
                        ? StringComparer.InvariantCulture
                        : StringComparer.CurrentCulture;
                }
            }
        }

        public bool HasAnyFunction
        {
            get
            {
                return Functions != ModifyFunctions.None
                    || Modifier != null;
            }
        }

        public CultureInfo CultureInfo => (CultureInvariant) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

        public bool HasFunction(ModifyFunctions functions) => (Functions & functions) != 0;

        public IEnumerable<string> Modify(IEnumerable<string> values, ModifyFunctions? filter = null)
        {
            ModifyFunctions functions = Functions;

            if (filter != null)
                functions &= filter.Value;

            StringComparer comparer = StringComparer;

            bool trimStart = (functions & ModifyFunctions.TrimStart) != 0;
            bool trimEnd = (functions & ModifyFunctions.TrimEnd) != 0;

            if (trimStart && trimEnd)
            {
                values = values.Select(f => f.Trim());
            }
            else if (trimStart)
            {
                values = values.Select(f => f.TrimStart());
            }
            else if (trimEnd)
            {
                values = values.Select(f => f.TrimEnd());
            }

            if ((functions & ModifyFunctions.RemoveEmpty) != 0)
                values = values.Where(f => !string.IsNullOrEmpty(f));

            if ((functions & ModifyFunctions.RemoveWhiteSpace) != 0)
                values = values.Where(f => !string.IsNullOrWhiteSpace(f));

            if ((functions & ModifyFunctions.ToLower) != 0)
            {
                values = values.Select(f => f.ToLower(CultureInfo));
            }
            else if ((functions & ModifyFunctions.ToUpper) != 0)
            {
                values = values.Select(f => f.ToUpper(CultureInfo));
            }

            if ((functions & ModifyFunctions.Distinct) != 0)
                values = values.Distinct(comparer);

            if ((functions & ModifyFunctions.Sort) != 0)
            {
                if (SortProperty == ValueSortProperty.Length)
                {
                    values = values.OrderBy(f => f.Length);
                }
                else
                {
                    values = values.OrderBy(f => f, comparer);
                }
            }
            else if ((functions & ModifyFunctions.SortDescending) != 0)
            {
                if (SortProperty == ValueSortProperty.Length)
                {
                    values = values.OrderByDescending(f => f.Length);
                }
                else
                {
                    values = values.OrderByDescending(f => f, comparer);
                }
            }

            if (Modifier != null)
                values = Modifier(values);

            return values;
        }
    }
}
