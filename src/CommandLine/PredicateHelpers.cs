// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Orang.Expressions;

namespace Orang.CommandLine
{
    internal static class PredicateHelpers
    {
        public static Func<string, bool> GetLengthPredicate(Expression expression)
        {
            if (expression.Kind == ExpressionKind.IntervalExpression)
            {
                var intervalExpression = (IntervalExpression)expression;

                BinaryExpression left = intervalExpression.Left;
                BinaryExpression right = intervalExpression.Right;

                int value1 = Parse(left.Value);
                int value2 = Parse(right.Value);

                switch (left.Kind)
                {
                    case ExpressionKind.GreaterThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => f.Length > value1 && f.Length < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => f.Length > value1 && f.Length <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => f.Length < value1 && f.Length > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => f.Length < value1 && f.Length >= value2;
                            }

                            break;
                        }
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => f.Length >= value1 && f.Length < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => f.Length >= value1 && f.Length <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => f.Length <= value1 && f.Length > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => f.Length <= value1 && f.Length >= value2;
                            }

                            break;
                        }
                }
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                int value = Parse(binaryExpression.Value);

                switch (binaryExpression.Kind)
                {
                    case ExpressionKind.EqualsExpression:
                        return f => f.Length == value;
                    case ExpressionKind.GreaterThanExpression:
                        return f => f.Length > value;
                    case ExpressionKind.LessThanExpression:
                        return f => f.Length < value;
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        return f => f.Length >= value;
                    case ExpressionKind.LessThanOrEqualExpression:
                        return f => f.Length <= value;
                }
            }

            throw new ArgumentException("", nameof(expression));

            int Parse(string value)
            {
                if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int result))
                    throw new ArgumentException("", nameof(expression));

                return result;
            }
        }

        public static Func<long, bool> GetLongPredicate(Expression expression)
        {
            if (expression.Kind == ExpressionKind.IntervalExpression)
            {
                var intervalExpression = (IntervalExpression)expression;

                BinaryExpression left = intervalExpression.Left;
                BinaryExpression right = intervalExpression.Right;

                long value1 = Parse(left.Value);
                long value2 = Parse(right.Value);

                switch (left.Kind)
                {
                    case ExpressionKind.GreaterThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => f > value1 && f < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => f > value1 && f <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => f < value1 && f > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => f < value1 && f >= value2;
                            }

                            break;
                        }
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => f >= value1 && f < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => f >= value1 && f <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => f <= value1 && f > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => f <= value1 && f >= value2;
                            }

                            break;
                        }
                }
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                long value = Parse(binaryExpression.Value);

                switch (binaryExpression.Kind)
                {
                    case ExpressionKind.EqualsExpression:
                        return f => f == value;
                    case ExpressionKind.GreaterThanExpression:
                        return f => f > value;
                    case ExpressionKind.LessThanExpression:
                        return f => f < value;
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        return f => f >= value;
                    case ExpressionKind.LessThanOrEqualExpression:
                        return f => f <= value;
                }
            }

            throw new ArgumentException("", nameof(expression));

            long Parse(string value)
            {
                if (!long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long result))
                    throw new ArgumentException("", nameof(expression));

                return result;
            }
        }

        public static Func<DateTime, bool> GetDateTimePredicate(Expression expression)
        {
            if (expression.Kind == ExpressionKind.IntervalExpression)
            {
                var intervalExpression = (IntervalExpression)expression;

                BinaryExpression left = intervalExpression.Left;
                BinaryExpression right = intervalExpression.Right;

                DateTime value1 = Parse(left.Value, out bool dateOnly1);
                DateTime value2 = Parse(right.Value, out bool dateOnly2);

                switch (left.Kind)
                {
                    case ExpressionKind.GreaterThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => ((dateOnly1) ? f.Date : f) > value1 && ((dateOnly2) ? f.Date : f) < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => ((dateOnly1) ? f.Date : f) > value1 && ((dateOnly2) ? f.Date : f) <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => ((dateOnly1) ? f.Date : f) < value1 && ((dateOnly2) ? f.Date : f) > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => ((dateOnly1) ? f.Date : f) < value1 && ((dateOnly2) ? f.Date : f) >= value2;
                            }

                            break;
                        }
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.LessThanExpression:
                                    return f => ((dateOnly1) ? f.Date : f) >= value1 && ((dateOnly2) ? f.Date : f) < value2;
                                case ExpressionKind.LessThanOrEqualExpression:
                                    return f => ((dateOnly1) ? f.Date : f) >= value1 && ((dateOnly2) ? f.Date : f) <= value2;
                            }

                            break;
                        }
                    case ExpressionKind.LessThanOrEqualExpression:
                        {
                            switch (right.Kind)
                            {
                                case ExpressionKind.GreaterThanExpression:
                                    return f => ((dateOnly1) ? f.Date : f) <= value1 && ((dateOnly2) ? f.Date : f) > value2;
                                case ExpressionKind.GreaterThanOrEqualExpression:
                                    return f => ((dateOnly1) ? f.Date : f) <= value1 && ((dateOnly2) ? f.Date : f) >= value2;
                            }

                            break;
                        }
                }
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                DateTime value = Parse(binaryExpression.Value, out bool dateOnly);

                switch (binaryExpression.Kind)
                {
                    case ExpressionKind.EqualsExpression:
                        return f => ((dateOnly) ? f.Date : f) == value;
                    case ExpressionKind.GreaterThanExpression:
                        return f => ((dateOnly) ? f.Date : f) > value;
                    case ExpressionKind.LessThanExpression:
                        return f => ((dateOnly) ? f.Date : f) < value;
                    case ExpressionKind.GreaterThanOrEqualExpression:
                        return f => ((dateOnly) ? f.Date : f) >= value;
                    case ExpressionKind.LessThanOrEqualExpression:
                        return f => ((dateOnly) ? f.Date : f) <= value;
                }
            }
            else if (expression.Kind == ExpressionKind.DecrementExpression)
            {
                var decrementExpression = (DecrementExpression)expression;

                if (!TimeSpan.TryParse(decrementExpression.Value, CultureInfo.InvariantCulture, out TimeSpan result))
                    throw new ArgumentException("", nameof(expression));

                DateTime dateTime = DateTime.Now - result;

                return f => f > dateTime;
            }

            throw new ArgumentException("", nameof(expression));

            DateTime Parse(string value, out bool dateOnly)
            {
                if (value == null)
                {
                    dateOnly = false;
                    return DateTime.MinValue;
                }

                if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    throw new ArgumentException("", nameof(expression));

                dateOnly = result.TimeOfDay.Duration() == TimeSpan.Zero;

                return result;
            }
        }
    }
}
