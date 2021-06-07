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

        public List<NewWord> NewWords { get; } = new List<NewWord>();

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
            if (newValue != null)
            {
                if (!string.Equals(capture.Value, newValue, StringComparison.Ordinal))
                {
                    var fix = new SpellingFix(newValue, (isUserInput) ? SpellingFixKind.User : SpellingFixKind.Predefined);

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
                    AddNewWord(input, (SpellingCapture)capture, lineNumber);
                }
            }
            else
            {
                AddNewWord(input, (SpellingCapture)capture, lineNumber);
            }
        }

        private void AddNewWord(string input, SpellingCapture capture, int? lineNumber = null)
        {
            int lineCharIndex = GetLineCharIndex(input, capture.Index);
            int startIndex = capture.Index - lineCharIndex;
            int endIndex = FindEndOfLine(input, capture.Index + capture.Length);

            var newWord = new NewWord(
                capture.Value,
                input[startIndex..endIndex],
                lineCharIndex,
                lineNumber,
                CurrentPath,
                capture.ContainingValue);

            NewWords.Add(newWord);

            static int GetLineCharIndex(string input, int index)
            {
                int count = 0;
                while (index > 0
                    && input[index - 1] != '\n')
                {
                    index--;
                    count++;
                }

                return count;
            }

            static int FindEndOfLine(string input, int index)
            {
                while (index < input.Length)
                {
                    if (input[index] == '\r'
                        || input[index] == '\n')
                    {
                        return index;
                    }

                    index++;
                }

                return input.Length;
            }
        }
    }
}
