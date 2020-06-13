// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class ContentTextWriter
    {
        public ContentTextWriter(Verbosity verbosity = Verbosity.Normal)
        {
            Verbosity = verbosity;
        }

        public static ContentTextWriter Default { get; } = new ContentTextWriter(verbosity: Verbosity.Normal);

        public Verbosity Verbosity { get; }

        public void Write(string? value)
        {
            Logger.Write(value, Verbosity);
        }

        public void Write(string? value, in ConsoleColors colors)
        {
            Logger.Write(value, colors, Verbosity);
        }

        public void Write(string? value, int startIndex, int length)
        {
            Logger.Write(value, startIndex, length, Verbosity);
        }

        public void Write(string? value, int startIndex, int length, in ConsoleColors colors)
        {
            Logger.Write(value, startIndex, length, colors, Verbosity);
        }

        public void WriteLine()
        {
            Logger.WriteLine(Verbosity);
        }
    }
}
