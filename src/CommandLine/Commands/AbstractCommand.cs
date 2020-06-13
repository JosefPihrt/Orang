// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal abstract class AbstractCommand<TOptions> where TOptions : AbstractCommandOptions
    {
        protected AbstractCommand(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }

        protected abstract CommandResult ExecuteCore(CancellationToken cancellationToken = default);

        public CommandResult Execute()
        {
            CancellationTokenSource? cts = null;

            try
            {
                cts = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                CancellationToken cancellationToken = cts.Token;

                try
                {
                    return ExecuteCore(cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    OperationCanceled(ex);
                }
                catch (AggregateException ex)
                {
                    OperationCanceledException? operationCanceledException = ex.GetOperationCanceledException();

                    if (operationCanceledException != null)
                    {
                        OperationCanceled(operationCanceledException);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                cts?.Dispose();
            }

            return CommandResult.Canceled;
        }

        protected virtual void OperationCanceled(OperationCanceledException ex)
        {
            OperationCanceled();
        }

        protected virtual void OperationCanceled()
        {
            WriteLine();
            WriteLine("Operation was canceled.");
        }
    }
}
