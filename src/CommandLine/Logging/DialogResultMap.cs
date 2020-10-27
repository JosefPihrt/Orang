// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;

namespace Orang.CommandLine
{
    internal static class DialogResultMap
    {
        private static ImmutableDictionary<string, DialogResult>? _all;
        private static ImmutableDictionary<string, DialogResult>? _yesNoCancel;
        private static ImmutableDictionary<string, DialogResult>? _yesYesToAllCancel;

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

        public static ImmutableDictionary<string, DialogResult> YesNoCancel
        {
            get
            {
                if (_yesNoCancel == null)
                    Interlocked.CompareExchange(ref _yesNoCancel, Create(), null);

                return _yesNoCancel;

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

        public static ImmutableDictionary<string, DialogResult> YesYesToAllCancel
        {
            get
            {
                if (_yesYesToAllCancel == null)
                    Interlocked.CompareExchange(ref _yesYesToAllCancel, Create(), null);

                return _yesYesToAllCancel;

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
