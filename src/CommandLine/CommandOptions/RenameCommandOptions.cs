// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal sealed class RenameCommandOptions : DeleteOrRenameCommandOptions
    {
        internal RenameCommandOptions()
        {
        }

        public bool Interactive { get; internal set; }

        public ConflictResolution ConflictResolution { get; internal set; }

        public ReplaceOptions ReplaceOptions { get; internal set; } = null!;

        protected override void WriteDiagnosticCore()
        {
            DiagnosticWriter.WriteRenameCommand(this);
        }
    }
}
