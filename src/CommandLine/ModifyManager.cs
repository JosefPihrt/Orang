// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Orang.Aggregation;

namespace Orang.CommandLine
{
    internal class ModifyManager
    {
        private OutputSymbols? _symbols;

        public ModifyManager(FindCommandOptions options)
        {
            Options = options;
        }

        public FindCommandOptions Options { get; }

        public List<string>? FileValues { get; private set; }

        internal IResultStorage? FileStorage { get; private set; }

        internal OutputSymbols Symbols
        {
            get
            {
                if (_symbols == null)
                {
                    HighlightOptions highlightOptions = Options.HighlightOptions;

                    if (Options.ContentDisplayStyle != ContentDisplayStyle.Value
                        && Options.ContentDisplayStyle != ContentDisplayStyle.ValueDetail)
                    {
                        highlightOptions &= ~HighlightOptions.Boundary;
                    }

                    _symbols = OutputSymbols.Create(highlightOptions);
                }

                return _symbols;
            }
        }

        public void Reset()
        {
            if (FileValues == null)
            {
                FileValues = new List<string>();
            }
            else
            {
                FileValues.Clear();
            }

            if (FileStorage == null)
                FileStorage = new ListResultStorage(FileValues);
        }
    }
}
