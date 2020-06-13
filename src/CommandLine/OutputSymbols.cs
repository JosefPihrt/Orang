// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang
{
    internal class OutputSymbols
    {
        public static OutputSymbols Empty { get; } = new OutputSymbols();

        public static OutputSymbols Default { get; } = new OutputSymbols(
            tab: "T",
            carriageReturn: "CR",
            linefeed: "LF",
            space: "_",
            openValue: "(",
            closeValue: ")");

        public OutputSymbols(
            string? tab = null,
            string? carriageReturn = null,
            string? linefeed = null,
            string? space = null,
            string? openValue = null,
            string? closeValue = null)
        {
            Tab = tab;
            CarriageReturn = carriageReturn;
            Linefeed = linefeed;
            Space = space;
            OpenBoundary = openValue;
            CloseBoundary = closeValue;
        }

        public static OutputSymbols Create(HighlightOptions options)
        {
            if ((options & (HighlightOptions.Character | HighlightOptions.Boundary)) == 0)
                return Empty;

            return new OutputSymbols(
                tab: ((options & HighlightOptions.Tab) != 0) ? Default.Tab : null,
                carriageReturn: ((options & HighlightOptions.CarriageReturn) != 0) ? Default.CarriageReturn : null,
                linefeed: ((options & HighlightOptions.Linefeed) != 0) ? Default.Linefeed : null,
                space: ((options & HighlightOptions.Space) != 0) ? Default.Space : null,
                openValue: ((options & HighlightOptions.Boundary) != 0) ? Default.OpenBoundary : null,
                closeValue: ((options & HighlightOptions.Boundary) != 0) ? Default.CloseBoundary : null);
        }

        public string? Tab { get; }

        public string? CarriageReturn { get; }

        public string? Linefeed { get; }

        public string? Space { get; }

        public string? OpenBoundary { get; }

        public string? CloseBoundary { get; }
    }
}
