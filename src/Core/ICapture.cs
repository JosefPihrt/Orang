// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang
{
    public interface ICapture
    {
        string Value { get; }

        int Index { get; }

        int Length { get; }
    }
}