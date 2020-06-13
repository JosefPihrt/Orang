// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Orang.FileSystem;

namespace Orang.Operations
{
    internal class DeleteOperation : DeleteOrRenameOperation
    {
        public override OperationKind OperationKind => OperationKind.Delete;

        public DeleteOptions DeleteOptions { get; set; } = null!;

        protected override void ExecuteMatch(
            FileMatch fileMatch,
            string directoryPath)
        {
            bool deleted = false;

            try
            {
                if (!DryRun)
                {
                    FileSystemHelpers.Delete(
                        fileMatch,
                        contentOnly: DeleteOptions.ContentOnly,
                        includingBom: DeleteOptions.IncludingBom,
                        filesOnly: DeleteOptions.FilesOnly,
                        directoriesOnly: DeleteOptions.DirectoriesOnly);

                    deleted = true;
                }

                Report(fileMatch);

                Telemetry.IncrementProcessedCount(fileMatch.IsDirectory);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                Report(fileMatch, ex);
            }

            if (fileMatch.IsDirectory
                && deleted)
            {
                OnDirectoryChanged(new DirectoryChangedEventArgs(fileMatch.Path, null));
            }
        }
    }
}
