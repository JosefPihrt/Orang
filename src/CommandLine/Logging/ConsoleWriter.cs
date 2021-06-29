// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    internal sealed class ConsoleWriter : TextWriterWithVerbosity
    {
        public static ConsoleWriter Instance { get; } = new ConsoleWriter();

        private ConsoleWriter() : base(Console.Out, Console.Out.FormatProvider)
        {
        }

        public ConsoleColors Colors
        {
            get { return new ConsoleColors(Console.ForegroundColor, Console.BackgroundColor); }
            set
            {
                if (value.Foreground != null)
                    Console.ForegroundColor = value.Foreground.Value;

                if (value.Background != null)
                    Console.BackgroundColor = value.Background.Value;
            }
        }

        public void Write(char value, int repeatCount, in ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(value, repeatCount);
                Colors = tmp;
            }
            else
            {
                Write(value, repeatCount);
            }
        }

        public void Write(char value, int repeatCount, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value, repeatCount, colors);
        }

        public void Write(string? value, in ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(value);
                Colors = tmp;
            }
            else
            {
                Write(value);
            }
        }

        public void Write(string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value, colors);
        }

        public void Write(ReadOnlySpan<char> buffer, in ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(buffer);
                Colors = tmp;
            }
            else
            {
                Write(buffer);
            }
        }

        public void Write(ReadOnlySpan<char> buffer, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(buffer, colors);
        }

        public void Write(string? value, int startIndex, int length, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value.AsSpan(startIndex, length), colors);
        }

        public void WriteIf(bool condition, string? value, in ConsoleColors colors)
        {
            if (condition)
                Write(value, colors);
        }

        public void WriteIf(bool condition, string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            if (condition && verbosity <= Verbosity)
                Write(value, colors);
        }

        public void WriteLine(string? value, in ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(value);
                Colors = tmp;
                WriteLine();
            }
            else
            {
                WriteLine(value);
            }
        }

        public void WriteLine(string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                WriteLine(value, colors);
        }

        public void WriteLine(ReadOnlySpan<char> buffer, in ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(buffer);
                Colors = tmp;
                WriteLine();
            }
            else
            {
                WriteLine(buffer);
            }
        }

        public void WriteLine(ReadOnlySpan<char> buffer, in ConsoleColors colors, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                WriteLine(buffer, colors);
        }

        public void WriteLineIf(bool condition, string? value, in ConsoleColors colors)
        {
            if (condition)
                WriteLine(value, colors);
        }

        public void WriteLineIf(bool condition, ReadOnlySpan<char> buffer, in ConsoleColors colors)
        {
            if (condition)
                WriteLine(buffer, colors);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
