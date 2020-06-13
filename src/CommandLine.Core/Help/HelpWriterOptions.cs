// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine.Help
{
    public class HelpWriterOptions
    {
        public static HelpWriterOptions Default { get; } = new HelpWriterOptions();

        public HelpWriterOptions(
            string indent = "  ",
            Filter? filter = null)
        {
            Indent = indent;
            Filter = filter;
        }

        public string Indent { get; }

        public Filter? Filter { get; }
    }
}
