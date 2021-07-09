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
#if DEBUG // short command syntax
            if (args?.Length > 0)
            {
                switch (args[0])
                {
                    case "f":
                        {
                            ReplaceArgs("find");
                            break;
                        }
                    case "r":
                        {
                            ReplaceArgs("replace");
                            break;
                        }
                }

                void ReplaceArgs(string commandName)
                {
                    Array.Resize(ref args, args.Length + 1);

                    for (int i = args.Length - 1; i >= 2; i--)
                        args[i] = args[i - 1];

                    args[0] = commandName;
                    args[1] = "-c";
                }
            }
#endif
            try
            {
                Parser parser = CreateParser(ignoreUnknownArguments: true);

                if (args == null
                    || args.Length == 0)
                {
                    HelpCommand.WriteCommandsHelp();
                    return ExitCodes.Match;
                }

                var success = true;
                var help = false;

                ParserResult<BaseCommandLineOptions> defaultResult = parser
                    .ParseArguments<BaseCommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        if (!options.Help)
                            return;

                        string? commandName = args?.FirstOrDefault();
                        Command? command = (commandName != null)
                            ? CommandLoader.LoadCommand(typeof(Program).Assembly, commandName)
                            : null;

                        success = ParseVerbosityAndOutput(options);

                        if (!success)
                            return;

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
                if (!success)
                    return ExitCodes.Error;

                if (help)
                    return ExitCodes.Match;

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
                    SpellcheckCommandLineOptions,
                    RenameCommandLineOptions,
                    ReplaceCommandLineOptions,
                    SplitCommandLineOptions,
                    SyncCommandLineOptions
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
                    return ExitCodes.Error;

                parserResult.WithParsed<AbstractCommandLineOptions>(options =>
                {
                    success = ParseVerbosityAndOutput(options);

                    if (success)
                        WriteArgs(args);
                });

                if (!success)
                    return ExitCodes.Error;

                return parserResult.MapResult(
                    (CopyCommandLineOptions options) => Copy(options),
                    (MoveCommandLineOptions options) => Move(options),
                    (SyncCommandLineOptions options) => Sync(options),
                    (DeleteCommandLineOptions options) => Delete(options),
                    (EscapeCommandLineOptions options) => Escape(options),
                    (FindCommandLineOptions options) => Find(options),
                    (HelpCommandLineOptions options) => Help(options),
                    (ListPatternsCommandLineOptions options) => ListPatterns(options),
                    (MatchCommandLineOptions options) => Match(options),
                    (SpellcheckCommandLineOptions options) => Spellcheck(options),
                    (RenameCommandLineOptions options) => Rename(options),
                    (ReplaceCommandLineOptions options) => Replace(options),
                    (SplitCommandLineOptions options) => Split(options),
                    _ => ExitCodes.Error);
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

            return ExitCodes.Error;
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
                if (!TryParseOutputOptions(
                    baseOptions.Output,
                    OptionNames.Output,
                    out string? filePath,
                    out Verbosity fileVerbosity,
                    out Encoding? encoding,
                    out bool append))
                {
                    return false;
                }

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
                return ExitCodes.Error;

            return Execute(new CopyCommand(options), commandLineOptions);
        }

        private static int Delete(DeleteCommandLineOptions commandLineOptions)
        {
            var options = new DeleteCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new DeleteCommand(options), commandLineOptions);
        }

        private static int Escape(EscapeCommandLineOptions commandLineOptions)
        {
            var options = new EscapeCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new EscapeCommand(options), commandLineOptions);
        }

        private static int Find(FindCommandLineOptions commandLineOptions)
        {
            var options = new FindCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new FindCommand<FindCommandOptions>(options), commandLineOptions);
        }

        private static int Help(HelpCommandLineOptions commandLineOptions)
        {
            var options = new HelpCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new HelpCommand(options), commandLineOptions);
        }

        private static int ListPatterns(ListPatternsCommandLineOptions commandLineOptions)
        {
            var options = new ListPatternsCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new ListPatternsCommand(options), commandLineOptions);
        }

        private static int Match(MatchCommandLineOptions commandLineOptions)
        {
            var options = new MatchCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new MatchCommand(options), commandLineOptions);
        }

        private static int Move(MoveCommandLineOptions commandLineOptions)
        {
            var options = new MoveCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new MoveCommand(options), commandLineOptions);
        }

        private static int Spellcheck(SpellcheckCommandLineOptions commandLineOptions)
        {
            var options = new SpellcheckCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new SpellcheckCommand(options), commandLineOptions);
        }

        private static int Sync(SyncCommandLineOptions commandLineOptions)
        {
            var options = new SyncCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new SyncCommand(options), commandLineOptions);
        }

        private static int Rename(RenameCommandLineOptions commandLineOptions)
        {
            var options = new RenameCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new RenameCommand(options), commandLineOptions);
        }

        private static int Replace(ReplaceCommandLineOptions commandLineOptions)
        {
            var options = new ReplaceCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new ReplaceCommand(options), commandLineOptions);
        }

        private static int Split(SplitCommandLineOptions commandLineOptions)
        {
            var options = new SplitCommandOptions();

            if (!commandLineOptions.TryParse(options))
                return ExitCodes.Error;

            return Execute(new SplitCommand(options), commandLineOptions);
        }

        private static int Execute<TOptions>(
            AbstractCommand<TOptions> command,
            AbstractCommandLineOptions options) where TOptions : AbstractCommandOptions
        {
#if DEBUG
            if (ShouldLog(Verbosity.Diagnostic))
            {
                WriteLine("--- RAW PARAMETERS ---", Verbosity.Diagnostic);
                DiagnosticWriter.WriteParameters(options);
                WriteLine("--- END OF RAW PARAMETERS ---", Verbosity.Diagnostic);

                WriteLine("--- APP PARAMETERS ---", Verbosity.Diagnostic);
                command.Options.WriteDiagnostic();
                WriteLine("--- END OF APP PARAMETERS ---", Verbosity.Diagnostic);
            }
#endif
            CommandResult result = command.Execute();

            switch (result)
            {
                case CommandResult.Success:
                    return ExitCodes.Match;
                case CommandResult.NoMatch:
                    return ExitCodes.NoMatch;
                case CommandResult.Fail:
                case CommandResult.Canceled:
                    return ExitCodes.Error;
                default:
                    throw new InvalidOperationException($"Unknown enum value '{result}'.");
            }
        }

        private static class ExitCodes
        {
            public const int Match = 0;
            public const int NoMatch = 1;
            public const int Error = 2;
        }
    }
}
