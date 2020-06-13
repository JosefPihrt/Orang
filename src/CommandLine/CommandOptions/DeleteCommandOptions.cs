// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal sealed class DeleteCommandOptions : DeleteOrRenameCommandOptions
    {
        internal DeleteCommandOptions()
        {
        }

        public bool ContentOnly { get; internal set; }

        public bool IncludingBom { get; internal set; }

        public bool FilesOnly { get; internal set; }

        public bool DirectoriesOnly { get; internal set; }

        protected override void WriteDiagnosticCore()
        {
            DiagnosticWriter.WriteDeleteCommand(this);
        }
    }
}
