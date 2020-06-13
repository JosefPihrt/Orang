// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

#pragma warning disable RCS1223

namespace Orang.FileSystem
{
    public class FileSystemFilter
    {
        public FileSystemFilter(
            Filter? name = null,
            FileNamePart part = FileNamePart.Name,
            Filter? extension = null,
            Filter? content = null,
            FilePropertyFilter? properties = null,
            FileAttributes attributes = default,
            FileAttributes attributesToSkip = default,
            FileEmptyOption emptyOption = FileEmptyOption.None)
        {
            Name = name;
            Part = part;
            Extension = extension;
            Content = content;
            Properties = properties;
            Attributes = attributes;
            AttributesToSkip = attributesToSkip;
            EmptyOption = emptyOption;
        }

        public Filter? Name { get; }

        public FileNamePart Part { get; }

        public Filter? Extension { get; }

        public Filter? Content { get; }

        public FilePropertyFilter? Properties { get; }

        public FileAttributes Attributes { get; }

        public FileAttributes AttributesToSkip { get; }

        public FileEmptyOption EmptyOption { get; }
    }
}
