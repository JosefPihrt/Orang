// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal abstract class DeleteOrRenameCommand<TOptions> :
        CommonFindCommand<TOptions>,
        INotifyDirectoryChanged
        where TOptions : DeleteOrRenameCommandOptions
    {
        protected DeleteOrRenameCommand(TOptions options) : base(options)
        {
        }

        public event EventHandler<DirectoryChangedEventArgs>? DirectoryChanged;

        protected virtual void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }

        protected sealed override void ExecuteResult(SearchResult result, SearchContext context, ColumnWidths? columnWidths)
        {
            ExecuteMatchCore(result.FileMatch, context, result.BaseDirectoryPath, columnWidths);
        }
    }
}
