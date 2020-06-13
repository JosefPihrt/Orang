// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Orang.FileSystem;

namespace Orang.Operations
{
    internal class CopyOperation : CommonCopyOperation
    {
        public override OperationKind OperationKind => OperationKind.Copy;

        protected override void ExecuteOperation(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
        }
    }
}
