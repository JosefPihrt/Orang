// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using Orang;
using Orang.FileSystem;

namespace N
{
    public class C
    {
        public void M()
        {
            var regex = new Regex("pattern", RegexOptions.IgnoreCase);

            var name = new Filter(regex);

            var filter = new FileSystemFilter(name);

            var searchOptions = new FileSystemSearchOptions(recurseSubdirectories: false);

            var search = new FileSystemSearch(filter, options: searchOptions);

            search.Replace("directoryPath", ReplaceOptions.Empty);
        }

        public void M2()
        {
            var name = new Filter("pattern", RegexOptions.IgnoreCase);

            var filter = new FileSystemFilter(name);

            //FileSystemSearch.Replace("directoryPath", filter, recurseSubdirectories: false);
        }

        public static void SM()
        {
        }
    }
}
