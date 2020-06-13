// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Orang.CommandLine
{
    internal static class OptionNames
    {
        public const string Ask = "ask";
        public const string Attributes = "attributes";
        public const string AttributesToSkip = "attributes-to-skip";
        public const string Compare = "compare";
        public const string Conflict = "conflict";
        public const string Content = "content";
        public const string ContentOnly = "content-only";
        public const string Display = "display";
        public const string DryRun = "dry-run";
        public const string Encoding = "encoding";
        public const string Evaluator = "evaluator";
        public const string Extension = "extension";
        public const string Filter = "filter";
        public const string Flat = "flat";
        public const string Help = "help";
        public const string Highlight = "highlight";
        public const string CharGroup = "char-group";
        public const string IncludeDirectory = "include-directory";
        public const string IncludingBom = "including-bom";
        public const string Input = "input";
        public const string Manual = "manual";
        public const string MaxCount = "max-count";
        public const string Modify = "modify";
        public const string Name = "name";
        public const string NoGroups = "no-groups";
        public const string NoRecurse = "no-recurse";
        public const string Options = "options";
        public const string Output = "output";
        public const string PathsFrom = "paths-from";
        public const string Progress = "progress";
        public const string Properties = "properties";
        public const string Replacement = "replacement";
        public const string Section = "section";
        public const string Sort = "sort";
        public const string Target = "target";
        public const string Values = "values";
        public const string Verbosity = "verbosity";

        private static ImmutableDictionary<string, string>? _namesToShortNames;

        public static ImmutableDictionary<string, string> NamesToShortNames
        {
            get
            {
                if (_namesToShortNames == null)
                    Interlocked.CompareExchange(ref _namesToShortNames, Create(), null);

                return _namesToShortNames;

                static ImmutableDictionary<string, string> Create()
                {
                    FieldInfo[] names = typeof(OptionNames).GetFields();

                    return names.Join(
                        typeof(OptionShortNames).GetFields(),
                        f => f.Name,
                        f => f.Name,
                        (f, g) => new KeyValuePair<string, string>((string)f.GetValue(null)!, g.GetValue(null)!.ToString()!)).ToImmutableDictionary();
                }
            }
        }

        public static string GetHelpText(string name)
        {
            if (NamesToShortNames.TryGetValue(name, out string shortName))
            {
                return $"-{shortName}, --{name}";
            }
            else
            {
                return $"--{name}";
            }
        }
    }
}
