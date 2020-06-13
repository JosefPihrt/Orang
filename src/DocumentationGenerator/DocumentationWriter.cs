// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using DotMarkdown;
using Orang.CommandLine;

namespace Orang.Documentation
{
    internal class DocumentationWriter : MarkdownDocumentationWriter
    {
        private static readonly Regex _metaValueRegex = new Regex(@"\<\w+_OPTIONS\>");

        public DocumentationWriter(MarkdownWriter writer) : base(writer)
        {
        }

        public override void WriteOptionDescription(CommandOption option)
        {
            _writer.WriteString(option.Description);

            OptionValueProvider provider = null!;

            if (TryGetProvider())
            {
                if (!string.IsNullOrEmpty(option.Description))
                    _writer.WriteString(" ");

                _writer.WriteString((provider.Values.Length == 1) ? "Allowed value is " : "Allowed values are ");
                _writer.WriteString(OptionValueProviders.GetHelpText(provider, f => !f.Hidden));
                _writer.WriteString(".");
            }

            if (!string.IsNullOrEmpty(option.Description)
                || provider != null)
            {
                _writer.WriteLine();
                _writer.WriteLine();
            }

            bool TryGetProvider()
            {
                if (option.ValueProviderName != null)
                    return OptionValueProviders.ProvidersByName.TryGetValue(option.ValueProviderName, out provider);

                if (option.MetaValue != null
                    && OptionValueProviders.ProvidersByName.TryGetValue(option.MetaValue, out provider))
                {
                    return true;
                }

                Match match = _metaValueRegex.Match(option.Description);

                return match.Success
                    && OptionValueProviders.ProvidersByName.TryGetValue(match.Value, out provider);
            }
        }
    }
}
