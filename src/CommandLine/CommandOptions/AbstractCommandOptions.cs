// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal abstract class AbstractCommandOptions
    {
        internal void WriteDiagnostic()
        {
            WriteDiagnosticCore();
#if DEBUG
            if (Logger.ConsoleOut.Verbosity == Verbosity.Diagnostic)
                ConsoleHelpers.WaitForKeyPress();
#endif
        }

        protected abstract void WriteDiagnosticCore();
    }
}
