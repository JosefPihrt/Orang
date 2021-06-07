// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Orang.Text.RegularExpressions;

namespace Orang.CommandLine
{
    internal abstract class ContentWriter : IDisposable
    {
        protected ContentWriter(
            string input,
            ContentWriterOptions options) : this(input, ContentTextWriter.Default, options)
        {
        }

        protected ContentWriter(
            string input,
            ContentTextWriter? writer,
            ContentWriterOptions options)
        {
            Input = input;
            Options = options;
            MatchingLineCount = -1;
            Writer = writer;
        }

        public string Input { get; }

        public ContentWriterOptions Options { get; }

        public int MatchCount { get; private set; }

        public int MatchingLineCount { get; protected set; }

        protected ContentTextWriter? Writer { get; }

        protected abstract ValueWriter ValueWriter { get; }

        public OutputSymbols Symbols => Options.Symbols;

        public ConsoleColors MatchColors => (Options.HighlightMatch) ? Colors.Match : default;

        public ConsoleColors ReplacementColors => (Options.HighlightReplacement) ? Colors.Replacement : default;

        public ConsoleColors MatchBoundaryColors => (Options.HighlightBoundary) ? Colors.MatchBoundary : default;

        public ConsoleColors ReplacementBoundaryColors => (Options.HighlightBoundary) ? Colors.ReplacementBoundary : default;

        private ConsoleColors NoReplacementColors => (Options.HighlightReplacement) ? Colors.Match : default;

        private ConsoleColors NoReplacementBoundaryColors => (Options.HighlightBoundary) ? Colors.MatchBoundary : default;

        public static ContentWriter CreateFind(
            ContentDisplayStyle contentDisplayStyle,
            string input,
            ContentWriterOptions options,
            IResultStorage storage,
            MatchOutputInfo outputInfo,
            bool ask = false)
        {
            return CreateFind(
                contentDisplayStyle,
                input,
                options,
                storage,
                outputInfo,
                ContentTextWriter.Default,
                ask);
        }

        public static ContentWriter CreateFind(
            ContentDisplayStyle contentDisplayStyle,
            string input,
            ContentWriterOptions options,
            IResultStorage? storage,
            MatchOutputInfo? outputInfo,
            ContentTextWriter? writer,
            bool ask = false)
        {
            if (ask)
            {
                switch (contentDisplayStyle)
                {
                    case ContentDisplayStyle.Value:
                    case ContentDisplayStyle.ValueDetail:
                        return new AskValueContentWriter(input, options, storage, outputInfo);
                    case ContentDisplayStyle.Line:
                        return new AskLineContentWriter(input, options, storage);
                    case ContentDisplayStyle.UnmatchedLines:
                    case ContentDisplayStyle.AllLines:
                        throw new InvalidOperationException();
                    default:
                        throw new InvalidOperationException($"Unknown enum value '{contentDisplayStyle}'.");
                }
            }
            else
            {
                switch (contentDisplayStyle)
                {
                    case ContentDisplayStyle.Value:
                    case ContentDisplayStyle.ValueDetail:
                        return new ValueContentWriter(input, writer, options, storage, outputInfo);
                    case ContentDisplayStyle.Line:
                        return new LineContentWriter(input, writer, options, storage);
                    case ContentDisplayStyle.UnmatchedLines:
                        return new UnmatchedLineWriter(input, writer, options, storage);
                    case ContentDisplayStyle.AllLines:
                        return new AllLinesContentWriter(input, writer, options, storage);
                    default:
                        throw new InvalidOperationException($"Unknown enum value '{contentDisplayStyle}'.");
                }
            }
        }

        public static ContentWriter CreateReplace(
            ContentDisplayStyle contentDisplayStyle,
            string input,
            IReplacer replacer,
            ContentWriterOptions options,
            TextWriter? textWriter = null,
            MatchOutputInfo? outputInfo = null,
            SpellcheckState? spellcheckState = null)
        {
            switch (contentDisplayStyle)
            {
                case ContentDisplayStyle.Value:
                case ContentDisplayStyle.ValueDetail:
                    return new ValueReplacementWriter(input, replacer, options, textWriter, outputInfo, spellcheckState);
                case ContentDisplayStyle.Line:
                    return new LineReplacementWriter(input, replacer, options, textWriter, spellcheckState);
                case ContentDisplayStyle.AllLines:
                    return new AllLinesReplacementWriter(input, replacer, options, textWriter, spellcheckState);
                case ContentDisplayStyle.UnmatchedLines:
                    throw new NotSupportedException($"Value '{contentDisplayStyle}' is not supported.");
                default:
                    throw new InvalidOperationException($"Unknown enum value '{contentDisplayStyle}'.");
            }
        }

