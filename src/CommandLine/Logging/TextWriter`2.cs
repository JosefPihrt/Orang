// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orang
{
    internal class TextWriter<TWriter1, TWriter2> : TextWriter
        where TWriter1 : TextWriter
        where TWriter2 : TextWriter
    {
        public TextWriter(TWriter1 writer1, TWriter2 writer2)
        {
            Writer1 = writer1;
            Writer2 = writer2;
        }

        public TextWriter(TWriter1 writer1, TWriter2 writer2, IFormatProvider formatProvider) : base(formatProvider)
        {
            Writer1 = writer1;
            Writer2 = writer2;
        }

        public override Encoding Encoding => throw new NotSupportedException();

        public TWriter1 Writer1 { get; }

        public TWriter2 Writer2 { get; }

        public override void Write(bool value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(char value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(char[]? buffer)
        {
            Writer1.Write(buffer);
            Writer2.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Writer1.Write(buffer, index, count);
            Writer2.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(double value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(int value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(long value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(object? value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            Writer1.Write(buffer);
            Writer2.Write(buffer);
        }

        public override void Write(float value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(string? value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

#pragma warning disable CS0809
        [Obsolete]
        public override void Write(string format, object? arg0)
        {
            Writer1.Write(format, arg0);
            Writer2.Write(format, arg0);
        }

        [Obsolete]
        public override void Write(string format, object? arg0, object? arg1)
        {
            Writer1.Write(format, arg0, arg1);
            Writer2.Write(format, arg0, arg1);
        }

        [Obsolete]
        public override void Write(string format, object? arg0, object? arg1, object? arg2)
        {
            Writer1.Write(format, arg0, arg1, arg2);
            Writer2.Write(format, arg0, arg1, arg2);
        }

        [Obsolete]
        public override void Write(string format, params object?[] arg)
        {
            Writer1.Write(format, arg);
            Writer2.Write(format, arg);
        }
#pragma warning restore CS0809

        public override void Write(StringBuilder? value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(uint value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

        public override void Write(ulong value)
        {
            Writer1.Write(value);
            Writer2.Write(value);
        }

#pragma warning disable CS0809
        [Obsolete("", error: true)]
        public override Task WriteAsync(char value)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteAsync(string? value)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
#pragma warning restore CS0809

        public override void WriteLine()
        {
            Writer1.WriteLine();
            Writer2.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(char[]? buffer)
        {
            Writer1.WriteLine(buffer);
            Writer2.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Writer1.WriteLine(buffer, index, count);
            Writer2.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(object? value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            Writer1.WriteLine(buffer);
            Writer2.WriteLine(buffer);
        }

        public override void WriteLine(float value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(string? value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

#pragma warning disable CS0809
        [Obsolete]
        public override void WriteLine(string format, object? arg0)
        {
            Writer1.WriteLine(format, arg0);
            Writer2.WriteLine(format, arg0);
        }

        [Obsolete]
        public override void WriteLine(string format, object? arg0, object? arg1)
        {
            Writer1.WriteLine(format, arg0, arg1);
            Writer2.WriteLine(format, arg0, arg1);
        }

        [Obsolete]
        public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
        {
            Writer1.WriteLine(format, arg0, arg1, arg2);
            Writer2.WriteLine(format, arg0, arg1, arg2);
        }

        [Obsolete]
        public override void WriteLine(string format, params object?[] arg)
        {
            Writer1.WriteLine(format, arg);
            Writer2.WriteLine(format, arg);
        }
#pragma warning restore CS0809

        public override void WriteLine(StringBuilder? value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Writer1.WriteLine(value);
            Writer2.WriteLine(value);
        }

#pragma warning disable CS0809
        [Obsolete("", error: true)]
        public override Task WriteLineAsync()
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteLineAsync(char value)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteLineAsync(string? value)
        {
            throw new NotSupportedException();
        }

        [Obsolete("", error: true)]
        public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
#pragma warning restore CS0809
    }
}
