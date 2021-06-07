// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Orang.Text.RegularExpressions;

namespace Orang
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ReplaceOptions : IReplacer
    {
        public static ReplaceOptions Empty { get; } = new ReplaceOptions("");

        public ReplaceOptions(
            string? replacement,
            ReplaceFunctions functions = ReplaceFunctions.None,
            bool cultureInvariant = false)
        {
            Replacement = replacement ?? "";
            Functions = functions;
            CultureInvariant = cultureInvariant;
        }

        public ReplaceOptions(
            MatchEvaluator matchEvaluator,
            ReplaceFunctions functions = ReplaceFunctions.None,
            bool cultureInvariant = false)
        {
            MatchEvaluator = matchEvaluator ?? throw new ArgumentNullException(nameof(matchEvaluator));
            Functions = functions;
            CultureInvariant = cultureInvariant;
        }

        public string? Replacement { get; }

        public MatchEvaluator? MatchEvaluator { get; }

        public ReplaceFunctions Functions { get; }

        public bool CultureInvariant { get; }

        internal CultureInfo CultureInfo => (CultureInvariant) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => (Replacement != null) ? $"{Replacement}" : $"{MatchEvaluator!.Method}";

        public string Replace(ICapture capture)
        {
            var regexCapture = (RegexCapture)capture;

            var match = (Match)regexCapture.Capture;

            return Replace(match);
        }

        public string Replace(Match match)
        {
            string s = (MatchEvaluator != null) ? MatchEvaluator(match) : match.Result(Replacement);

            if (Functions != ReplaceFunctions.None)
            {
                if ((Functions & ReplaceFunctions.TrimStart) != 0)
                {
                    if ((Functions & ReplaceFunctions.TrimEnd) != 0)
                    {
                        s = s.Trim();
                    }
                    else
                    {
                        s = s.TrimStart();
                    }
                }
                else if ((Functions & ReplaceFunctions.TrimEnd) != 0)
                {
                    s = s.TrimEnd();
                }

                if ((Functions & ReplaceFunctions.ToLower) != 0)
                {
                    s = s.ToLower(CultureInfo);
                }
                else if ((Functions & ReplaceFunctions.ToUpper) != 0)
                {
                    s = s.ToUpper(CultureInfo);
                }
            }

            return s;
        }
    }
}
