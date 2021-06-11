// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;
using DotMarkdown.Linq;
using Orang.CommandLine;
using static DotMarkdown.Linq.MFactory;
using Orang.CommandLine.Help;

namespace Orang.Documentation
{
    internal static class Program
    {
        private static readonly Regex _removeNewlineRegex = new Regex(@"\ *\r?\n\ *");

        private static void Main(params string[] args)
        {
            IEnumerable<Command> commands = CommandLoader.LoadCommands(typeof(CommandLoader).Assembly)
                .Select(c => c.WithOptions(c.Options.OrderBy(f => f, CommandOptionComparer.Name)))
                .OrderBy(c => c.Name, StringComparer.InvariantCulture);

            var application = new CommandLineApplication(
                "orang",
                "Search, replace, rename and delete files and its content using the power of .NET regular expressions.",
                commands.OrderBy(f => f.Name, StringComparer.InvariantCulture));

            string? destinationDirectoryPath = null;
            string? dataDirectoryPath = null;

            if (Debugger.IsAttached)
            {
                destinationDirectoryPath = (args.Length > 0) ? args[0] : @"..\..\..\..\..\docs\cli";
                dataDirectoryPath = @"..\..\..\data";
            }
            else
            {
                destinationDirectoryPath = args[0];
                dataDirectoryPath = @"..\src\DocumentationGenerator\data";
            }

            string readmeFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "README.md"));

            var settings = new MarkdownWriterSettings(
                MarkdownFormat.Default.WithTableOptions(
                    MarkdownFormat.Default.TableOptions | TableOptions.FormatHeaderAndContent));

            using (var sw = new StreamWriter(readmeFilePath, append: false, Encoding.UTF8))
            using (MarkdownWriter mw = MarkdownWriter.Create(sw, settings))
            {
                mw.WriteStartHeading(1);
                mw.WriteString("Orang Command-Line Interface");
                mw.WriteRaw(" <img align=\"left\" src=\"../../images/icon48.png\">");
                mw.WriteEndHeading();
                mw.WriteString(application.Description);
                mw.WriteLine();

                mw.WriteHeading2("Commands");

                Table(
                    TableRow("Command", "Description"),
                    application
                        .Commands
                        .Select(f => TableRow(Link(f.Name, f.Name + "-command.md"), f.Description)))
                    .WriteTo(mw);

                mw.WriteLine();

                string readmeLinksFilePath = Path.Combine(dataDirectoryPath, "readme_bottom.md");

                if (File.Exists(readmeLinksFilePath))
                    mw.WriteRaw(File.ReadAllText(readmeLinksFilePath));

                WriteFootNote(mw);

                Console.WriteLine(readmeFilePath);
            }

            string valuesFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "OptionValues.md"));

            ImmutableArray<OptionValueProvider> providers = OptionValueProvider.GetProviders(
                commands.SelectMany(f => f.Options),
                OptionValueProviders.ProvidersByName.Select(f => f.Value))
                .ToImmutableArray();

            MDocument document = Document(
                Heading1("List of Option Values"),
                BulletList(providers.Select(f => Link(
                    f.Name,
                    MarkdownHelpers.CreateGitHubHeadingLink(f.Name)))),
                providers.Select(provider =>
                {
                    return new MObject[]
                    {
                        Heading2(provider.Name),
                        Table(
                            TableRow("Value", " ", "Description"),
                            provider.Values.Select(f => CreateTableRow(f, providers)))
                    };
                }));

            document.Add(
                Heading2("Expression Syntax"),
                Table(
                    TableRow("Expression", "Description"),
                    HelpProvider.GetExpressionItems(includeDate: true)
                        .Select(f => TableRow(InlineCode(f.expression), f.description))));

            AddFootnote(document);

            var markdownFormat = new MarkdownFormat(
                tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatContent);

            File.WriteAllText(valuesFilePath, document.ToString(markdownFormat));

            foreach (Command command in application.Commands)
            {
                readmeFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, $"{command.Name}-command.md"));

                using (var sw = new StreamWriter(readmeFilePath, append: false, Encoding.UTF8))
                using (MarkdownWriter mw = MarkdownWriter.Create(sw))
                {
                    var writer = new DocumentationWriter(mw);

                    writer.WriteCommandHeading(command, application);
                    writer.WriteCommandDescription(command);

                    mw.WriteLink("Home", "README.md#readme");

                    foreach (string section in new[] { "Synopsis", "Arguments", "Options", "Samples" })
                    {
                        mw.WriteString(" ");
                        mw.WriteCharEntity((char)0x2022);
                        mw.WriteString(" ");
                        mw.WriteLink(section, "#" + section);
                    }

                    mw.WriteLine();

                    writer.WriteCommandSynopsis(command, application);
                    writer.WriteArguments(command.Arguments);
                    writer.WriteOptions(command.Options);

                    string samplesFilePath = Path.Combine(dataDirectoryPath, command.Name + "_bottom.md");

                    if (File.Exists(samplesFilePath))
                    {
                        string content = File.ReadAllText(samplesFilePath);
                        mw.WriteRaw(content);
                    }

                    WriteFootNote(mw);

                    Console.WriteLine(readmeFilePath);
                }
            }

            Console.WriteLine("Done");

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private static MTableRow CreateTableRow(
            OptionValue optionValue,
            ImmutableArray<OptionValueProvider> providers)
        {
            object? value = null;
            string? shortValue = null;

            switch (optionValue)
            {
                case SimpleOptionValue simpleOptionValue:
                    {
                        value = simpleOptionValue.Value;
                        shortValue = simpleOptionValue.ShortValue;
                        break;
                    }
                case KeyValuePairOptionValue keyValuePairOptionValue:
                    {
                        string value2 = keyValuePairOptionValue.Value;

                        if (OptionValueProvider.MetaValueRegex.IsMatch(value2)
                            && providers.Any(f => f.Name == value2))
                        {
                            value = Inline(
                                $"{keyValuePairOptionValue.Key}=",
                                Link(value2, MarkdownHelpers.CreateGitHubHeadingLink(value2)));
                        }
                        else
                        {
                            value = $"{keyValuePairOptionValue.Key}={value2}";
                        }

                        shortValue = keyValuePairOptionValue.ShortKey;
                        break;
                    }
            }

            string description = _removeNewlineRegex.Replace(optionValue.Description ?? "", " ");

            return TableRow(
                value,
                (string.IsNullOrEmpty(shortValue)) ? " " : shortValue,
                (string.IsNullOrEmpty(description)) ? " " : description);
        }

        private static void WriteFootNote(MarkdownWriter mw)
        {
            mw.WriteLine();
            mw.WriteStartItalic();
            mw.WriteString("(Generated with ");
            mw.WriteLink("DotMarkdown", "http://github.com/JosefPihrt/DotMarkdown");
            mw.WriteString(")");
            mw.WriteEndItalic();
        }

        private static void AddFootnote(MDocument document)
        {
            document.Add(Italic("(Generated with ", Link("DotMarkdown", "http://github.com/JosefPihrt/DotMarkdown"), ")"));
        }
    }
}
