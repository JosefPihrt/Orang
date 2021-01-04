// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Orang.Syntax;

namespace Orang.CommandLine
{
    internal sealed class ListPatternsCommandOptions : AbstractCommandOptions
    {
        internal ListPatternsCommandOptions()
        {
        }

        public char? Value { get; internal set; }

        public Filter? Filter { get; internal set; }

        public bool InCharGroup { get; internal set; }

        public RegexOptions RegexOptions { get; internal set; }

        public ImmutableArray<SyntaxSection> Sections { get; internal set; }

        protected override void WriteDiagnosticCore()
        {
            DiagnosticWriter.WriteListPatternsCommand(this);
        }
    }
}
