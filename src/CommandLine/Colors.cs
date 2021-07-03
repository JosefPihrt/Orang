// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orang
{
    internal static class Colors
    {
        public static ConsoleColors BasePath { get; } = new ConsoleColors(ConsoleColor.DarkGray);
        public static ConsoleColors ContextLine { get; } = new ConsoleColors(ConsoleColor.DarkGray);
        public static ConsoleColors EmptyMatch { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors EmptyReplacement { get; } = new ConsoleColors(ConsoleColor.Cyan);
        public static ConsoleColors EmptySplit { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors InvalidReplacement { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Yellow);
        public static ConsoleColors LineNumber { get; } = new ConsoleColors(ConsoleColor.Cyan);
        public static ConsoleColors Match { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Green);
        public static ConsoleColors Match_Path { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors MatchBoundary { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors Matched_Path { get; } = new ConsoleColors(ConsoleColor.Cyan);
        public static ConsoleColors Message_OK { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors Message_Change { get; } = new ConsoleColors(ConsoleColor.Cyan);
        public static ConsoleColors Message_DryRun { get; } = new ConsoleColors(ConsoleColor.DarkGray);
        public static ConsoleColors Message_Warning { get; } = new ConsoleColors(ConsoleColor.Yellow);
        public static ConsoleColors Path_Progress { get; } = new ConsoleColors(ConsoleColor.DarkGray);
        public static ConsoleColors Replacement { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Cyan);
        public static ConsoleColors ReplacementBoundary { get; } = new ConsoleColors(ConsoleColor.Cyan);
        public static ConsoleColors Split { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Green);
        public static ConsoleColors SplitBoundary { get; } = new ConsoleColors(ConsoleColor.Green);
        public static ConsoleColors Symbol { get; } = new ConsoleColors(ConsoleColor.Red);
        public static ConsoleColors Sync_Add { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Green);
        public static ConsoleColors Sync_Error { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Yellow);
        public static ConsoleColors Sync_Delete { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Magenta);
        public static ConsoleColors Sync_Update { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Cyan);
        public static ConsoleColors Sync_Rename { get; } = new ConsoleColors(ConsoleColor.Black, ConsoleColor.Cyan);
        public static ConsoleColors Syntax { get; } = new ConsoleColors(ConsoleColor.Green);
    }
}
