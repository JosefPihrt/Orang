// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Orang.FileSystem;

namespace Orang.CommandLine
{
    internal sealed class SearchContext
    {
        public SearchContext(
            SearchTelemetry telemetry,
            ProgressReporter? progress = null,
            List<SearchResult>? results = null,
            in CancellationToken cancellationToken = default)
        {
            Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            Progress = progress;
            Results = results;
            CancellationToken = cancellationToken;
        }

        public SearchTelemetry Telemetry { get; }

        public ProgressReporter? Progress { get; }

        public List<SearchResult>? Results { get; }

        public Dictionary<string, long>? DirectorySizeMap { get; set; }

        public CancellationToken CancellationToken { get; }

        public TerminationReason TerminationReason { get; set; }
    }
}
