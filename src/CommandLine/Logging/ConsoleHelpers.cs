// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
    }
}
