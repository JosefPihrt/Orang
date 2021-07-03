// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Orang.FileSystem;
using Orang.Text.RegularExpressions;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class LogHelpers
    {
        public static void WriteObsoleteWarning(string message)
        {
            WriteWarning(message);
            ConsoleHelpers.WaitForKeyPress();
        }

        public static void WriteFilePathEnd(int count, MaxReason maxReason, bool includeCount)
        {
            Verbosity verbosity = ConsoleOut.Verbosity;

            if (count >= 0)
            {
                if (verbosity >= Verbosity.Detailed
                    || includeCount)
                {
                    verbosity = (includeCount) ? Verbosity.Minimal : Verbosity.Detailed;

                    ConsoleOut.Write("  ", Colors.Message_OK, verbosity);
                    ConsoleOut.Write(count.ToString("n0"), Colors.Message_OK, verbosity);
                    ConsoleOut.WriteIf(maxReason == MaxReason.CountExceedsMax, "+", Colors.Message_OK, verbosity);
                }
            }

            ConsoleOut.WriteLine(Verbosity.Minimal);

            if (Out != null)
            {
                verbosity = Out.Verbosity;

                if (verbosity >= Verbosity.Detailed
                    || includeCount)
                {
                    verbosity = (includeCount) ? Verbosity.Minimal : Verbosity.Detailed;

                    Out.Write("  ", verbosity);
                    Out.Write(count.ToString("n0"), verbosity);
                    Out.WriteIf(maxReason == MaxReason.CountExceedsMax, "+", verbosity);
                }

                Out.WriteLine(Verbosity.Minimal);
            }
        }

        public static void WriteFileError(
            Exception ex,
            string? path = null,
            string? indent = null,
            Verbosity verbosity = Verbosity.Normal)
        {
            string message = ex.Message;

            WriteLine($"{indent}ERROR: {message}", Colors.Message_Warning, verbosity);

            if (!string.IsNullOrEmpty(path)
                && !string.IsNullOrEmpty(message)
                && !message.Contains(path, FileSystemHelpers.Comparison))
            {
                WriteLine($"{indent}PATH: {path}", Colors.Message_Warning, verbosity);
            }
#if DEBUG
            WriteLine($"{indent}STACK TRACE:", verbosity);
            WriteLine(ex.StackTrace, verbosity);
#endif
        }

        public static void WriteSearchedFilesAndDirectories(
            SearchTelemetry telemetry,
            SearchTarget searchTarget,
            Verbosity verbosity = Verbosity.Detailed)
        {
            if (!ShouldLog(verbosity))
                return;

            WriteLine(verbosity);

            if (searchTarget != SearchTarget.Directories)
            {
                WriteCount("Files searched", telemetry.FileCount, verbosity: verbosity);
            }

            if (searchTarget != SearchTarget.Files)
            {
                if (searchTarget != SearchTarget.Directories)
                    Write("  ", verbosity);

                WriteCount("Directories searched", telemetry.DirectoryCount, verbosity: verbosity);
            }

            if (telemetry.FilesTotalSize > 0)
            {
                if (searchTarget == SearchTarget.Files)
                {
                    WriteCount("  Files total size", telemetry.FilesTotalSize, verbosity: verbosity);
                }
                else if (searchTarget == SearchTarget.Directories)
                {
                    WriteCount("  Directories total size", telemetry.FilesTotalSize, verbosity: verbosity);
                }
                else if (searchTarget == SearchTarget.All)
                {
                    WriteCount("  Total size", telemetry.FilesTotalSize, verbosity: verbosity);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            if (telemetry.Elapsed != default)
                Write($"  Elapsed time: {telemetry.Elapsed:mm\\:ss\\.ff}", verbosity);

            WriteLine(verbosity);
        }

        public static void WriteProcessedFilesAndDirectories(
            SearchTelemetry telemetry,
            SearchTarget searchTarget,
            string processedFilesTitle,
            string processedDirectoriesTitle,
            bool dryRun,
            Verbosity verbosity = Verbosity.Detailed)
        {
            if (!ShouldLog(verbosity))
                return;

            WriteLine(verbosity);

            const string filesTitle = "Matching files";
            const string directoriesTitle = "Matching directories";

            bool files = searchTarget != SearchTarget.Directories;
            bool directories = searchTarget != SearchTarget.Files;

            string matchingFileCount = telemetry.MatchingFileCount.ToString("n0");
            string matchingDirectoryCount = telemetry.MatchingDirectoryCount.ToString("n0");
            string processedFileCount = telemetry.ProcessedFileCount.ToString("n0");
            string processedDirectoryCount = telemetry.ProcessedDirectoryCount.ToString("n0");

            int width1 = Math.Max(filesTitle.Length, processedFilesTitle.Length);
            int width2 = Math.Max(matchingFileCount.Length, processedFileCount.Length);
            int width3 = Math.Max(directoriesTitle.Length, processedDirectoriesTitle.Length);
            int width4 = Math.Max(matchingDirectoryCount.Length, processedDirectoryCount.Length);

            ConsoleColors colors = Colors.Message_OK;

            if (files)
                WriteCount(filesTitle, matchingFileCount, width1, width2, colors, verbosity);

            if (directories)
            {
                if (files)
                    Write("  ", colors, verbosity);

                WriteCount(directoriesTitle, matchingDirectoryCount, width3, width4, colors, verbosity);
            }

            WriteLine(verbosity);

            colors = (dryRun) ? Colors.Message_DryRun : Colors.Message_Change;

            if (files)
                WriteCount(processedFilesTitle, processedFileCount, width1, width2, colors, verbosity);

            if (directories)
            {
                if (files)
                    Write("  ", colors, verbosity);

                WriteCount(processedDirectoriesTitle, processedDirectoryCount, width3, width4, colors, verbosity);
            }

            WriteLine(verbosity);
        }

        public static void WriteGroups(
            GroupDefinitionCollection groupDefinitions,
            Dictionary<int, ConsoleColors>? colors = null,
            Verbosity verbosity = Verbosity.Detailed)
        {
            if (!ShouldLog(verbosity))
                return;

            if (groupDefinitions.Count == 1
                && groupDefinitions[0].Number == 0)
            {
                return;
            }

            WriteLine(verbosity);

            WriteLine("Groups:", verbosity);

            int maxWidth = groupDefinitions.Max(f => f.Number).GetDigitCount();

            foreach (GroupDefinition groupDefinition in groupDefinitions.OrderBy(f => f.Number))
            {
                if (groupDefinition.Number == 0)
                    continue;

                ConsoleColors groupColors = default;

                colors?.TryGetValue(groupDefinition.Number, out groupColors);

                Write(" ", verbosity);

                string indexText = groupDefinition.Number.ToString();

                Write(' ', maxWidth - indexText.Length, verbosity);
                Write(indexText, groupColors, verbosity);

                if (indexText != groupDefinition.Name)
                {
                    Write(" ", verbosity);
                    Write(groupDefinition.Name, groupColors, verbosity);
                }

                WriteLine(verbosity);
            }
        }

        public static int WriteCount(
            string name,
            int count,
            in ConsoleColors colors = default,
            Verbosity verbosity = Verbosity.Quiet)
        {
            return WriteCount(name, count.ToString("n0"), colors, verbosity);
        }

        public static int WriteCount(
            string name,
            long count,
            in ConsoleColors colors = default,
            Verbosity verbosity = Verbosity.Quiet)
        {
            return WriteCount(name, count.ToString("n0"), colors, verbosity);
        }

        public static int WriteCount(
            string name,
            string count,
            in ConsoleColors colors = default,
            Verbosity verbosity = Verbosity.Quiet)
        {
            return WriteCount(name, count, name.Length, count.Length, colors, verbosity);
        }

        internal static int WriteCount(
            string name,
            string count,
            int nameWidth,
            int countWidth,
            in ConsoleColors colors,
            Verbosity verbosity = Verbosity.Quiet)
        {
            if (name.Length > 0)
            {
                Write(name, colors, verbosity);
                Write(": ", colors, verbosity);
            }

            Write(' ', nameWidth - name.Length, verbosity);
            Write(' ', countWidth - count.Length, verbosity);
            Write(count, colors, verbosity);

            return nameWidth + countWidth + 2;
        }
    }
}
