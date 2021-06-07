// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Orang.Text.RegularExpressions;

namespace Orang
{
    internal sealed class MatchOutputInfo
    {
        private const string ItemSeparator = " ";

        private string? _matchSpaces;
        private string? _groupSpaces;

        private MatchOutputInfo(
            int groupNumber,
            int matchWidth,
            int groupWidth,
            int captureWidth,
            int indexWidth,
            int lengthWidth,
            OutputCaptions? captions = null,
            string? prefixSeparator = null)
        {
            GroupNumber = groupNumber;
            MatchWidth = matchWidth;
            GroupWidth = groupWidth;
            CaptureWidth = captureWidth;
            IndexWidth = indexWidth;
            LengthWidth = lengthWidth;
            Captions = captions ?? OutputCaptions.Default;
            PrefixSeparator = prefixSeparator ?? ":";
        }

        public int GroupNumber { get; }

        public int MatchWidth { get; }

        public int GroupWidth { get; }

        public int CaptureWidth { get; }

        public int IndexWidth { get; }

        public int LengthWidth { get; }

        public OutputCaptions Captions { get; }

        public string PrefixSeparator { get; }

        private string MatchSpaces
        {
            get
            {
                return _matchSpaces ??= new string(
                    ' ',
                    MatchWidth
                        + 1
                        + PrefixSeparator.Length
                        + ItemSeparator.Length);
            }
        }

        private string GroupSpaces
        {
            get
            {
                return _groupSpaces ??= new string(
                    ' ',
                    GroupWidth
                        + 1
                        + PrefixSeparator.Length
                        + ItemSeparator.Length);
            }
        }

        public int Width
        {
            get
            {
                int minWidth = PrefixSeparator.Length + ItemSeparator.Length + 1;

                int width = 0;

                if (MatchWidth > 0)
                    width += MatchWidth + minWidth;

                if (GroupWidth > 0)
                    width += GroupWidth + minWidth;

                if (CaptureWidth > 0)
                    width += CaptureWidth + minWidth;

                if (IndexWidth > 0)
                    width += IndexWidth + minWidth;

                if (LengthWidth > 0)
                    width += LengthWidth + minWidth;

                return width;
            }
        }

        public static MatchOutputInfo Create(
            MatchData matchData,
            int groupNumber = -1,
            bool includeGroupNumber = true,
            bool includeCaptureNumber = true,
            OutputCaptions? captions = null,
            string? prefixSeparator = null)
        {
            int matchWidth = Math.Max(matchData.Items.Count, 0).GetDigitCount();
            int groupWidth = 0;
            int captureWidth = 0;
            int indexWidth = 0;
            int lengthWidth = 0;

            int maxGroupNameLength = 0;
            int maxCaptureCount = 0;
            int maxIndex = 0;
            int maxLength = 0;

            foreach (MatchItem matchItem in matchData.Items)
            {
                foreach (GroupItem groupItem in matchItem.GroupItems)
                {
                    if (!groupItem.Success)
                        continue;

                    if (groupNumber >= 0
                        && groupNumber != groupItem.Number)
                    {
                        continue;
                    }

                    if (includeGroupNumber)
                        maxGroupNameLength = Math.Max(maxGroupNameLength, groupItem.Name.Length);

                    if (includeCaptureNumber)
                        maxCaptureCount = Math.Max(maxCaptureCount, groupItem.Captures.Count);

                    foreach (Capture? capture in groupItem.Captures)
                    {
                        maxIndex = Math.Max(maxIndex, capture!.Index);
                        maxLength = Math.Max(maxLength, capture.Length);
                    }
                }
            }

            if (maxGroupNameLength > 0)
                groupWidth = maxGroupNameLength;

            if (maxCaptureCount > 1)
                captureWidth = maxCaptureCount.GetDigitCount();

            if (maxIndex > 0)
                indexWidth = maxIndex.GetDigitCount();

            if (maxLength > 0)
                lengthWidth = maxLength.GetDigitCount();

            return new MatchOutputInfo(
                groupNumber,
                matchWidth,
                groupWidth,
                captureWidth,
                indexWidth,
                lengthWidth,
                captions,
                prefixSeparator);
        }

        public static MatchOutputInfo Create(
            List<ICapture> splits,
            OutputCaptions? captions = null,
            string? prefixSeparator = null)
        {
            int maxIndex = 0;
            int maxLength = 0;

            foreach (ICapture capture in splits)
            {
                maxIndex = Math.Max(maxIndex, capture.Index);
                maxLength = Math.Max(maxLength, capture.Length);
            }

            return new MatchOutputInfo(
                groupNumber: -1,
                matchWidth: Math.Max(splits.Count, 0).GetDigitCount(),
                groupWidth: 0,
                captureWidth: 0,
                maxIndex.GetDigitCount(),
                maxLength.GetDigitCount(),
                captions,
                prefixSeparator);
        }

        public string GetText(
            CaptureItem item,
            int matchNumber,
            int captureNumber,
            bool omitMatchInfo = false,
            bool omitGroupInfo = false)
        {
            return GetText(item.Capture, matchNumber, item.GroupName, captureNumber, omitMatchInfo, omitGroupInfo);
        }

        public string GetText(
            Capture capture,
            int matchNumber,
            string? groupName,
            int captureNumber,
            bool omitMatchInfo = false,
            bool omitGroupInfo = false)
        {
            return GetText(
                index: capture.Index,
                length: capture.Length,
                matchNumber: matchNumber,
                groupName: groupName,
                captureNumber: captureNumber,
                omitMatchInfo: omitMatchInfo,
                omitGroupInfo: omitGroupInfo);
        }

        public string GetText(
            ICapture capture,
            int matchNumber,
            string? groupName)
        {
            return GetText(
                index: capture.Index,
                length: capture.Length,
                matchNumber: matchNumber,
                groupName: groupName,
                captureNumber: -1,
                omitMatchInfo: false,
                omitGroupInfo: false);
        }

        public string GetText(
            int index,
            int length,
            int matchNumber,
            string? groupName,
            int captureNumber,
            bool omitMatchInfo = false,
            bool omitGroupInfo = false)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            if (omitMatchInfo)
            {
                sb.Append(MatchSpaces);
            }
            else
            {
                AppendNumber(Captions.ShortMatch, matchNumber, MatchWidth);
            }

            if (groupName != null)
            {
                if (omitGroupInfo)
                {
                    sb.Append(GroupSpaces);
                }
                else
                {
                    Append(Captions.ShortGroup, groupName, GroupWidth);
                }
            }

            if (captureNumber != -1
                && CaptureWidth > 0)
            {
                AppendNumber(Captions.ShortCapture, captureNumber, CaptureWidth);
            }

            AppendNumber(Captions.ShortIndex, index, IndexWidth);
            AppendNumber(Captions.ShortLength, length, LengthWidth);

            return StringBuilderCache.GetStringAndFree(sb);

            void Append(string caption, string value, int width)
            {
                if (caption.Length > 0)
                {
                    sb.Append(caption);
                    sb.Append(PrefixSeparator);
                }

                sb.Append(value);
                sb.Append(' ', width - value.Length);
                sb.Append(ItemSeparator);
            }

            void AppendNumber(string caption, int number, int width)
            {
                if (caption.Length > 0)
                {
                    sb.Append(caption);
                    sb.Append(PrefixSeparator);
                }

                sb.Append(' ', width - number.GetDigitCount());
                sb.Append(number);
                sb.Append(ItemSeparator);
            }
        }
    }
}
