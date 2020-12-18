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

            using (var sw = new StreamWriter(readmeFilePath, append: false, Encoding.UTF8))
            using (MarkdownWriter mw = MarkdownWriter.Create(sw))
            {
                mw.WriteHeading1("Orang Command-Line Interface");
                mw.WriteString(application.Description);
                mw.WriteLine();

                mw.WriteHeading2("Commands");

                foreach (Command command in application.Commands)
                {
                    mw.WriteStartBulletItem();
                    mw.WriteLink(command.Name, command.Name + "-command.md");
                    mw.WriteEndBulletItem();
                }

                mw.WriteLine();

                string readmeLinksFilePath = Path.Combine(dataDirectoryPath, "readme_bottom.md");

                if (File.Exists(readmeLinksFilePath))
                    mw.WriteRaw(File.ReadAllText(readmeLinksFilePath));

                WriteFootNote(mw);

                Console.WriteLine(readmeFilePath);
            }

            string valuesFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "AllowedValues.md"));

            ImmutableArray<OptionValueProvider> providers = OptionValueProvider.GetProviders(
                commands.SelectMany(f => f.Options),
                OptionValueProviders.ProvidersByName.Select(f => f.Value))
                .ToImmutableArray();

            MDocument document = Document(
                Heading1("List of Allowed Values"),
                BulletList(providers.Select(f => Link(
                    f.Name,
                    "#"
                        + f.Name
                            .ToLower()
                            .Replace("<", "")
                            .Replace(">", "")
                            .Replace("_", "-")))),
                providers.Select(provider =>
                {
                    return new MObject[]
                    {
                        Heading2(provider.Name),
                        Table(
                            TableRow("Value", "Description"),
                            provider.Values.Select(
                                f => TableRow(f.HelpValue, _removeNewlineRegex.Replace(f.Description ?? "", " "))))
                    };
                }));

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
