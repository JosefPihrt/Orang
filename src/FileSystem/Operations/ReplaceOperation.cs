// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Orang.FileSystem;

#pragma warning disable IDE0068

namespace Orang.Operations
{
    internal class ReplaceOperation : CommonFindOperation
    {
        public override OperationKind OperationKind => OperationKind.Replace;

        public ReplaceOptions ReplaceOptions { get; set; } = null!;

        protected override void ExecuteDirectory(string path)
        {
            Debug.Assert(ContentFilter?.IsNegative == false);
        }

        protected override void ExecuteMatch(
            FileMatch fileMatch,
            string directoryPath)
        {
            TextWriter? textWriter = null;
            List<Capture>? captures = null;

            try
            {
                captures = ListCache<Capture>.GetInstance();

                GetCaptures(
                    fileMatch.ContentMatch!,
                    ContentFilter!.GroupNumber,
                    predicate: ContentFilter.Predicate,
                    captures: captures);

                if (!DryRun)
                {
                    textWriter = new StreamWriter(fileMatch.Path, false, fileMatch.Encoding);

                    WriteMatches(fileMatch.ContentText, captures, ReplaceOptions, textWriter);
                }

                int fileMatchCount = captures.Count;
                int fileReplacementCount = fileMatchCount;
                Telemetry.MatchCount += fileMatchCount;
                Telemetry.ProcessedMatchCount += fileReplacementCount;

                if (fileReplacementCount > 0)
                    Telemetry.ProcessedFileCount++;

                Report(fileMatch);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                Report(fileMatch, ex);
            }
            finally
            {
                textWriter?.Dispose();

                if (captures != null)
                    ListCache<Capture>.Free(captures);
            }
        }

        private void WriteMatches(
            string input,
            IEnumerable<Capture> captures,
            ReplaceOptions replaceOptions,
            TextWriter textWriter)
        {
            int index = 0;

            foreach (Capture capture in captures)
            {
                textWriter.Write(input.AsSpan(index, capture.Index - index));

                string result = replaceOptions.Replace((Match)capture);

                textWriter.Write(result);

                index = capture.Index + capture.Length;
            }

            textWriter.Write(input.AsSpan(index, input.Length - index));
        }
    }
}
