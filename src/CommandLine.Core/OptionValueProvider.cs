// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orang
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class OptionValueProvider
    {
        private static readonly Regex _metaValueRegex = new Regex(@"\<\w+\>");

        public OptionValueProvider(string name, params OptionValue[] values)
        {
            Name = name;
            Values = values.ToImmutableArray();
        }

        public OptionValueProvider(string name, IEnumerable<OptionValue> values)
        {
            Name = name;
            Values = values.ToImmutableArray();
        }

        public string Name { get; }

        public ImmutableArray<OptionValue> Values { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name;

        public OptionValueProvider WithValues(params OptionValue[] values)
        {
            return WithValues(Name, values);
        }

        public OptionValueProvider WithValues(string name, params OptionValue[] values)
        {
            return new OptionValueProvider(name, Values.AddRange(values));
        }

        public OptionValueProvider WithoutValues(params OptionValue[] values)
        {
            return WithoutValues(Name, values);
        }

        public OptionValueProvider WithoutValues(string name, params OptionValue[] values)
        {
            ImmutableArray<OptionValue>.Builder builder = ImmutableArray.CreateBuilder<OptionValue>();

            foreach (OptionValue value in Values)
            {
                if (Array.IndexOf(values, value) == -1)
                    builder.Add(value);
            }

            return new OptionValueProvider(name, builder);
        }

        public bool ContainsKeyOrShortKey(string value)
        {
            foreach (OptionValue optionValue in Values)
            {
                if (optionValue.Kind == OptionValueKind.KeyValuePair)
                {
                    var keyValue = (KeyValuePairOptionValue)optionValue;

                    if (keyValue.IsKeyOrShortKey(value))
                        return true;
                }
            }

            return false;
        }

        public bool TryParseEnum<TEnum>(string? value, out TEnum result) where TEnum : struct
        {
            foreach (OptionValue optionValue in Values)
            {
                if (optionValue is SimpleOptionValue enumOptionValue)
                {
                    if (enumOptionValue.Value == value
                        || enumOptionValue.ShortValue == value)
                    {
                        return Enum.TryParse(optionValue.Name, out result);
                    }
                }
            }

            result = default;
            return false;
        }

        public OptionValue GetValue(string name)
        {
            foreach (OptionValue value in Values)
            {
                if (string.Equals(value.Name, name, StringComparison.Ordinal))
                    return value;
            }

            throw new ArgumentException("", nameof(name));
        }

        public static IEnumerable<OptionValueProvider> GetProviders(IEnumerable<CommandOption> options, IEnumerable<OptionValueProvider> providers)
        {
            IEnumerable<string?> metaValues = options
                .SelectMany(f => _metaValueRegex.Matches(f.Description).Cast<Match>().Select(m => m.Value))
                .Concat(options.Select(f => f.MetaValue))
                .Distinct();

            ImmutableArray<OptionValueProvider> providers2 = metaValues
                .Join(providers, f => f, f => f.Name, (_, f) => f)
                .ToImmutableArray();

            return providers2
                .SelectMany(f => f.Values)
                .OfType<KeyValuePairOptionValue>()
                .Select(f => f.Value)
                .Distinct()
                .Join(providers, f => f, f => f.Name, (_, f) => f)
                .Concat(providers2)
                .Distinct()
                .OrderBy(f => f.Name);
        }
    }
}
