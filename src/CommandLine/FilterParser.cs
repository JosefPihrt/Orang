// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Orang.CommandLine.Help;
using Orang.Expressions;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class FilterParser
    {
        public static bool TryParse(
            IEnumerable<string> values,
            string optionName,
            OptionValueProvider provider,
            out Filter? filter,
            bool allowNull = false,
            OptionValueProvider? namePartProvider = null,
            FileNamePart defaultNamePart = FileNamePart.Name,
            PatternOptions includedPatternOptions = PatternOptions.None)
        {
            return TryParse(
                values: values,
                optionName: optionName,
                provider: provider,
                filter: out filter,
                namePart: out _,
                allowNull: allowNull,
                namePartProvider: namePartProvider,
                defaultNamePart: defaultNamePart,
                includedPatternOptions: includedPatternOptions);
        }

        public static bool TryParse(
            IEnumerable<string> values,
            string optionName,
            OptionValueProvider provider,
            out Filter? filter,
            out FileNamePart namePart,
            bool allowNull = false,
            OptionValueProvider? namePartProvider = null,
            FileNamePart defaultNamePart = FileNamePart.Name,
            PatternOptions includedPatternOptions = PatternOptions.None)
        {
            filter = null;
            namePart = defaultNamePart;

            string? pattern = values.FirstOrDefault();

            if (pattern == null)
            {
                if (allowNull)
                {
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Option '{OptionNames.GetHelpText(optionName)}' is required.");
                }
            }

            TimeSpan matchTimeout = Regex.InfiniteMatchTimeout;
            string? groupName = null;
            string? separator = null;
            Func<Capture, bool>? predicate = null;

            List<string>? options = null;

            foreach (string option in values.Skip(1))
            {
                int index = option.IndexOf("=");

                if (index != -1)
                {
                    string key = option.Substring(0, index);

                    if (!provider.ContainsKeyOrShortKey(key))
                    {
                        WriteOptionError(option, optionName, provider);
                        return false;
                    }

                    string value = option.Substring(index + 1);

                    if (OptionValues.Group.IsKeyOrShortKey(key))
                    {
                        groupName = value;
                        continue;
                    }
                    else if (OptionValues.ListSeparator.IsKeyOrShortKey(key))
                    {
                        separator = value;
                        continue;
                    }
                    else if (OptionValues.Part.IsKeyOrShortKey(key))
                    {
                        if (!TryParseAsEnum(value, out namePart, provider: namePartProvider ?? OptionValueProviders.NamePartKindProvider))
                        {
                            WriteOptionValueError(value, OptionValues.Part, namePartProvider ?? OptionValueProviders.NamePartKindProvider);
                            return false;
                        }

                        continue;
                    }
                    else if (OptionValues.Timeout.IsKeyOrShortKey(key))
                    {
                        if (!TryParseMatchTimeout(value, out matchTimeout))
                        {
                            WriteOptionValueError(value, OptionValues.Timeout);
                            return false;
                        }

                        continue;
                    }
                }

                if (Expression.TryParse(option, out Expression? expression))
                {
                    if (OptionValues.Length.IsKeyOrShortKey(expression.Identifier)
                        && provider.ContainsKeyOrShortKey(expression.Identifier))
                    {
                        try
                        {
                            predicate = PredicateHelpers.GetLengthPredicate(expression);
                            continue;
                        }
                        catch (ArgumentException)
                        {
                            WriteOptionValueError(expression.Value, OptionValues.Length, HelpProvider.GetExpressionsText("  ", includeDate: false));
                            return false;
                        }
                    }
                    else
                    {
                        WriteOptionError(option, optionName, provider);
                        return false;
                    }
                }

                (options ??= new List<string>()).Add(option);
            }

            if (!TryParseRegexOptions(options, optionName, out RegexOptions regexOptions, out PatternOptions patternOptions, includedPatternOptions, provider))
                return false;

            switch (patternOptions & (PatternOptions.WholeWord | PatternOptions.WholeLine))
            {
                case PatternOptions.None:
                case PatternOptions.WholeWord:
                case PatternOptions.WholeLine:
                    {
                        break;
                    }
                default:
                    {
                        WriteError($"Values '{OptionValueProviders.PatternOptionsProvider.GetValue(nameof(PatternOptions.WholeWord)).HelpValue}' and '{OptionValueProviders.PatternOptionsProvider.GetValue(nameof(PatternOptions.WholeLine)).HelpValue}' cannot be combined.");
                        return false;
                    }
            }

            if ((patternOptions & PatternOptions.FromFile) != 0
                && !FileSystemHelpers.TryReadAllText(pattern, out pattern, ex => WriteError(ex)))
            {
                return false;
            }

            pattern = BuildPattern(pattern, patternOptions, separator);

            Regex? regex = null;

            try
            {
                regex = new Regex(pattern, regexOptions, matchTimeout);
            }
            catch (ArgumentException ex)
            {
                WriteError(ex, $"Could not parse regular expression: {ex.Message}");
                return false;
            }

            int groupIndex = -1;

            if (groupName != null)
            {
                groupIndex = regex.GroupNumberFromName(groupName);
                if (groupIndex == -1)
                {
                    string message = $"Group '{groupName}' does not exist.";

                    string[] groupNames = regex.GetGroupNames();

                    if (groupNames.Length > 1)
                        message += $" Existing group names: {TextHelpers.Join(", ", " and ", groupNames.Where(f => f != "0"))}.";

                    WriteError(message);
                    return false;
                }
            }

            filter = new Filter(
                regex,
                isNegative: (patternOptions & PatternOptions.Negative) != 0,
                groupNumber: groupIndex,
                predicate: predicate);

            return true;
        }

        private static string BuildPattern(
            string pattern,
            PatternOptions patternOptions,
            string? separator)
        {
            bool literal = (patternOptions & PatternOptions.Literal) != 0;

            if ((patternOptions & PatternOptions.List) != 0)
            {
                IEnumerable<string> values;

                if ((patternOptions & PatternOptions.FromFile) != 0
                    && separator == null)
                {
                    values = TextHelpers.ReadLines(pattern).Where(f => f.Length > 0);
                }
                else
                {
                    values = pattern.Split(separator ?? ",", StringSplitOptions.RemoveEmptyEntries);
                }

                pattern = JoinValues(values);
            }
            else if (literal)
            {
                pattern = RegexEscape.Escape(pattern);
            }

            if ((patternOptions & PatternOptions.WholeWord) != 0)
            {
                pattern = @"\b(?:" + pattern + @")\b";
            }
            else if ((patternOptions & PatternOptions.WholeLine) != 0)
            {
                pattern = @"(?:\A|(?<=\n))(?:" + pattern + @")(?:\z|(?=\r?\n))";
            }

            if ((patternOptions & PatternOptions.Equals) == PatternOptions.Equals)
            {
                return @"\A(?:" + pattern + @")\z";
            }
            else if ((patternOptions & PatternOptions.StartsWith) != 0)
            {
                return @"\A(?:" + pattern + ")";
            }
            else if ((patternOptions & PatternOptions.EndsWith) != 0)
            {
                return "(?:" + pattern + @")\z";
            }

            return pattern;

            string JoinValues(IEnumerable<string> values)
            {
                using (IEnumerator<string> en = values.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        string value = en.Current;

                        if (en.MoveNext())
                        {
                            StringBuilder sb = StringBuilderCache.GetInstance();

                            AppendValue(value, sb);

                            do
                            {
                                sb.Append("|");
                                AppendValue(en.Current, sb);

                            } while (en.MoveNext());

                            return StringBuilderCache.GetStringAndFree(sb);
                        }

                        return (literal) ? RegexEscape.Escape(value) : value;
                    }

                    return "";
                }
            }

            void AppendValue(string value, StringBuilder sb)
            {
                if (literal)
                {
                    sb.Append(RegexEscape.Escape(value));
                }
                else
                {
                    sb.Append("(?:");
                    sb.Append(value);
                    sb.Append(")");
                }
            }
        }

        private static bool TryParseRegexOptions(
            IEnumerable<string>? options,
            string optionsParameterName,
            out RegexOptions regexOptions,
            out PatternOptions patternOptions,
            PatternOptions includedPatternOptions = PatternOptions.None,
            OptionValueProvider? provider = null)
        {
            regexOptions = RegexOptions.None;

            if (!TryParseAsEnumFlags(options, optionsParameterName, out patternOptions, provider: provider ?? OptionValueProviders.PatternOptionsProvider))
                return false;

            Debug.Assert((patternOptions & (PatternOptions.CaseSensitive | PatternOptions.IgnoreCase)) != (PatternOptions.CaseSensitive | PatternOptions.IgnoreCase));

            if ((patternOptions & PatternOptions.CaseSensitive) != 0)
            {
                includedPatternOptions &= ~PatternOptions.IgnoreCase;
            }
            else if ((patternOptions & PatternOptions.IgnoreCase) != 0)
            {
                includedPatternOptions &= ~PatternOptions.CaseSensitive;
            }

            patternOptions |= includedPatternOptions;

            if ((patternOptions & PatternOptions.Compiled) != 0)
                regexOptions |= RegexOptions.Compiled;

            if ((patternOptions & PatternOptions.CultureInvariant) != 0)
                regexOptions |= RegexOptions.CultureInvariant;

            if ((patternOptions & PatternOptions.ECMAScript) != 0)
                regexOptions |= RegexOptions.ECMAScript;

            if ((patternOptions & PatternOptions.ExplicitCapture) != 0)
                regexOptions |= RegexOptions.ExplicitCapture;

            if ((patternOptions & PatternOptions.IgnoreCase) != 0)
                regexOptions |= RegexOptions.IgnoreCase;

            if ((patternOptions & PatternOptions.IgnorePatternWhitespace) != 0)
                regexOptions |= RegexOptions.IgnorePatternWhitespace;

            if ((patternOptions & PatternOptions.Multiline) != 0)
                regexOptions |= RegexOptions.Multiline;

            if ((patternOptions & PatternOptions.RightToLeft) != 0)
                regexOptions |= RegexOptions.RightToLeft;

            if ((patternOptions & PatternOptions.Singleline) != 0)
                regexOptions |= RegexOptions.Singleline;

            return true;
        }

        private static bool TryParseMatchTimeout(string value, out TimeSpan matchTimeout)
        {
            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int seconds))
            {
                try
                {
                    matchTimeout = TimeSpan.FromSeconds(seconds);
                    return true;
                }
                catch (Exception ex) when (ex is ArgumentException
                    || ex is OverflowException)
                {
                }
            }

            matchTimeout = default;
            return false;
        }
    }
}
