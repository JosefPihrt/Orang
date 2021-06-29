// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Orang.Spelling;

namespace Orang.CommandLine
{
    internal class SpellcheckState : IReplacer
    {
        public SpellcheckState(
            Spellchecker spellchecker,
            SpellingData data,
            IEnumerable<Filter>? filters = null)
        {
            Spellchecker = spellchecker;
            Data = data;
            Filters = filters?.ToImmutableArray() ?? ImmutableArray<Filter>.Empty;

            OriginalFixes = data.Fixes;
        }

        public Spellchecker Spellchecker { get; }

        public SpellingData Data { get; internal set; }

        public ImmutableArray<Filter> Filters { get; }

        internal FixList OriginalFixes { get; }

        public List<SpellingFixResult> Results { get; } = new List<SpellingFixResult>();

        public string? CurrentPath { get; set; }

        public string Replace(ICapture capture)
        {
            SpellingFix fix = GetFix(capture.Value);

            return fix.Value;
        }

        private SpellingFix GetFix(string value)
        {
            TextCasing textCasing = TextUtility.GetTextCasing(value);

            if (textCasing != TextCasing.Undefined
                && Data.Fixes.TryGetValue(value, out ImmutableHashSet<SpellingFix>? fixes))
            {
                SpellingFix fix = fixes.SingleOrDefault(
                    f => TextUtility.GetTextCasing(f.Value) != TextCasing.Undefined,
                    shouldThrow: false);

                if (!fix.IsDefault)
                    return fix.WithValue(TextUtility.SetTextCasing(fix.Value, textCasing));
            }

            return default;
        }

        public void ProcessReplacement(string input, ICapture capture, string? newValue, int? lineNumber = null, bool isUserInput = false)
        {
            SpellingFix fix = default;

            if (newValue != null)
            {
                if (!string.Equals(capture.Value, newValue, StringComparison.Ordinal))
                {
                    fix = new SpellingFix(newValue, (isUserInput) ? SpellingFixKind.User : SpellingFixKind.Predefined);

                    if (fix.Kind != SpellingFixKind.Predefined
                        && (fix.Kind != SpellingFixKind.User
                            || TextUtility.TextCasingEquals(capture.Value, fix.Value)))
                    {
                        Data = Data.AddFix(capture.Value, fix);
                    }

                    Data = Data.AddWord(newValue);
                }
                else
                {
                    Data = Data.AddIgnoredValue(capture.Value);
                }
            }

            var result = new SpellingFixResult(
                input,
                (SpellingCapture)capture,
                fix,
                lineNumber,
                CurrentPath);

            Results.Add(result);
        }
    }
}
