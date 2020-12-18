// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.Expressions
{
    internal class BinaryExpression : Expression
    {
        public BinaryExpression(
            string text,
            string identifier,
            string value,
            ExpressionKind kind) : base(text, identifier, value)
        {
            Kind = kind;
        }

        public override ExpressionKind Kind { get; }
    }
}
