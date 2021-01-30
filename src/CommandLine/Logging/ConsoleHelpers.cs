// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class ConsoleHelpers
    {
        public static void WaitForKeyPress(string? message = null)
        {
            if (Console.IsInputRedirected)
                return;

            ConsoleOut.Write(message ?? "Press any key to continue...");
            Console.ReadKey();
            ConsoleOut.WriteLine();
        }

        public static string? ReadRedirectedInput()
        {
            if (Console.IsInputRedirected)
            {
                using (Stream stream = Console.OpenStandardInput())
                using (var streamReader = new StreamReader(stream, Console.InputEncoding))
                {
                    return streamReader.ReadToEnd();
                }
            }

            return null;
        }

        public static IEnumerable<string> ReadRedirectedInputAsLines()
        {
            if (Console.IsInputRedirected)
            {
                using (Stream stream = Console.OpenStandardInput())
                using (var streamReader = new StreamReader(stream, Console.InputEncoding))
                {
                    string? line;

                    while ((line = streamReader.ReadLine()) != null)
                        yield return line;
                }
            }
        }

        public static bool AskToExecute(string text, string? indent = null)
        {
            switch (Ask(
                text,
                " (Y/N/C): ",
                "Y (Yes), N (No), C (Cancel)",
                DialogResultMap.Yes_No_Cancel,
                indent,
                throwOnCancel: true))
            {
                case DialogResult.Yes:
                    return true;
                case DialogResult.No:
                case DialogResult.None:
                    return false;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static DialogResult AskToContinue(string indent)
        {
            return Ask(
                question: "Continue?",
                suffix: " (Y[A]/C): ",
                helpText: "Y (Yes), YA (Yes to All), C (Cancel)",
                map: DialogResultMap.Yes_YesToAll_Cancel,
                indent: indent,
                throwOnCancel: true);
        }

        public static DialogResult Ask(string question, string? indent = null)
        {
            return Ask(
                question,
                " (Y[A]/N[A]/C): ",
                "Y (Yes), YA (Yes to All), N (No), NA (No to All), C (Cancel)",
                DialogResultMap.All,
                indent);
        }

        private static DialogResult Ask(
            string question,
            string suffix,
            string helpText,
            ImmutableDictionary<string, DialogResult> map,
            string? indent,
            bool throwOnCancel = false)
        {
            while (true)
            {
                ConsoleOut.Write(indent);
                ConsoleOut.Write(question);
                ConsoleOut.Write(suffix);

                string? s = Console.ReadLine()?.Trim();

                if (s != null)
                {
                    if (s.Length == 0)
                        return DialogResult.None;

                    if (map.TryGetValue(s, out DialogResult dialogResult))
                    {
                        if (dialogResult == DialogResult.Cancel
                            && throwOnCancel)
                        {
                            throw new OperationCanceledException();
                        }

                        return dialogResult;
                    }
                }
                else
                {
                    ConsoleOut.WriteLine();
                    return DialogResult.None;
                }

                ConsoleOut.Write(indent);
                ConsoleOut.WriteLine($"Value '{s}' is invalid. Allowed values are: {helpText}");
            }
        }

        public static string? ReadUserInput(
            string prompt,
            string defaultValue,
            string? indent = null)
        {
            while (true)
            {
                string? s = ReadUserInput(defaultValue, indent + prompt);

                if (s == null)
                    ConsoleOut.WriteLine();

                return s;
            }
        }

        public static string? ReadUserInput(string defaultValue, string? prompt = null)
        {
            bool treatControlCAsInput = Console.TreatControlCAsInput;

            Console.TreatControlCAsInput = true;

            string? input = ReadUserInputImpl(defaultValue, prompt ?? "");

            Console.TreatControlCAsInput = treatControlCAsInput;

            return input;
        }

        public static string? ReadUserInputImpl(string defaultValue, string prompt)
        {
            Console.Write(prompt);

            int initTop = Console.CursorTop;
            int initLeft = Console.CursorLeft;

            List<char> buffer = defaultValue.ToList();

            Console.Write(defaultValue);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            int index = GetIndex();

                            if (index == 0)
                                break;

                            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                            {
                                int i = index - 1;
                                while (i > 0)
                                {
                                    if (char.IsLetterOrDigit(buffer[i])
                                        && !char.IsLetterOrDigit(buffer[i - 1]))
                                    {
                                        break;
                                    }

                                    i--;
                                }

                                int offset = index - i - 1;

                                int left = Console.CursorLeft;
                                int top = Console.CursorTop;

                                while (true)
                                {
                                    if (offset < left)
                                    {
                                        left -= offset;
                                        break;
                                    }
                                    else
                                    {
                                        offset -= left;
                                        left = Console.WindowWidth;
                                        top--;
                                    }
                                }

                                Console.SetCursorPosition(left, top);
                                break;
                            }

                            if (Console.CursorLeft == 0)
                            {
                                Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop - 1);
                            }
                            else
                            {
                                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            }

                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            int index = GetIndex();

                            if (index == buffer.Count)
                                break;

                            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                            {
                                int i = index + 1;
                                while (i < buffer.Count - 1)
                                {
                                    if (char.IsLetterOrDigit(buffer[i])
                                        && !char.IsLetterOrDigit(buffer[i + 1]))
                                    {
                                        break;
                                    }

                                    i++;
                                }

                                int offset = i + 1 - index;

                                int left = Console.CursorLeft;
                                int top = Console.CursorTop;

                                while (true)
                                {
                                    int right = Console.WindowWidth - left;

                                    if (offset < right)
                                    {
                                        left += offset;
                                        break;
                                    }
                                    else
                                    {
                                        offset -= right;
                                        left = 0;
                                        top++;
                                    }
                                }

                                Console.SetCursorPosition(left, top);
                                break;
                            }

                            if (Console.CursorLeft == Console.WindowWidth - 1)
                            {
                                Console.SetCursorPosition(0, Console.CursorTop + 1);
                            }
                            else
                            {
                                Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                            }

                            break;
                        }
                    case ConsoleKey.Home:
                        {
                            int left = (Console.CursorTop == initTop)
                                ? prompt.Length
                                : 0;

                            Console.SetCursorPosition(left, Console.CursorTop);
                            break;
                        }
                    case ConsoleKey.End:
                        {
                            int remainingLineCount = Console.WindowWidth - Console.CursorLeft;
                            int remainingCharCount = buffer.Count - GetIndex();

                            int left = (remainingCharCount > remainingLineCount)
                                ? Console.WindowWidth - 1
                                : Console.CursorLeft + remainingCharCount;

                            Console.SetCursorPosition(left, Console.CursorTop);
                            break;
                        }
                    case ConsoleKey.Backspace:
                        {
                            if (buffer.Count == 0)
                                break;

                            int index = GetIndex();

                            if (index == 0)
                                break;

                            buffer.RemoveAt(index - 1);

                            int left = Console.CursorLeft - 1;
                            int top = Console.CursorTop;

                            if (Console.CursorLeft == 0)
                            {
                                left = Console.WindowWidth - 1;
                                top--;
                            }

                            Console.SetCursorPosition(left, top);

                            char[] text = buffer.Skip(index - 1).Append(' ').ToArray();
                            Console.Write(text);

                            Console.SetCursorPosition(left, top);
                            break;
                        }
                    case ConsoleKey.Delete:
                        {
                            if (buffer.Count == 0)
                                break;

                            int index = GetIndex();

                            if (buffer.Count == index)
                                break;

                            buffer.RemoveAt(index);

                            int left = Console.CursorLeft;
                            int top = Console.CursorTop;

                            char[] text = buffer.Skip(index).Append(' ').ToArray();
                            Console.Write(text);

                            Console.SetCursorPosition(left, top);

                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            Console.SetCursorPosition(initLeft, initTop);
                            Console.Write(new string(' ', buffer.Count));
                            Console.SetCursorPosition(initLeft, initTop);

                            if (string.Equals(new string(buffer.ToArray()), defaultValue, StringComparison.Ordinal))
                            {
                                buffer.Clear();
                            }
                            else if (buffer.Count > 0)
                            {
                                Console.Write(defaultValue);
                                buffer = defaultValue.ToList();
                            }

                            break;
                        }
                    default:
                        {
                            char ch = keyInfo.KeyChar;

                            // ctrl+c
                            if (keyInfo.Modifiers == ConsoleModifiers.Control
                                && ch == 3)
                            {
                                Console.WriteLine();
                                throw new OperationCanceledException();
                            }

                            if (ch < 32)
                                break;

                            int index = GetIndex();

                            buffer.Insert(index, ch);

                            int left = Console.CursorLeft;
                            int top = Console.CursorTop;

                            char[] text = buffer.Skip(index).ToArray();
                            Console.Write(text);

                            if (left == Console.WindowWidth - 1)
                            {
                                Console.SetCursorPosition(0, top + 1);
                            }
                            else
                            {
                                Console.SetCursorPosition(left + 1, top);
                            }

                            break;
                        }
                }

                keyInfo = Console.ReadKey(true);
            }

            Console.WriteLine();

            return new string(buffer.ToArray());

            int GetIndex()
            {
                if (Console.CursorTop == initTop)
                    return Console.CursorLeft - initLeft;

                int index = Console.WindowWidth - initLeft;
                index += (Console.CursorTop - initTop - 1) * Console.WindowWidth;
                index += Console.CursorLeft;

                return index;
            }
        }
    }
}
