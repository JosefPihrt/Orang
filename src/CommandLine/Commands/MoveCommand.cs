// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Orang.CommandLine
{
    internal class MoveCommand : CommonCopyCommand<MoveCommandOptions>
    {
        public MoveCommand(MoveCommandOptions options) : base(options)
        {
        }

        protected override string GetQuestionText(bool isDirectory)
        {
            return (isDirectory) ? "Move directory?" : "Move file?";
        }

        protected override void ExecuteOperation(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }

        protected override void WriteSummary(SearchTelemetry telemetry, Verbosity verbosity)
        {
            base.WriteSummary(telemetry, verbosity);

            ConsoleColors colors = (Options.DryRun) ? Colors.Message_DryRun : Colors.Message_Change;

            if (telemetry.ProcessedFileCount > 0)
                LogHelpers.WriteCount("Moved files", telemetry.ProcessedFileCount, colors, verbosity);

            if (telemetry.ProcessedDirectoryCount > 0)
            {
                if (telemetry.ProcessedFileCount > 0)
                    Logger.Write("  ", verbosity);

                LogHelpers.WriteCount("Moved directories", telemetry.ProcessedDirectoryCount, colors, verbosity);
            }

            Logger.WriteLine(verbosity);
        }
    }
}
