// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal abstract class CommonReplaceCommandOptions : CommonFindCommandOptions
    {
        internal CommonReplaceCommandOptions()
        {
        }

        public string? Input { get; internal set; }

        public bool DryRun { get; internal set; }

        public IReplacer Replacer { get; internal set; } = null!;

        public bool Interactive { get; internal set; }
    }
}
