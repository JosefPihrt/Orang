// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            //WriteLine($"Orang Command Line Tool version {typeof(Program).GetTypeInfo().Assembly.GetName().Version}");
            //WriteLine("Copyright (c) Josef Pihrt. All rights reserved.");
            //WriteLine();

            try
            {
                Parser parser = CreateParser(ignoreUnknownArguments: true);

                bool help = false;

                ParserResult<BaseCommandLineOptions> defaultResult = parser
                    .ParseArguments<BaseCommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        if (!options.Help)
                            return;

                        string? commandName = args?.FirstOrDefault();
                        Command? command = (commandName != null) ? CommandLoader.LoadCommand(typeof(Program).Assembly, commandName) : null;

                        ParseVerbosityAndOutput(options);
                        WriteArgs(args);

                        if (command != null)
                        {
                            HelpCommand.WriteCommandHelp(command);
                        }
                        else
                        {
                            HelpCommand.WriteCommandsHelp();
                        }

                        help = true;
                    })
#if DEBUG
                    .WithNotParsed(_ =>
                    {
                    });
#else
                    ;
#endif

                if (help)
                    return 0;

                bool success = true;

                parser = CreateParser();

                ParserResult<object> parserResult = parser.ParseArguments<
                    CopyCommandLineOptions,
                    DeleteCommandLineOptions,
                    EscapeCommandLineOptions,
                    FindCommandLineOptions,
                    HelpCommandLineOptions,
                    ListPatternsCommandLineOptions,
                    MatchCommandLineOptions,
                    MoveCommandLineOptions,
                    RenameCommandLineOptions,
                    ReplaceCommandLineOptions,
                    SplitCommandLineOptions
                    >(args);

                parserResult.WithNotParsed(_ =>
                {
                    var helpText = new HelpText(SentenceBuilder.Create(), HelpCommand.GetHeadingText());

                    helpText = HelpText.DefaultParsingErrorsHandler(parserResult, helpText);

                    VerbAttribute? verbAttribute = parserResult.TypeInfo.Current.GetCustomAttribute<VerbAttribute>();

                    if (verbAttribute != null)
                    {
                        helpText.AddPreOptionsText(Environment.NewLine + HelpCommand.GetFooterText(verbAttribute.Name));
                    }

                    Console.Error.WriteLine(helpText);

                    success = false;
                });

                if (!success)
                    return 2;

                parserResult.WithParsed<AbstractCommandLineOptions>(options =>
                {
                    success = ParseVerbosityAndOutput(options);
                    WriteArgs(args);
                });

                if (!success)
                    return 2;

                return parserResult.MapResult(
                    (CopyCommandLineOptions options) => Copy(options),
                    (MoveCommandLineOptions options) => Move(options),
                    (DeleteCommandLineOptions options) => Delete(options),
                    (EscapeCommandLineOptions options) => Escape(options),
                    (FindCommandLineOptions options) => Find(options),
                    (HelpCommandLineOptions options) => Help(options),
                    (ListPatternsCommandLineOptions options) => ListPatterns(options),
                    (MatchCommandLineOptions options) => Match(options),
                    (RenameCommandLineOptions options) => Rename(options),
                    (ReplaceCommandLineOptions options) => Replace(options),
                    (SplitCommandLineOptions options) => Split(options),
                    _ => 2);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
            finally
            {
                Out?.Dispose();
                Out = null;
            }

            return 2;
        }

        private static Parser CreateParser(bool? ignoreUnknownArguments = null)
        {
            ParserSettings defaultSettings = Parser.Default.Settings;

            return new Parser(settings =>
            {
                settings.AutoHelp = false;
                settings.AutoVersion = defaultSettings.AutoVersion;
                settings.CaseInsensitiveEnumValues = defaultSettings.CaseInsensitiveEnumValues;
                settings.CaseSensitive = defaultSettings.CaseSensitive;
                settings.EnableDashDash = true;
                settings.HelpWriter = null;
                settings.IgnoreUnknownArguments = ignoreUnknownArguments ?? defaultSettings.IgnoreUnknownArguments;
                settings.MaximumDisplayWidth = defaultSettings.MaximumDisplayWidth;
                settings.ParsingCulture = defaultSettings.ParsingCulture;
            });
        }

        private static bool ParseVerbosityAndOutput(AbstractCommandLineOptions options)
        {
            var defaultVerbosity = Verbosity.Normal;

            if (options.Verbosity != null
                && !TryParseVerbosity(options.Verbosity, out defaultVerbosity))
            {
                return false;
            }

            ConsoleOut.Verbosity = defaultVerbosity;

            if (options is BaseCommandLineOptions baseOptions)
            {
                if (!TryParseOutputOptions(baseOptions.Output, OptionNames.Output, out string? filePath, out Verbosity fileVerbosity, out Encoding? encoding, out bool append))
                    return false;

                if (filePath != null)
                {
                    FileMode fileMode = (append)
                        ? FileMode.Append
                        : FileMode.Create;

                    var stream = new FileStream(filePath, fileMode, FileAccess.Write, FileShare.Read);
                    var writer = new StreamWriter(stream, encoding, bufferSize: 4096, leaveOpen: false);
                    Out = new TextWriterWithVerbosity(writer) { Verbosity = fileVerbosity };
                }
            }

            return true;
        }

        [Conditional("DEBUG")]
        private static void WriteArgs(string[]? args)
        {
            if (args != null)
            {
                WriteLine("--- ARGS ---", Verbosity.Diagnostic);

                foreach (string arg in args)
                    WriteLine(arg, Verbosity.Diagnostic);

                WriteLine("--- END OF ARGS ---", Verbosity.Diagnostic);
            }
        }

        private static int Copy(CopyCommandLineOptions commandLineOptions)
        {
            var options = new CopyCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 1;

            return Execute(new CopyCommand(options));
        }

        private static int Delete(DeleteCommandLineOptions commandLineOptions)
        {
            var options = new DeleteCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new DeleteCommand(options));
        }

        private static int Escape(EscapeCommandLineOptions commandLineOptions)
        {
            var options = new EscapeCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new EscapeCommand(options));
        }

        private static int Find(FindCommandLineOptions commandLineOptions)
        {
            var options = new FindCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new FindCommand<FindCommandOptions>(options));
        }

        private static int Help(HelpCommandLineOptions commandLineOptions)
        {
            var options = new HelpCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new HelpCommand(options));
        }

        private static int ListPatterns(ListPatternsCommandLineOptions commandLineOptions)
        {
            var options = new ListPatternsCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new ListPatternsCommand(options));
        }

        private static int Match(MatchCommandLineOptions commandLineOptions)
        {
            var options = new MatchCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new MatchCommand(options));
        }

        private static int Move(MoveCommandLineOptions commandLineOptions)
        {
            var options = new MoveCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 1;

            return Execute(new MoveCommand(options));
        }

        private static int Rename(RenameCommandLineOptions commandLineOptions)
        {
            var options = new RenameCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new RenameCommand(options));
        }

        private static int Replace(ReplaceCommandLineOptions commandLineOptions)
        {
            var options = new ReplaceCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new ReplaceCommand(options));
        }

        private static int Split(SplitCommandLineOptions commandLineOptions)
        {
            var options = new SplitCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return 2;

            return Execute(new SplitCommand(options));
        }

        private static int Execute<TOptions>(AbstractCommand<TOptions> command) where TOptions : AbstractCommandOptions
        {
#if DEBUG
            if (ShouldLog(Verbosity.Diagnostic))
                command.Options.WriteDiagnostic();
#endif
            CommandResult result = command.Execute();

            switch (result)
            {
                case CommandResult.Success:
                    return 0;
                case CommandResult.NoMatch:
                    return 1;
                case CommandResult.Fail:
                case CommandResult.Canceled:
                    return 2;
                default:
                    throw new InvalidOperationException($"Unknown enum value '{result}'.");
            }
        }
    }
}
