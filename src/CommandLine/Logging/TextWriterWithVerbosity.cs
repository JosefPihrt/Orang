// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Orang
{
    internal class TextWriterWithVerbosity : TextWriter
    {
        public TextWriterWithVerbosity(TextWriter writer)
        {
            Writer = writer;
        }

        public TextWriterWithVerbosity(TextWriter writer, IFormatProvider formatProvider) : base(formatProvider)
        {
            Writer = writer;
        }

        public Verbosity Verbosity { get; set; } = Verbosity.Diagnostic;

        public override Encoding Encoding => Writer.Encoding;

        protected TextWriter Writer { get; }

        public void Write(char value, int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
                Write(value);
        }

        public void Write(char value, int repeatCount, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value, repeatCount);
        }

        public void Write(char value, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value);
        }

        public void Write(string? value, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value);
        }

        public void Write(ReadOnlySpan<char> buffer, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(buffer);
        }

        public void Write(string? value, int startIndex, int length)
        {
            Write(value.AsSpan(startIndex, length));
        }

        public void Write(string? value, int startIndex, int length, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                Write(value.AsSpan(startIndex, length));
        }

        public void WriteIf(bool condition, string? value)
        {
            if (condition)
                Write(value);
        }

        public void WriteIf(bool condition, ReadOnlySpan<char> buffer)
        {
            if (condition)
                Write(buffer);
        }

        public void WriteIf(bool condition, string? value, Verbosity verbosity)
        {
            if (condition && verbosity <= Verbosity)
                Write(value);
        }

        public void WriteIf(bool condition, ReadOnlySpan<char> buffer, Verbosity verbosity)
        {
            if (condition && verbosity <= Verbosity)
                Write(buffer);
        }

        public void WriteLine(Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                WriteLine();
        }

        public void WriteLine(string? value, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                WriteLine(value);
        }

        public void WriteLine(ReadOnlySpan<char> buffer, Verbosity verbosity)
        {
            if (verbosity <= Verbosity)
                WriteLine(buffer);
        }

        public void WriteLineIf(bool condition)
        {
            if (condition)
                WriteLine();
        }

        public void WriteLineIf(bool condition, Verbosity verbosity)
        {
            if (condition && verbosity <= Verbosity)
                WriteLine();
        }

        public void WriteLineIf(bool condition, string? value)
        {
            if (condition)
                WriteLine(value);
        }

        public void WriteLineIf(bool condition, ReadOnlySpan<char> buffer)
        {
            if (condition)
                WriteLine(buffer);
        }

        public override void Write(bool value)
        {
            Writer.Write(value);
        }

        public override void Write(char value)
        {
            Writer.Write(value);
        }

        public override void Write(char[]? buffer)
        {
            Writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Writer.Write(value);
        }

        public override void Write(double value)
        {
            Writer.Write(value);
        }

        public override void Write(int value)
        {
            Writer.Write(value);
        }

        public override void Write(long value)
        {
            Writer.Write(value);
        }

        public override void Write(object? value)
        {
            Writer.Write(value);
        }

        public override void Write(float value)
        {
            Writer.Write(value);
        }

        public override void Write(string? value)
        {
            Writer.Write(value);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            Writer.Write(buffer);
        }

        public override void Write(string format, object? arg0)
        {
            Debug.Fail("");
            Writer.Write(format, arg0);
        }

        public override void Write(string format, object? arg0, object? arg1)
        {
            Debug.Fail("");
            Writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, object? arg0, object? arg1, object? arg2)
        {
            Debug.Fail("");
            Writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object?[] arg)
        {
            Debug.Fail("");
            Writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            Writer.Write(value);
        }

        public override void Write(ulong value)
        {
            Writer.Write(value);
        }

        public override void WriteLine()
        {
            Writer.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char[]? buffer)
        {
            Writer.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(object? value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(string? value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            Writer.WriteLine(buffer);
        }

        public override void WriteLine(string format, object? arg0)
        {
            Debug.Fail("");
            Writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object? arg0, object? arg1)
        {
            Debug.Fail("");
            Writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
        {
            Debug.Fail("");
            Writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object?[] arg)
        {
            Debug.Fail("");
            Writer.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Writer.WriteLine(value);
        }

        public bool ShouldWrite(Verbosity verbosity)
        {
            return verbosity <= Verbosity;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Writer.Dispose();

            base.Dispose(disposing);
        }
    }
}
