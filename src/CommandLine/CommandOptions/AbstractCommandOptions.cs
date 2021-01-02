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
            Console.WriteLine("Press any key to continue...");
            Console.Read();
#endif
        }

        protected abstract void WriteDiagnosticCore();
    }
}
