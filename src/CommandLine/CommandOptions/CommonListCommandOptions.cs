// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal abstract class CommonListCommandOptions : AbstractCommandOptions
    {
        internal CommonListCommandOptions()
        {
        }

        public string? Filter { get; internal set; }
    }
}
