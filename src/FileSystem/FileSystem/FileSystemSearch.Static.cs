// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;

#pragma warning disable RCS1223

namespace Orang.FileSystem
{
    public partial class FileSystemSearch
    {
        internal static void Replace(
            string directoryPath,
            FileSystemFilter filter,
            ReplaceOptions? replaceOptions = null,
            SearchTarget searchTarget = SearchTarget.Files,
            bool recurseSubdirectories = true,
            bool dryRun = false,
            CancellationToken cancellationToken = default)
        {
            var searchOptions = new FileSystemSearchOptions(
                searchTarget: searchTarget,
                recurseSubdirectories: recurseSubdirectories);

            var search = new FileSystemSearch(
                filter,
                directoryFilter: null,
                searchProgress: null,
                options: searchOptions);

            search.Replace(
                directoryPath,
                replaceOptions,
                progress: null,
                dryRun: dryRun,
                cancellationToken: cancellationToken);
        }
    }
}
