// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;

namespace Orang.CommandLine
{
    internal static class DialogResultMap
    {
        private static ImmutableDictionary<string, DialogResult>? _all;
        private static ImmutableDictionary<string, DialogResult>? _yes_No_Cancel;
        private static ImmutableDictionary<string, DialogResult>? _yes_YesToAll_Cancel;

        public static ImmutableDictionary<string, DialogResult> All
        {
            get
            {
                if (_all == null)
                    Interlocked.CompareExchange(ref _all, Create(), null);

                return _all;

                static ImmutableDictionary<string, DialogResult> Create()
                {
                    ImmutableDictionary<string, DialogResult>.Builder builder
                        = ImmutableDictionary.CreateBuilder<string, DialogResult>();

                    builder.Add("y", DialogResult.Yes);
                    builder.Add("yes", DialogResult.Yes);
                    builder.Add("ya", DialogResult.YesToAll);
                    builder.Add("yes to all", DialogResult.YesToAll);
                    builder.Add("n", DialogResult.No);
                    builder.Add("no", DialogResult.No);
                    builder.Add("na", DialogResult.NoToAll);
                    builder.Add("no to all", DialogResult.NoToAll);
                    builder.Add("c", DialogResult.Cancel);
                    builder.Add("cancel", DialogResult.Cancel);

                    return builder.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public static ImmutableDictionary<string, DialogResult> Yes_No_Cancel
        {
            get
            {
                if (_yes_No_Cancel == null)
                    Interlocked.CompareExchange(ref _yes_No_Cancel, Create(), null);

                return _yes_No_Cancel;

                static ImmutableDictionary<string, DialogResult> Create()
                {
                    ImmutableDictionary<string, DialogResult>.Builder builder
                        = ImmutableDictionary.CreateBuilder<string, DialogResult>();

                    builder.Add("y", DialogResult.Yes);
                    builder.Add("yes", DialogResult.Yes);
                    builder.Add("n", DialogResult.No);
                    builder.Add("no", DialogResult.No);
                    builder.Add("c", DialogResult.Cancel);
                    builder.Add("cancel", DialogResult.Cancel);

                    return builder.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public static ImmutableDictionary<string, DialogResult> Yes_YesToAll_Cancel
        {
            get
            {
                if (_yes_YesToAll_Cancel == null)
                    Interlocked.CompareExchange(ref _yes_YesToAll_Cancel, Create(), null);

                return _yes_YesToAll_Cancel;

                static ImmutableDictionary<string, DialogResult> Create()
                {
                    ImmutableDictionary<string, DialogResult>.Builder builder
                        = ImmutableDictionary.CreateBuilder<string, DialogResult>();

                    builder.Add("y", DialogResult.Yes);
                    builder.Add("yes", DialogResult.Yes);
                    builder.Add("ya", DialogResult.YesToAll);
                    builder.Add("yes to all", DialogResult.YesToAll);
                    builder.Add("c", DialogResult.Cancel);
                    builder.Add("cancel", DialogResult.Cancel);

                    return builder.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
                }
            }
        }
    }
}
