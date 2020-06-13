// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Orang
{
    internal class StreamComparer
    {
        public static int DefaultBufferSize = 4096;
        public readonly int BufferSize = DefaultBufferSize;

        private readonly byte[] _buffer1;
        private readonly byte[] _buffer2;

        public StreamComparer() : this(DefaultBufferSize)
        {
        }

        public StreamComparer(int bufferSize)
        {
            BufferSize = bufferSize;

            _buffer1 = new byte[BufferSize];
            _buffer2 = new byte[BufferSize];
        }

        public static StreamComparer Default { get; } = new StreamComparer();

        public bool Equals(Stream stream1, Stream stream2)
        {
            return stream1.Length == stream2.Length
                && ByteEquals(stream1, stream2);
        }

        public bool ByteEquals(Stream stream1, Stream stream2)
        {
            while (true)
            {
                int count1 = Read(stream1, _buffer1);
                int count2 = Read(stream2, _buffer2);

                if (count1 != count2)
                    return false;

                if (count1 == 0)
                    return true;

                Span<byte> span1 = _buffer1.AsSpan(0, count1);
                Span<byte> span2 = _buffer2.AsSpan(0, count1);

                if (!span1.SequenceEqual(span2))
                    return false;
            }
        }

        private static int Read(Stream stream, byte[] buffer)
        {
            int totalRead = 0;

            while (totalRead < buffer.Length)
            {
                int read = stream.Read(buffer, totalRead, buffer.Length - totalRead);

                if (read == 0)
                    return totalRead;

                totalRead += read;
            }

            return totalRead;
        }
    }
}
