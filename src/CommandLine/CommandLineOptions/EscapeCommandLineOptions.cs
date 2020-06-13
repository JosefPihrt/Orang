// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Orang.CommandLine
{
    [Verb("escape", HelpText = "Escapes special characters by replacing them with their escape codes.")]
    internal sealed class EscapeCommandLineOptions : BaseCommandLineOptions
    {
        [Option(shortName: OptionShortNames.Input, longName: OptionNames.Input,
            HelpText = "Text to be escaped.",
            MetaValue = MetaValues.Input)]
        public string Input { get; set; } = null!;

        [Option(longName: OptionNames.CharGroup,
            HelpText = "Text is part of a character group.")]
        public bool CharGroup { get; set; }

        [Option(shortName: OptionShortNames.Replacement, longName: OptionNames.Replacement,
            HelpText = "Text is a replacement string.")]
        public bool Replacement { get; set; }

        public bool TryParse(EscapeCommandOptions options)
        {
            string input = Input;

            if (input == null)
            {
                string? redirectedInput = ConsoleHelpers.ReadRedirectedInput();

                if (redirectedInput == null)
                {
                    Logger.WriteError("Input is missing.");
                    return false;
                }

                input = redirectedInput;
            }

            options.Input = input;
            options.InCharGroup = CharGroup;
            options.Replacement = Replacement;

            return true;
        }
    }
}
