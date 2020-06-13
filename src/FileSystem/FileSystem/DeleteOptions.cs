// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Orang.FileSystem
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class DeleteOptions
    {
        internal static DeleteOptions Default { get; } = new DeleteOptions();

        public DeleteOptions(
            bool contentOnly = false,
            bool includingBom = false,
            bool filesOnly = false,
            bool directoriesOnly = false)
        {
            ContentOnly = contentOnly;
            IncludingBom = includingBom;
            FilesOnly = filesOnly;
            DirectoriesOnly = directoriesOnly;
        }

        public bool ContentOnly { get; }

        public bool IncludingBom { get; }

        public bool FilesOnly { get; }

        public bool DirectoriesOnly { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{nameof(ContentOnly)} = {ContentOnly}  {nameof(IncludingBom)} = {IncludingBom}";
    }
}
