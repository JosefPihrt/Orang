// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.Expressions
{
    internal class IntervalExpression : Expression
    {
        public IntervalExpression(
            string text,
            string identifier,
            string value,
            BinaryExpression left,
            BinaryExpression right) : base(text, identifier, value)
        {
            Left = left;
            Right = right;
        }

        public override ExpressionKind Kind => ExpressionKind.IntervalExpression;

        public BinaryExpression Left { get; }

        public BinaryExpression Right { get; }
    }
}