        protected abstract void WriteStartMatches();

        protected abstract void WriteEndMatches();

        protected abstract void WriteStartMatch(ICapture capture);

        protected abstract void WriteEndMatch(ICapture capture);

        protected abstract void WriteStartReplacement(ICapture capture, string? result);

        protected abstract void WriteEndReplacement(ICapture capture, string? result);

        protected abstract void WriteMatchSeparator();

        public abstract void Dispose();

        protected void Write(string? value)
        {
            Writer?.Write(value);
        }

        protected void Write(string? value, in ConsoleColors colors)
        {
            Writer?.Write(value, colors);
        }

        protected void Write(string? value, int startIndex, int length)
        {
            Writer?.Write(value, startIndex, length);
        }

        protected void Write(string? value, int startIndex, int length, in ConsoleColors colors)
        {
            Writer?.Write(value, startIndex, length, colors);
        }

        protected void WriteLine()
        {
            Writer?.WriteLine();
        }

        protected void WriteLineNumber(int value)
        {
            Write(value.ToString(), Colors.LineNumber);
            Write(" ");
        }

        protected virtual void WriteMatchValue(ICapture capture)
        {
            if (capture.Length > 0)
            {
                WriteNonEmptyMatchValue(capture);
            }
            else if (Options.HighlightEmptyMatch)
            {
                Write("|", Colors.EmptyMatch);
            }
        }

        protected virtual void WriteNonEmptyMatchValue(ICapture capture)
        {
            ValueWriter.Write(
                Input,
                capture.Index,
                capture.Length,
                Symbols,
                MatchColors,
                MatchBoundaryColors);
        }

        protected virtual void WriteReplacementValue(ICapture capture, string? result)
        {
            string replacement = result ?? capture.Value;

            if (replacement.Length > 0)
            {
                if (result != null)
                {
                    WriteNonEmptyReplacementValue(replacement, ReplacementColors, ReplacementBoundaryColors);
                }
                else
                {
                    WriteNonEmptyReplacementValue(replacement, NoReplacementColors, NoReplacementBoundaryColors);
                }
            }
            else if (Options.HighlightEmptyReplacement)
            {
                Write("|", (result != null) ? Colors.EmptyReplacement : Colors.EmptyMatch);
            }
        }

        protected virtual void WriteNonEmptyReplacementValue(
            string result,
            in ConsoleColors colors,
            in ConsoleColors boundaryColors)
        {
            ValueWriter.Write(result, Symbols, colors, boundaryColors);
        }

