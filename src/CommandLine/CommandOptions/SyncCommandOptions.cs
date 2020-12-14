﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Orang.CommandLine
{
    internal sealed class SyncCommandOptions : CommonCopyCommandOptions
    {
        internal SyncCommandOptions()
        {
        }

        new public SyncConflictResolution ConflictResolution { get; internal set; }

        //TODO: SyncCommandOptions.WriteDiagnosticCore
        protected override void WriteDiagnosticCore()
        {
        }
    }
}
