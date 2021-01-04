// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.CommandLine
{
    internal abstract class AbstractCommandOptions
    {
        internal void WriteDiagnostic()
        {
            DiagnosticWriter.WriteStart();
            WriteDiagnosticCore();
            DiagnosticWriter.WriteEnd();
#if DEBUG
            if (Logger.ConsoleOut.Verbosity == Verbosity.Diagnostic)
                ConsoleHelpers.WaitForKeyPress();
#endif
        }

        protected abstract void WriteDiagnosticCore();
    }
}
