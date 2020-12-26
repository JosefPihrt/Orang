// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Orang
{
    internal readonly struct CaptureInfo
    {
        public CaptureInfo(string value, int index, Capture? capture = null)
        {
            Value = value;
            Index = index;
            Capture = capture;
        }

        public string Value { get; }

        public int Index { get; }

        public int Length => Value.Length;

        public Capture? Capture { get; }

        public static CaptureInfo FromCapture(Capture capture)
        {
            return new CaptureInfo(capture.Value, capture.Index, capture);
        }
    }
}
