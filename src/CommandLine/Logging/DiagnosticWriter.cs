// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal static class DiagnosticWriter
    {
        public static Verbosity Verbosity { get; } = Verbosity.Diagnostic;

        private static ConsoleColors ValueColors { get; } = new ConsoleColors(ConsoleColor.Cyan);

        private static ConsoleColors NullValueColors { get; } = new ConsoleColors(ConsoleColor.DarkGray);

        private static ValueWriter ValueWriter { get; }
            = new ValueWriter(new ContentTextWriter(Verbosity.Diagnostic), includeEndingIndent: false);

        private static OutputSymbols Symbols_Character { get; } = OutputSymbols.Create(HighlightOptions.Character);

        private static OutputSymbols Symbols_Newline { get; } = OutputSymbols.Create(HighlightOptions.Newline);

        internal static void WriteStart()
        {
            Logger.WriteLine("--- PARAMETERS ---", Verbosity);
        }

        internal static void WriteEnd()
        {
            Logger.WriteLine("--- END OF PARAMETERS ---", Verbosity);
        }

        internal static void WriteCopyCommand(CopyCommandOptions options)
        {
            WriteOption("ask", options.AskMode);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteOption("compare", options.CompareOptions);
            WriteOption("conflict resolution", options.ConflictResolution);
            WriteFilter("content filter", options.ContentFilter);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("dry run", options.DryRun);
            WriteOption("empty", options.EmptyOption);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("flat", options.Flat);
            WriteOption("highlight options", options.HighlightOptions);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteOption("max matches in file", options.MaxMatchesInFile);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("search target", options.SearchTarget);
            WriteSortOptions("sort", options.SortOptions);
            WriteOption("target", options.Target);
        }

        internal static void WriteDeleteCommand(DeleteCommandOptions options)
        {
            WriteOption("ask", options.AskMode == AskMode.File);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteFilter("content filter", options.ContentFilter);
            WriteOption("content only", options.ContentOnly);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteOption("directories only", options.DirectoriesOnly);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("dry run", options.DryRun);
            WriteOption("empty", options.EmptyOption);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("files only", options.FilesOnly);
            WriteOption("highlight options", options.HighlightOptions);
            WriteOption("including bom", options.IncludingBom);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("search target", options.SearchTarget);
            WriteSortOptions("sort", options.SortOptions);
        }

        internal static void WriteEscapeCommand(EscapeCommandOptions options)
        {
            WriteOption("char group", options.InCharGroup);
            WriteInput(options.Input);
            WriteOption("replacement", options.Replacement);
        }

        internal static void WriteFindCommand(FindCommandOptions options)
        {
            WriteOption("ask", options.AskMode);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteFilter("content filter", options.ContentFilter);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("empty", options.EmptyOption);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("highlight options", options.HighlightOptions);
            WriteInput(options.Input);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteOption("max matches in file", options.MaxMatchesInFile);
            WriteModify("modify", options.ModifyOptions);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("search target", options.SearchTarget);
            WriteOption("split", options.Split);
            WriteSortOptions("sort", options.SortOptions);
        }

        internal static void WriteHelpCommand(HelpCommandOptions options)
        {
            WriteOption("command", options.Command);
            WriteOption("manual", options.Manual);
            WriteFilter("filter", options.Filter);
        }

        internal static void WriteMatchCommand(MatchCommandOptions options)
        {
            WriteDisplayFormat("display", options.Format);
            WriteFilter("filter", options.Filter);
            WriteOption("highlight options", options.HighlightOptions);
            WriteInput(options.Input);
            WriteOption("max count", options.MaxCount);
        }

        internal static void WriteListPatternsCommand(ListPatternsCommandOptions options)
        {
            WriteOption("char", options.Value);
            WriteOption("char group", options.InCharGroup);
            WriteFilter("filter", options.Filter);
            WriteOption("regex options", options.RegexOptions);
            WriteOption("sections", options.Sections);
        }

        internal static void WriteMoveCommand(MoveCommandOptions options)
        {
            WriteOption("ask", options.AskMode);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteOption("compare", options.CompareOptions);
            WriteOption("conflict resolution", options.ConflictResolution);
            WriteFilter("content filter", options.ContentFilter);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("dry run", options.DryRun);
            WriteOption("empty", options.EmptyOption);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("flat", options.Flat);
            WriteOption("highlight options", options.HighlightOptions);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteOption("max matches in file", options.MaxMatchesInFile);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("search target", options.SearchTarget);
            WriteSortOptions("sort", options.SortOptions);
            WriteOption("target", options.Target);
        }

        internal static void WriteRenameCommand(RenameCommandOptions options)
        {
            WriteOption("ask", options.AskMode == AskMode.File);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteOption("conflict resolution", options.ConflictResolution);
            WriteFilter("content filter", options.ContentFilter);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("dry run", options.DryRun);
            WriteOption("empty", options.EmptyOption);
            WriteEvaluator("evaluator", options.ReplaceOptions.MatchEvaluator);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("highlight options", options.HighlightOptions);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteReplaceModify("modify", options.ReplaceOptions);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("replacement", options.ReplaceOptions.Replacement);
            WriteOption("search target", options.SearchTarget);
            WriteSortOptions("sort", options.SortOptions);
        }

        internal static void WriteReplaceCommand(ReplaceCommandOptions options)
        {
            WriteOption("ask", options.AskMode);
            WriteOption("attributes", options.Attributes);
            WriteOption("attributes to skip", options.AttributesToSkip);
            WriteFilter("content filter", options.ContentFilter);
            WriteEncoding("default encoding", options.DefaultEncoding);
            WriteFilter("directory filter", options.DirectoryFilter, options.DirectoryNamePart);
            WriteDisplayFormat("display", options.Format);
            WriteOption("dry run", options.DryRun);
            WriteOption("empty", options.EmptyOption);
            WriteEvaluator("evaluator", options.ReplaceOptions.MatchEvaluator);
            WriteFilter("extension filter", options.ExtensionFilter);
            WriteFilePropertyFilter(
                "file properties",
                options.SizePredicate,
                options.CreationTimePredicate,
                options.ModifiedTimePredicate);
            WriteOption("highlight options", options.HighlightOptions);
            WriteInput(options.Input);
            WriteOption("max matching files", options.MaxMatchingFiles);
            WriteOption("max matches in file", options.MaxMatchesInFile);
            WriteReplaceModify("modify", options.ReplaceOptions);
            WriteFilter("name filter", options.NameFilter, options.NamePart);
            WritePaths("paths", options.Paths);
            WriteOption("progress", options.Progress);
            WriteOption("recurse subdirectories", options.RecurseSubdirectories);
            WriteOption("replacement", options.ReplaceOptions.Replacement);
            WriteOption("search target", options.SearchTarget);
            WriteSortOptions("sort", options.SortOptions);
        }

        internal static void WriteSplitCommand(SplitCommandOptions options)
        {
            WriteDisplayFormat("display", options.Format);
            WriteFilter("filter", options.Filter);
            WriteOption("highlight options", options.HighlightOptions);
            WriteInput(options.Input);
            WriteOption("max count", options.MaxCount);
            WriteOption("omit groups", options.OmitGroups);
        }

        private static void WriteInput(string? value)
        {
            WriteName("input");

            const int maxLength = 10000;
            int remainingLength = 0;

            if (value?.Length > maxLength)
            {
                remainingLength = value.Length - maxLength;
                value = value.Remove(maxLength);
            }

            WriteValue(value);

            if (remainingLength > 0)
                Logger.Write($"<truncated> {remainingLength:n0} remaining characters", NullValueColors);

            WriteLine();
        }

        private static void WriteOption(string name, int value)
        {
            WriteName(name);
            WriteValue(value.ToString());
            WriteLine();
        }

        private static void WriteOption(string name, bool value)
        {
            WriteName(name);
            WriteValue(value);
            WriteLine();
        }

        private static void WriteOption(string name, char? value)
        {
            WriteName(name);

            if (value == null)
            {
                WriteNullValue();
            }
            else
            {
                WriteValue(value.Value.ToString());
            }

            WriteLine();
        }

        private static void WriteOption(string name, string? value, bool replaceAllSymbols = false)
        {
            WriteName(name);
            WriteValue(value, replaceAllSymbols: replaceAllSymbols);
            WriteLine();
        }

        private static void WriteOption<TEnum>(string name, TEnum value) where TEnum : Enum
        {
            WriteName(name);
            WriteValue(value.ToString());
            WriteLine();
        }

        private static void WriteOption<TEnum>(string name, ImmutableArray<TEnum> values) where TEnum : Enum
        {
            WriteName(name);

            if (values.IsEmpty)
            {
                WriteNullValue();
            }
            else
            {
                WriteValue(string.Join(", ", values));
            }

            WriteLine();
        }

        private static void WriteEncoding(string name, Encoding encoding)
        {
            WriteName(name);
            WriteValue(encoding.EncodingName);
            WriteLine();
        }

        private static void WriteFilter(string name, Filter? filter, FileNamePart? part = null)
        {
            WriteName(name);

            if (filter == null)
            {
                WriteNullValue();
                WriteLine();
                return;
            }

            WriteLine();
            WriteIndent();
            WriteName("pattern");
            WriteIndent();
            WriteLine(filter.Regex.ToString());
            WriteIndent();
            WriteName("options");
            WriteIndent();
            WriteLine(filter.Regex.Options.ToString());
            WriteIndent();
            WriteName("negative");
            WriteIndent();
            WriteLine(filter.IsNegative.ToString().ToLowerInvariant());
            WriteIndent();
            WriteName("group");

            if (string.IsNullOrEmpty(filter.GroupName))
            {
                WriteNullValue();
                WriteLine();
            }
            else
            {
                WriteIndent();
                WriteLine(filter.GroupName);
            }

            if (part != null)
            {
                WriteIndent();
                WriteName("part");
                WriteIndent();
                WriteLine(part.ToString());
            }
        }

        private static void WriteFilePropertyFilter(
            string name,
            FilterPredicate<long>? sizePredicate,
            FilterPredicate<DateTime>? creationTimePredicate,
            FilterPredicate<DateTime>? modifiedTimePredicate)
        {
            WriteName(name);

            if (sizePredicate == null
                && creationTimePredicate == null
                && modifiedTimePredicate == null)
            {
                WriteNullValue();
                WriteLine();
            }
            else
            {
                WriteLine();
                WriteIndent();
                WriteFilterPredicate("creation time", creationTimePredicate);
                WriteFilterPredicate("modified time", modifiedTimePredicate);
                WriteFilterPredicate("size", sizePredicate);
            }

            static void WriteFilterPredicate<T>(string name, FilterPredicate<T>? filterPredicate)
            {
                if (filterPredicate == null)
                    return;

                WriteIndent();
                WriteName(name);
                WriteValue(filterPredicate.Expression.Kind.ToString());
                WriteIndent();
                WriteValue(filterPredicate.Expression.Text);
                WriteLine();
            }
        }

        private static void WriteDisplayFormat(string name, OutputDisplayFormat format)
        {
            WriteName(name);

            if (format == null)
            {
                WriteNullValue();
                WriteLine();
                return;
            }

            WriteLine();
            WriteIndent();
            WriteOption("content display", format.ContentDisplayStyle);
            WriteIndent();
            WriteOption("display parts", format.DisplayParts);
            WriteIndent();
            WriteOption("file properties", format.FileProperties);
            WriteIndent();
            WriteOption("indent", format.Indent, replaceAllSymbols: true);
            WriteIndent();
            WriteOption("line options", format.LineOptions);

            if (format.LineContext.Before == format.LineContext.After)
            {
                WriteIndent();
                WriteOption("context", format.LineContext.Before);
            }
            else
            {
                WriteIndent();
                WriteOption("context before", format.LineContext.Before);
                WriteIndent();
                WriteOption("context after", format.LineContext.After);
            }

            WriteIndent();
            WriteOption("path display", format.PathDisplayStyle);
            WriteIndent();

            WriteName("separator");
            WriteValue(format.Separator, replaceAllSymbols: true);

            if (format.Separator?.EndsWith("\n") != true)
                WriteLine();

            WriteOption("no align", !format.AlignColumns);
        }

        private static void WriteSortOptions(string name, SortOptions? options)
        {
            WriteName(name);

            if (options == null)
            {
                WriteNullValue();
                WriteLine();
                return;
            }

            WriteLine();
            WriteIndent();
            WriteOption("max count", options.MaxCount);

            foreach (SortDescriptor descriptor in options.Descriptors)
            {
                WriteIndent();
                WriteName("property");
                WriteValue(descriptor.Property.ToString());
                WriteIndent();
                WriteName("direction");
                WriteValue(descriptor.Direction.ToString());
                WriteLine();
            }
        }

        private static void WritePaths(string name, ImmutableArray<PathInfo> paths)
        {
            WriteName(name);

            if (paths.Length > 1)
                WriteLine();

            foreach (PathInfo path in paths)
            {
                WriteValue($"[{path.Origin}] {path.Path}");
                WriteLine();
            }
        }

        private static void WriteEvaluator(string name, MatchEvaluator? matchEvaluator)
        {
            WriteName(name);

            if (matchEvaluator != null
                && !object.ReferenceEquals(matchEvaluator.Method.DeclaringType!.Assembly, typeof(Program).Assembly))
            {
                WriteLine();
                WriteIndent();
                WriteOption("type", matchEvaluator.Method.DeclaringType.AssemblyQualifiedName);
                WriteIndent();
                WriteOption("method", matchEvaluator.Method.Name);
            }
            else
            {
                WriteNullValue();
                WriteLine();
            }
        }

        private static void WriteReplaceModify(string name, ReplaceOptions options)
        {
            WriteName(name);

            if (options.Functions != ReplaceFunctions.None)
            {
                if (options.CultureInvariant)
                {
                    WriteValue(nameof(options.CultureInvariant) + ", " + options.Functions.ToString());
                }
                else
                {
                    WriteValue(options.Functions.ToString());
                }
            }
            else if (options.CultureInvariant)
            {
                WriteValue(nameof(options.CultureInvariant));
            }
            else
            {
                WriteNullValue();
            }

            WriteLine();
        }

        private static void WriteModify(string name, ModifyOptions options)
        {
            WriteName(name);

            if (options.Aggregate
                || options.CultureInvariant
                || options.IgnoreCase
                || options.Functions != ModifyFunctions.None
                || options.SortProperty != ValueSortProperty.None)
            {
                if (options.Aggregate)
                {
                    WriteLine();
                    WriteIndent();
                    WriteName("aggregate");
                    WriteValue(options.Aggregate);
                }

                if (options.CultureInvariant)
                {
                    WriteLine();
                    WriteIndent();
                    WriteName("culture invariant");
                    WriteValue(options.CultureInvariant);
                }

                if (options.IgnoreCase)
                {
                    WriteLine();
                    WriteIndent();
                    WriteName("ignore case");
                    WriteValue(options.IgnoreCase);
                }

                if (options.SortProperty != ValueSortProperty.None)
                {
                    WriteLine();
                    WriteIndent();
                    WriteName("sort by");
                    WriteValue(options.SortProperty.ToString());
                }

                WriteLine();
                WriteIndent();
                WriteName("functions");

                if (options.Functions != ModifyFunctions.None)
                {
                    WriteValue(options.Functions.ToString());
                }
                else
                {
                    WriteNullValue();
                }
            }
            else
            {
                WriteNullValue();
            }

            WriteLine();
        }

        private static void WriteValue(bool value)
        {
            WriteIndent();
            Write((value) ? "true" : "false");
        }

        private static void WriteValue(string? value, bool replaceAllSymbols = false)
        {
            if (value == null)
            {
                WriteNullValue();
            }
            else
            {
                WriteIndent();

                if (replaceAllSymbols)
                {
                    ValueWriter.Write(value, Symbols_Character, ValueColors);
                }
                else
                {
                    ValueWriter.Write(value, Symbols_Newline, ValueColors);
                }
            }
        }

        private static void WriteIndent()
        {
            Write("  ");
        }

        private static void WriteName(string name)
        {
            Logger.Write(name, Verbosity);
            Logger.Write(":", Verbosity);
        }

        private static void WriteNullValue()
        {
            WriteIndent();
            Logger.Write("<none>", NullValueColors, Verbosity);
        }

        private static void Write(string value)
        {
            Logger.Write(value, ValueColors, Verbosity);
        }

        private static void WriteLine()
        {
            Logger.WriteLine(Verbosity);
        }

        private static void WriteLine(string? value)
        {
            Logger.WriteLine(value, ValueColors, Verbosity);
        }
    }
}
