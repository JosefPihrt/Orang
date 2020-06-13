// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Orang.Text.RegularExpressions
{
    internal sealed class CaptureItem
    {
        internal CaptureItem(Capture capture, GroupItem groupItem)
        {
            Capture = capture;
            GroupItem = groupItem;
        }

        public string Value => Capture.Value;

        public int Index => Capture.Index;

        public int Length => Capture.Length;

        public Capture Capture { get; }

        public GroupItem GroupItem { get; }

        public MatchItem MatchItem => GroupItem.MatchItem;

        public int GroupNumber => GroupDefinition.Number;

        public string GroupName => GroupDefinition.Name;

        public GroupDefinition GroupDefinition => GroupItem.GroupDefinition;

        public override string ToString() => Value;
    }
}