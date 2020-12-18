// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Orang.CommandLine
{
    internal class ValueWriter
    {
        public ValueWriter(
            ContentTextWriter? writer,
            string? indent = null,
            bool includeEndingIndent = true)
        {
            Writer = writer;
            Indent = indent;
            IncludeEndingIndent = includeEndingIndent;
        }

        private ContentTextWriter? Writer { get; }

        public string? Indent { get; }

        public bool IncludeEndingIndent { get; }

        public void Write(
            string value,
            OutputSymbols symbols,
            in ConsoleColors colors = default,
            in ConsoleColors boundaryColors = default)
        {
            Write(value, 0, value.Length, symbols, colors, boundaryColors);
        }

        public void Write(
            string value,
            int startIndex,
            int length,
            OutputSymbols? symbols,
            in ConsoleColors colors = default,
            in ConsoleColors boundaryColors = default)
        {
            if (symbols == null)
                symbols = OutputSymbols.Empty;

            Write(symbols.OpenBoundary, boundaryColors);

            if (!ReferenceEquals(symbols, OutputSymbols.Empty)
                || !string.IsNullOrEmpty(Indent))
            {
                WriteImpl(
                    value,
                    startIndex,
                    length,
                    symbols,
                    (string.IsNullOrEmpty(symbols.OpenBoundary)) ? colors : default);
            }
            else
            {
                Write(value, startIndex, length, colors);
            }

            Write(symbols.CloseBoundary, boundaryColors);
        }

        private void WriteImpl(string value, int startIndex, int length, OutputSymbols symbols, ConsoleColors colors)
        {
            Debug.Assert(length >= 0, length.ToString());

            var symbolColors = new ConsoleColors(
                Colors.Symbol.Foreground ?? colors.Foreground,
                Colors.Symbol.Background ?? colors.Background);

            int lastPos = startIndex;
            int endIndex = startIndex + length - 1;
            int i = startIndex;

            while (i <= endIndex)
            {
                switch (value[i])
                {
                    case (char)9:
                        {
                            WriteSymbol(symbols.Tab);
                            break;
                        }
                    case (char)10:
                        {
                            Write(value, lastPos, i - lastPos, colors);
                            Write(symbols.Linefeed, symbolColors);
                            lastPos = i + 1;

                            WriteEndOfLine(value, i, endIndex, symbols);
                            break;
                        }
                    case (char)13:
                        {
                            Write(value, lastPos, i - lastPos, colors);
                            Write(symbols.CarriageReturn, symbolColors);

                            lastPos = i + 1;
                            break;
                        }
                    case (char)32:
                        {
                            WriteSymbol(symbols.Space);
                            break;
                        }
                }

                i++;
            }

            Write(value, lastPos, startIndex + length - lastPos, colors);

            void WriteSymbol(string? symbol)
            {
                if (symbol != null)
                {
                    Write(value, lastPos, i - lastPos, colors);
                    lastPos = i + 1;

                    Write(symbol, symbolColors);
                }
            }
        }

        protected virtual void WriteEndOfLine(string value, int index, int endIndex, OutputSymbols symbols)
        {
            WriteNewLine(value, index);

            if (IncludeEndingIndent
                || index < endIndex)
            {
                Write(Indent);
            }
        }

        protected void WriteNewLine(string value, int index)
        {
            if (index > 0
                && value[index - 1] == '\r')
            {
                Write("\r\n");
            }
            else
            {
                Write("\n");
            }
        }

        protected virtual void Write(string? value)
        {
            Writer?.Write(value);
        }

        protected virtual void Write(string? value, in ConsoleColors colors)
        {
            Writer?.Write(value, colors);
        }

        protected virtual void Write(string? value, int startIndex, int length, in ConsoleColors colors)
        {
            Writer?.Write(value, startIndex, length, colors);
        }
    }
}
