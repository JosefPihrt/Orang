// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal sealed class SyncCommandOptions : CommonCopyCommandOptions
    {
        internal SyncCommandOptions()
        {
        }

        new public SyncConflictResolution ConflictResolution { get; internal set; }

        public bool DetectRename { get; internal set; }

        protected override void WriteDiagnosticCore()
        {
            DiagnosticWriter.WriteSyncCommand(this);
        }
    }
}
