// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Orang.Syntax;
using Orang.Text.RegularExpressions;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class ListPatternsCommand : AbstractCommand<ListPatternsCommandOptions>
    {
        public ListPatternsCommand(ListPatternsCommandOptions options) : base(options)
        {
        }

        protected override CommandResult ExecuteCore(CancellationToken cancellationToken = default)
        {
            if (Options.Value != null)
            {
                return ListPatterns(Options.Value.Value);
            }
            else
            {
                return ListPatterns();
            }
        }

        private CommandResult ListPatterns()
        {
            IEnumerable<SyntaxItem> items = SyntaxItems.Load();

            string? filter = Options.Filter;

            ImmutableArray<SyntaxSection> sections = Options.Sections;

            if (!string.IsNullOrEmpty(filter))
            {
                items = items.Where(f => f.Text.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || f.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (sections.Any())
            {
                items = items.Join(sections, f => f.Section, f => f, (f, _) => f);
            }
            else
            {
                items = items.Where(
                    f => f.Section != SyntaxSection.GeneralCategories && f.Section != SyntaxSection.NamedBlocks);
            }

            List<SyntaxItem> list;
            if (sections.Any())
            {
                list = items.GroupBy(f => f.Section).OrderBy(f => sections.IndexOf(f.Key)).SelectMany(f => f).ToList();
            }
            else
            {
                list = items.GroupBy(f => f.Section).OrderBy(f => f.Key).SelectMany(f => f).ToList();
            }

            if (list.Count > 0)
            {
                int width = list.Max(f => f.Text.Length - f.Text.Count(ch => ch == '%'));

                foreach (IGrouping<SyntaxSection, SyntaxItem> grouping in list.GroupBy(f => f.Section))
                {
                    WriteLine();
                    WriteLine(TextHelpers.SplitCamelCase(grouping.Key.ToString()).ToUpper());

                    foreach (SyntaxItem item in grouping)
                    {
                        Write(" ");

                        string text = item.Text;
                        int length = text.Length;
                        int prevIndex = 0;

                        for (int i = 0; i < length; i++)
                        {
                            if (text[i] == '%')
                            {
                                Write(text, prevIndex, i - prevIndex, Colors.Syntax);

                                int j = i + 1;

                                while (j < length
                                    && text[j] != '%')
                                {
                                    j++;
                                }

                                Write(text, i + 1, j - i - 1);

                                i = j;
                                prevIndex = j + 1;
                            }
                        }

                        Write(text, prevIndex, length - prevIndex, Colors.Syntax);
                        Write(' ', width - text.Length + text.Count(ch => ch == '%'));
                        Write(" ");
                        WriteLine(item.Description);
                    }
                }

                return CommandResult.Success;
            }
            else
            {
                WriteLine("No syntax found");
                return CommandResult.NoMatch;
            }
        }

        private CommandResult ListPatterns(char ch)
        {
            var rows = new List<(string name, string description)>();

            if (ch >= 0 && ch <= 0x7F)
                rows.Add(("Name", TextHelpers.SplitCamelCase(((AsciiChar)ch).ToString())));

            int charCode = ch;

            rows.Add(("Decimal", charCode.ToString(CultureInfo.InvariantCulture)));
            rows.Add(("Hexadecimal", charCode.ToString("X", CultureInfo.InvariantCulture)));

            List<PatternInfo> patterns = GetPatterns(ch).ToList();

            int width = Math.Max(rows.Max(f => f.name.Length), patterns.Max(f => f.Pattern.Length));

            WriteLine();

            foreach ((string name, string description) in rows)
                WriteRow(name, description);

            WriteLine();

            if (patterns.Count > 0)
            {
                WriteRow("PATTERN", "DESCRIPTION");

                foreach (PatternInfo item in patterns)
                    WriteRow(item.Pattern, item.Description, Colors.Syntax);

                return CommandResult.Success;
            }
            else
            {
                WriteLine("No syntax found");
                return CommandResult.NoMatch;
            }

            void WriteRow(
                string value1,
                string? value2,
                in ConsoleColors colors1 = default,
                in ConsoleColors colors2 = default)
            {
                Write(value1, colors1);
                Write(' ', width - value1.Length + 1);
                WriteLine(value2 ?? "-", colors2);
            }
        }

        private IEnumerable<PatternInfo> GetPatterns(char ch)
        {
            IEnumerable<PatternInfo> patterns = GetPatterns(
                ch,
                inCharGroup: Options.InCharGroup,
                options: Options.RegexOptions);

            string? filter = Options.Filter;

            if (!string.IsNullOrEmpty(filter))
            {
                patterns = patterns.Where(f => f.Pattern.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || f.Description?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            ImmutableArray<SyntaxSection> sections = Options.Sections;

            if (sections.Any())
                patterns = patterns.Join(sections, f => f.Section, f => f, (f, _) => f);

            return patterns;
        }

        private IEnumerable<PatternInfo> GetPatterns(
            int charCode,
            bool inCharGroup = false,
            RegexOptions options = RegexOptions.None)
        {
            string s = ((char)charCode).ToString();

            if (charCode <= 0xFF)
            {
                switch (RegexEscape.GetEscapeMode((char)charCode, inCharGroup))
                {
                    case CharEscapeMode.Backslash:
                        {
                            yield return new PatternInfo(
                                @"\" + ((char)charCode).ToString(),
                                SyntaxSection.CharacterEscapes,
                                "Escaped character");
                            break;
                        }
                    case CharEscapeMode.Bell:
                        {
                            yield return new PatternInfo(@"\a", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.CarriageReturn:
                        {
                            yield return new PatternInfo(@"\r", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.Escape:
                        {
                            yield return new PatternInfo(@"\e", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.FormFeed:
                        {
                            yield return new PatternInfo(@"\f", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.Linefeed:
                        {
                            yield return new PatternInfo(@"\n", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.Tab:
                        {
                            yield return new PatternInfo(@"\t", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.VerticalTab:
                        {
                            yield return new PatternInfo(@"\v", SyntaxSection.CharacterEscapes);
                            break;
                        }
                    case CharEscapeMode.None:
                        {
                            yield return new PatternInfo(((char)charCode).ToString(), SyntaxSection.None);
                            break;
                        }
                }

                if (inCharGroup && charCode == 8)
                    yield return new PatternInfo(@"\b", SyntaxSection.CharacterEscapes, "Escaped character");
            }

            if (Regex.IsMatch(s, @"\d", options))
            {
                yield return new PatternInfo(@"\d", SyntaxSection.CharacterClasses, "Digit character");
            }
            else
            {
                yield return new PatternInfo(@"\D", SyntaxSection.CharacterClasses, "Non-digit character");
            }

            if (Regex.IsMatch(s, @"\s", options))
            {
                yield return new PatternInfo(@"\s", SyntaxSection.CharacterClasses, "Whitespace character");
            }
            else
            {
                yield return new PatternInfo(@"\S", SyntaxSection.CharacterClasses, "Non-whitespace character");
            }

            if (Regex.IsMatch(s, @"\w", options))
            {
                yield return new PatternInfo(@"\w", SyntaxSection.CharacterClasses, "Word character");
            }
            else
            {
                yield return new PatternInfo(@"\W", SyntaxSection.CharacterClasses, "Non-word character");
            }

            foreach (SyntaxItem item in SyntaxItems.Values)
            {
                if (item.Section == SyntaxSection.GeneralCategories)
                {
                    string pattern = @"\p{" + item.Text + "}";

                    if (Regex.IsMatch(s, pattern, options))
                    {
                        yield return new PatternInfo(
                            pattern,
                            SyntaxSection.GeneralCategories,
                            $"Unicode category: {item.Description}");
                    }
                }
            }

            foreach (SyntaxItem item in SyntaxItems.Values)
            {
                if (item.Section == SyntaxSection.NamedBlocks)
                {
                    string pattern = @"\p{" + item.Text + "}";

                    if (Regex.IsMatch(s, pattern, options))
                    {
                        yield return new PatternInfo(
                            pattern,
                            SyntaxSection.NamedBlocks,
                            $"Unicode block: {item.Description}");
                        break;
                    }
                }
            }

            if (charCode <= 0xFF)
            {
                yield return new PatternInfo(
                    @"\u" + charCode.ToString("X4", CultureInfo.InvariantCulture),
                    SyntaxSection.CharacterEscapes,
                    "Unicode character (four hexadecimal digits)");

                yield return new PatternInfo(
                    @"\x" + charCode.ToString("X2", CultureInfo.InvariantCulture),
                    SyntaxSection.CharacterEscapes,
                    "ASCII character (two hexadecimal digits)");

                yield return new PatternInfo(
                    @"\" + Convert.ToString(charCode, 8).PadLeft(2, '0'),
                    SyntaxSection.CharacterEscapes,
                    "ASCII character (two or three octal digits)");

                if (charCode > 0 && charCode <= 0x1A)
                {
                    yield return new PatternInfo(
                        @"\c" + Convert.ToChar('a' + charCode - 1),
                        SyntaxSection.CharacterEscapes,
                        "ASCII control character");

                    yield return new PatternInfo(
                        @"\c" + Convert.ToChar('A' + charCode - 1),
                        SyntaxSection.CharacterEscapes,
                        "ASCII control character");
                }
            }
        }

        private class PatternInfo
        {
            public PatternInfo(string pattern, SyntaxSection section, string? description = null)
            {
                Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
                Section = section;
                Description = description;
            }

            public string Pattern { get; }

            public SyntaxSection Section { get; }

            public string? Description { get; }
        }

        private enum AsciiChar
        {
            Null = 0,
            StartOfHeading = 1,
            StartOfText = 2,
            EndOfText = 3,
            EndOfTransmission = 4,
            Enquiry = 5,
            Acknowledge = 6,
            Bell = 7,
            Backspace = 8,
            Tab = 9,
            Linefeed = 10,
            VerticalTab = 11,
            FormFeed = 12,
            CarriageReturn = 13,
            ShiftOut = 14,
            ShiftIn = 15,
            DataLinkEscape = 16,
            DeviceControlOne = 17,
            DeviceControlTwo = 18,
            DeviceControlThree = 19,
            DeviceControlFour = 20,
            NegativeAcknowledge = 21,
            SynchronousIdle = 22,
            EndOfTransmissionBlock = 23,
            Cancel = 24,
            EndOfMedium = 25,
            Substitute = 26,
            Escape = 27,
            InformationSeparatorFour = 28,
            InformationSeparatorThree = 29,
            InformationSeparatorTwo = 30,
            InformationSeparatorOne = 31,
            Space = 32,
            ExclamationMark = 33,
            QuoteMark = 34,
            NumberSign = 35,
            Dollar = 36,
            Percent = 37,
            Ampersand = 38,
            Apostrophe = 39,
            LeftParenthesis = 40,
            RightParenthesis = 41,
            Asterisk = 42,
            Plus = 43,
            Comma = 44,
            Hyphen = 45,
            Dot = 46,
            Slash = 47,
            DigitZero = 48,
            DigitOne = 49,
            DigitTwo = 50,
            DigitThree = 51,
            DigitFour = 52,
            DigitFive = 53,
            DigitSix = 54,
            DigitSeven = 55,
            DigitEight = 56,
            DigitNine = 57,
            Colon = 58,
            Semicolon = 59,
            LeftAngleBracket = 60,
            EqualsSign = 61,
            RightAngleBracket = 62,
            QuestionMark = 63,
            AtSign = 64,
            CapitalLetterA = 65,
            CapitalLetterB = 66,
            CapitalLetterC = 67,
            CapitalLetterD = 68,
            CapitalLetterE = 69,
            CapitalLetterF = 70,
            CapitalLetterG = 71,
            CapitalLetterH = 72,
            CapitalLetterI = 73,
            CapitalLetterJ = 74,
            CapitalLetterK = 75,
            CapitalLetterL = 76,
            CapitalLetterM = 77,
            CapitalLetterN = 78,
            CapitalLetterO = 79,
            CapitalLetterP = 80,
            CapitalLetterQ = 81,
            CapitalLetterR = 82,
            CapitalLetterS = 83,
            CapitalLetterT = 84,
            CapitalLetterU = 85,
            CapitalLetterV = 86,
            CapitalLetterW = 87,
            CapitalLetterX = 88,
            CapitalLetterY = 89,
            CapitalLetterZ = 90,
            LeftSquareBracket = 91,
            Backslash = 92,
            RightSquareBracket = 93,
            CircumflexAccent = 94,
            Underscore = 95,
            GraveAccent = 96,
            SmallLetterA = 97,
            SmallLetterB = 98,
            SmallLetterC = 99,
            SmallLetterD = 100,
            SmallLetterE = 101,
            SmallLetterF = 102,
            SmallLetterG = 103,
            SmallLetterH = 104,
            SmallLetterI = 105,
            SmallLetterJ = 106,
            SmallLetterK = 107,
            SmallLetterL = 108,
            SmallLetterM = 109,
            SmallLetterN = 110,
            SmallLetterO = 111,
            SmallLetterP = 112,
            SmallLetterQ = 113,
            SmallLetterR = 114,
            SmallLetterS = 115,
            SmallLetterT = 116,
            SmallLetterU = 117,
            SmallLetterV = 118,
            SmallLetterW = 119,
            SmallLetterX = 120,
            SmallLetterY = 121,
            SmallLetterZ = 122,
            LeftCurlyBracket = 123,
            VerticalBar = 124,
            RightCurlyBracket = 125,
            Tilde = 126,
            Delete = 127,
        }
    }
}
