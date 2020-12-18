// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Orang.Text.RegularExpressions;

namespace Orang
{
    internal sealed class SplitOutputInfo
    {
        private SplitOutputInfo(
            int splitWidth,
            int indexWidth,
            int lengthWidth,
            OutputCaptions? captions = null,
            string? prefixSeparator = null)
        {
            SplitWidth = splitWidth;
            IndexWidth = indexWidth;
            LengthWidth = lengthWidth;
            Captions = captions ?? OutputCaptions.Default;
            PrefixSeparator = prefixSeparator ?? ":";
        }

        public int SplitWidth { get; }

        public int IndexWidth { get; }

        public int LengthWidth { get; }

        private OutputCaptions Captions { get; }

        private string PrefixSeparator { get; }

        public int Width
        {
            get
            {
                return ((PrefixSeparator.Length + 2) * 3)
                    + SplitWidth
                    + IndexWidth
                    + LengthWidth;
            }
        }

        public static SplitOutputInfo Create(
            SplitData splitData,
            OutputCaptions? captions = null,
            string? prefixSeparator = null)
        {
            int splitCount = 0;
            int maxGroupNameLength = 0;
            int maxIndex = 0;
            int maxLength = 0;

            foreach (SplitItem splitItem in splitData.Items)
            {
                if (splitItem.IsGroup)
                {
                    if (!splitData.OmitGroups)
                        maxGroupNameLength = Math.Max(maxGroupNameLength, splitItem.Name.Length);
                }
                else
                {
                    splitCount++;
                }

                maxIndex = Math.Max(maxIndex, splitItem.Index);
                maxLength = Math.Max(maxLength, splitItem.Length);
            }

            int splitWidth = Math.Max(splitCount.GetDigitCount(), maxGroupNameLength);
            int indexWidth = (maxIndex > 0) ? maxIndex.GetDigitCount() : 0;
            int lengthWidth = (maxLength > 0) ? maxLength.GetDigitCount() : 0;

            return new SplitOutputInfo(splitWidth, indexWidth, lengthWidth, captions, prefixSeparator);
        }

        public string GetText(SplitItem item)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append((item.IsGroup) ? Captions.ShortGroup : Captions.ShortSplit);
            sb.Append(PrefixSeparator);

            if (item.IsGroup)
            {
                sb.Append(item.Name);
            }
            else
            {
                sb.Append(item.Number);
            }

            sb.Append(' ', SplitWidth - item.Name.Length);
            sb.Append(" ");

            sb.Append(Captions.ShortIndex);
            sb.Append(PrefixSeparator);

            sb.Append(' ', IndexWidth - item.Index.GetDigitCount());
            sb.Append(item.Index);
            sb.Append(" ");

            sb.Append(Captions.ShortLength);
            sb.Append(PrefixSeparator);

            sb.Append(' ', LengthWidth - item.Length.GetDigitCount());
            sb.Append(item.Length);
            sb.Append(" ");

            return StringBuilderCache.GetStringAndFree(sb);
        }
    }
}