        public virtual void WriteMatches(IEnumerable<Capture> matches, in CancellationToken cancellationToken = default)
        {
            MatchCount = 0;

            WriteStartMatches();

            try
            {
                using (IEnumerator<Capture> en = matches.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteMatch(new RegexCapture(en.Current));
                            MatchCount++;

                            if (en.MoveNext())
                            {
                                WriteMatchSeparator();
                            }
                            else
                            {
                                break;
                            }

                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                WriteEndMatches();
            }
        }

        public virtual void WriteMatches(IEnumerable<ICapture> matches, in CancellationToken cancellationToken = default)
        {
            MatchCount = 0;

            WriteStartMatches();

            try
            {
                using (IEnumerator<ICapture> en = matches.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteMatch(en.Current);
                            MatchCount++;

                            if (en.MoveNext())
                            {
                                WriteMatchSeparator();
                            }
                            else
                            {
                                break;
                            }

                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                WriteEndMatches();
            }
        }

        internal void WriteMatches(IEnumerator<ICapture> en, in CancellationToken cancellationToken = default)
        {
            MatchCount = 0;

            WriteStartMatches();

            try
            {
                while (true)
                {
                    WriteMatch(en.Current);
                    MatchCount++;

                    if (en.MoveNext())
                    {
                        WriteMatchSeparator();
                    }
                    else
                    {
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                WriteEndMatches();
            }
        }

        protected virtual void WriteMatch(ICapture capture)
        {
            WriteStartMatch(capture);

            WriteMatchValue(capture);

            WriteEndMatch(capture);
        }

        protected virtual void WriteReplacement(ICapture capture, string? result)
        {
            WriteStartReplacement(capture, result);

            WriteReplacementValue(capture, result);

            WriteEndReplacement(capture, result);
        }

        protected void WriteStartLine(int index, int endIndex)
        {
            if (Options.TrimLine)
            {
                while (index < endIndex
                    && char.IsWhiteSpace(Input[index]))
                {
                    index++;
                }
            }

            if (index < endIndex)
            {
                if (Input[endIndex - 1] == '\r')
                    endIndex--;

                Write(Input, index, endIndex - index);
            }
        }

        protected void WriteEndLine(int index, int endIndex)
        {
            if (endIndex > 0
                && Input[endIndex - 1] == '\n')
            {
                endIndex--;

                if (endIndex > 0
                    && Input[endIndex - 1] == '\r')
                {
                    endIndex--;
                }
            }

            if (Options.TrimLine)
            {
                while (endIndex > index
                    && char.IsWhiteSpace(Input[endIndex - 1]))
                {
                    endIndex--;
                }
            }

            if (endIndex > index)
                Write(Input, index, endIndex - index);

            WriteLine();
        }

        protected int FindStartOfLine(ICapture capture)
        {
            return FindStartOfLine(capture.Index);
        }

        protected int FindStartOfLine(int index)
        {
            while (index > 0
                && Input[index - 1] != '\n')
            {
                index--;
            }

            return index;
        }

        protected int FindEndOfLine(ICapture capture)
        {
            int index = capture.Index + capture.Length;

            if (index > 0
                && capture.Length > 0
                && Input[index - 1] == '\n')
            {
                return index;
            }

            while (index < Input.Length)
            {
                if (Input[index] == '\n')
                    return ++index;

                index++;
            }

            return index;
        }

        protected void WriteContextBefore(int startIndex, int endIndex, int lineNumber)
        {
            if (startIndex == endIndex)
                return;

            int lastEolIndex = endIndex;
            int lineCount = 0;
            int i = endIndex - 2;

            while (true)
            {
                if (i > startIndex)
                {
                    if (Input[i] == '\n')
                    {
                        lineCount++;
                        lastEolIndex = i + 1;

                        if (lineCount == Options.ContextBefore)
                            break;
                    }

                    i--;
                }
                else if (i == startIndex)
                {
                    if (i == 0)
                    {
                        lineCount++;
                        lastEolIndex = 0;
                    }
                    else if (i > 0
                        && Input[i - 1] == '\n')
                    {
                        lineCount++;
                        lastEolIndex = i;
                    }

                    break;
                }
            }

            i = lastEolIndex;

            while (i < endIndex)
            {
                if (Input[i] == '\n')
                {
                    Write(Options.Indent);

                    if (Options.IncludeLineNumber)
                        WriteContextLineNumber(lineNumber - lineCount);

                    Write(Input, lastEolIndex, i - lastEolIndex + 1, Colors.ContextLine);

                    lastEolIndex = i + 1;
                    lineCount--;
                }

                i++;
            }
        }

        protected int WriteContextAfter(int startIndex, int endIndex, int lineNumber)
        {
            if (endIndex == startIndex)
                return startIndex;

            int lastEolIndex = startIndex;
            int lineCount = 0;

            for (int i = lastEolIndex; i < endIndex; i++)
            {
                if (Input[i] == '\n')
                {
                    Write(Options.Indent);

                    if (Options.IncludeLineNumber)
                        WriteContextLineNumber(lineNumber + lineCount + 1);

                    Write(Input, lastEolIndex, i - lastEolIndex + 1, Colors.ContextLine);

                    lastEolIndex = i + 1;
                    lineCount++;

                    if (lineCount == Options.ContextAfter)
                        break;
                }
            }

            return lastEolIndex;
        }

        private void WriteContextLineNumber(int value)
        {
            Write(value.ToString(), Colors.ContextLine);
            Write(" ");
        }
    }
}
