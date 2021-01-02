// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal static class OptionHelpText
    {
        public const string Modifier = "<MODIFIER> is either expression-body of a method with signature "
            + "'IEnumerable<string> M(IEnumerable<string> items)' "
            + "or a path to a code file that contains public method with signature 'IEnumerable<string> M(IEnumerable<string> items)'. "
            + "Imported namespaces are (when inline expression is used): "
            + "System, "
            + "System.Collections.Generic, "
            + "System.Linq, "
            + "System.Text, "
            + "System.Text.RegularExpressions.";
    }
}
