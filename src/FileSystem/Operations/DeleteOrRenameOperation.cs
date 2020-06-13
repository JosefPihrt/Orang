// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Orang.FileSystem;

namespace Orang.Operations
{
    internal abstract class DeleteOrRenameOperation : FileSystemOperation, INotifyDirectoryChanged
    {
        protected DeleteOrRenameOperation()
        {
        }

        public event EventHandler<DirectoryChangedEventArgs>? DirectoryChanged;

        protected virtual void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }
    }
}
