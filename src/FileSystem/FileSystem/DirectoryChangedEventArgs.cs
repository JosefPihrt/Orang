// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang.FileSystem
{
    internal class DirectoryChangedEventArgs : EventArgs
    {
        public DirectoryChangedEventArgs(string name, string? newName)
        {
            Name = name;
            NewName = newName;
        }

        public string Name { get; }

        public string? NewName { get; }
    }
}
