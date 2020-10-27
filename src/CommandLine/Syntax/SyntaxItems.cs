// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Orang.Syntax
{
    internal static class SyntaxItems
    {
        private static ImmutableArray<SyntaxItem> _values;

        public static IEnumerable<SyntaxItem> Load()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                Path.Combine("Resources", "RegexSyntax.xml"));

            return XDocument.Load(path)
                .Root
                .Elements()
                .Select(f => new SyntaxItem(
                    f.Element("Text").Value,
                    (SyntaxSection)Enum.Parse(typeof(SyntaxSection), f.Element("Category").Value),
                    f.Element("Description").Value));
        }

        public static ImmutableArray<SyntaxItem> Values
        {
            get
            {
                if (_values.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _values, Load().ToImmutableArray());
                }

                return _values;
            }
        }
    }
}
