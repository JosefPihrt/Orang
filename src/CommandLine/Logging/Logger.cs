// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    internal static class Logger
    {
        public static ConsoleWriter ConsoleOut { get; } = ConsoleWriter.Instance;

        public static TextWriterWithVerbosity? Out { get; set; }

        public static void Write(char value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(char value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity);
            Out?.Write(value, verbosity);
        }

        public static void Write(char value, int repeatCount)
        {
            Write(value, repeatCount, Verbosity.Quiet);
        }

        public static void Write(char value, int repeatCount, Verbosity verbosity)
        {
            ConsoleOut.Write(value, repeatCount, verbosity);
            Out?.Write(value, repeatCount, verbosity);
        }

        public static void Write(char value, int repeatCount, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.Write(value, repeatCount, colors, verbosity);
            Out?.Write(value, repeatCount, verbosity);
        }

        public static void Write(string? value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string? value, in ConsoleColors colors)
        {
            ConsoleOut.Write(value, colors);
            Out?.Write(value);
        }

        public static void Write(string? value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(ReadOnlySpan<char> value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.Write(value, colors, verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(ReadOnlySpan<char> value, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.Write(value, colors, verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(string? value, int startIndex, int length)
        {
            ConsoleOut.Write(value, startIndex, length);
            Out?.Write(value, startIndex, length);
        }

        public static void Write(string? value, int startIndex, int length, in ConsoleColors colors)
        {
            ConsoleOut.Write(value.AsSpan(startIndex, length), colors);
            Out?.Write(value, startIndex, length);
        }

        public static void Write(string? value, int startIndex, int length, Verbosity verbosity)
        {
            ConsoleOut.Write(value, startIndex, length, verbosity);
            Out?.Write(value, startIndex, length, verbosity);
        }

        public static void Write(string? value, int startIndex, int length, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.Write(value, startIndex, length, colors, verbosity);
            Out?.Write(value, startIndex, length, verbosity);
        }

        public static void WriteIf(bool condition, string? value)
        {
            ConsoleOut.WriteIf(condition, value);
            Out?.WriteIf(condition, value);
        }

        public static void WriteIf(bool condition, string? value, in ConsoleColors colors)
        {
            ConsoleOut.WriteIf(condition, value, colors);
            Out?.WriteIf(condition, value);
        }

        public static void WriteIf(bool condition, string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.WriteIf(condition, value, colors, verbosity);
            Out?.WriteIf(condition, value, verbosity);
        }

        public static void Write(object? value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void WriteLine()
        {
            ConsoleOut.WriteLine();
            Out?.WriteLine();
        }

        public static void WriteLine(Verbosity verbosity)
        {
            ConsoleOut.WriteLine(verbosity);
            Out?.WriteLine(verbosity);
        }

        public static void WriteLineIf(bool condition)
        {
            ConsoleOut.WriteLineIf(condition);
            Out?.WriteLineIf(condition);
        }

        public static void WriteLineIf(bool condition, Verbosity verbosity)
        {
            ConsoleOut.WriteLineIf(condition, verbosity);
            Out?.WriteLineIf(condition, verbosity);
        }

        public static void WriteLine(char value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string? value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string? value, in ConsoleColors colors)
        {
            ConsoleOut.WriteLine(value, colors);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string? value, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(ReadOnlySpan<char> value, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(string? value, in ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, colors, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLineIf(bool condition, string? value)
        {
            ConsoleOut.WriteLineIf(condition, value);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLineIf(bool condition, string value, in ConsoleColors colors)
        {
            ConsoleOut.WriteLineIf(condition, value, colors);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLine(object? value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteWarning(
            Exception exception,
            string? message = null,
            in ConsoleColors colors = default,
            Verbosity verbosity = Verbosity.Minimal)
        {
            WriteWarning(message ?? exception.Message, colors, verbosity);

            WriteException(exception);
        }

        public static void WriteWarning(
            string message,
            ConsoleColors colors = default,
            Verbosity verbosity = Verbosity.Minimal)
        {
            if (colors.IsDefault)
                colors = Colors.Message_Warning;

            WriteLine(message, colors, verbosity);
        }

        public static void WriteError(
            Exception exception,
            string? message = null)
        {
            WriteError(message ?? exception.Message);

            WriteException(exception);
        }

        public static void WriteError(string message)
        {
            Console.Error.WriteLine(message);
        }

        private static void WriteException(Exception exception)
        {
            var verbosity = Verbosity.Diagnostic;
#if DEBUG
            verbosity = Verbosity.Quiet;
#endif
            if (ConsoleOut.Verbosity >= verbosity)
                Console.Error.WriteLine(exception.ToString());

            if (Out?.Verbosity >= verbosity)
                Out.WriteLine(exception.ToString());
        }

        public static bool ShouldLog(Verbosity verbosity)
        {
            return verbosity <= ConsoleOut.Verbosity
                || (Out != null && verbosity <= Out.Verbosity);
        }
    }
}
