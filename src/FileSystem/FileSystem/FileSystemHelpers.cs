// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Orang.FileSystem
{
    internal static class FileSystemHelpers
    {
        private static ImmutableHashSet<char>? _invalidFileNameChars;

        private static readonly EnumerationOptions _enumerationOptionsNoRecurse = new EnumerationOptions()
        {
            AttributesToSkip = 0,
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
        };

        private static readonly EnumerationOptions _enumerationOptionsRecurse = new EnumerationOptions()
        {
            AttributesToSkip = 0,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
        };

        public static bool IsCaseSensitive { get; } = GetIsCaseSensitive();

        public static StringComparer Comparer { get; } = (IsCaseSensitive) ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;

        public static StringComparison Comparison { get; } = (IsCaseSensitive) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

        public static ImmutableHashSet<char> InvalidFileNameChars
        {
            get
            {
                if (_invalidFileNameChars == null)
                    Interlocked.CompareExchange(ref _invalidFileNameChars, Path.GetInvalidFileNameChars().ToImmutableHashSet(), null);

                return _invalidFileNameChars;
            }
        }

        public static IEnumerable<string> EnumerateFiles(string path, EnumerationOptions options)
        {
            return Directory.EnumerateFiles(path, "*", options);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, EnumerationOptions options)
        {
            return Directory.EnumerateDirectories(path, "*", options);
        }

        public static string[] GetFiles(string path, EnumerationOptions options)
        {
            return Directory.GetFiles(path, "*", options);
        }

        public static string[] GetDirectories(string path, EnumerationOptions options)
        {
            return Directory.GetDirectories(path, "*", options);
        }

        private static bool GetIsCaseSensitive()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return true;

            Debug.Fail(RuntimeInformation.OSDescription);

            return true;
        }

        internal static bool FileEquals(string path1, string path2, FileCompareOptions options)
        {
            if ((options & FileCompareOptions.ModifiedTime) != 0
                && File.GetLastWriteTimeUtc(path1) != File.GetLastWriteTimeUtc(path2))
            {
                return false;
            }

            if ((options & FileCompareOptions.Attributes) != 0
                && File.GetAttributes(path1) != File.GetAttributes(path2))
            {
                return false;
            }

            if ((options & (FileCompareOptions.Size | FileCompareOptions.Content)) != 0)
            {
                using (var fs1 = new FileStream(path1, FileMode.Open, FileAccess.Read))
                using (var fs2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
                {
                    if ((options & FileCompareOptions.Size) != 0
                        && fs1.Length != fs2.Length)
                    {
                        return false;
                    }

                    if ((options & FileCompareOptions.Content) != 0
                        && !StreamComparer.Default.ByteEquals(fs1, fs2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal static bool IsSubdirectory(string basePath, string path)
        {
            return path.Length > basePath.Length
                && (IsDirectorySeparator(basePath[^1])
                    || IsDirectorySeparator(path[basePath.Length]))
                && path.StartsWith(basePath, Comparison);
        }

        internal static bool UpdateAttributes(string sourcePath, string destinationPath)
        {
            FileAttributes sourceAttributes = File.GetAttributes(sourcePath);
            FileAttributes destinationAttributes = File.GetAttributes(destinationPath);

            if (sourceAttributes != destinationAttributes)
            {
                File.SetAttributes(destinationPath, sourceAttributes);
                return true;
            }

            return false;
        }

        internal static int IndexOfDirectorySeparator(string path, int start)
        {
            for (int i = start; i < path.Length; i++)
            {
                if (IsDirectorySeparator(path[i]))
                    return i;
            }

            return path.Length;
        }

        public static void Delete(
            FileMatch fileMatch,
            bool contentOnly = false,
            bool includingBom = false,
            bool filesOnly = false,
            bool directoriesOnly = false)
        {
            if (fileMatch.IsDirectory)
            {
                DeleteDirectory(fileMatch.Path, contentOnly: contentOnly, filesOnly: filesOnly, directoriesOnly: directoriesOnly);
            }
            else
            {
                DeleteFile(fileMatch.Path, contentOnly: contentOnly, includingBom: includingBom);
            }
        }

        public static void DeleteDirectory(string directoryPath, bool contentOnly, bool filesOnly, bool directoriesOnly)
        {
            if (contentOnly)
            {
                if (!directoriesOnly)
                {
                    foreach (string path in EnumerateFiles(directoryPath, _enumerationOptionsNoRecurse))
                        File.Delete(path);
                }

                if (!filesOnly)
                {
                    foreach (string path in EnumerateDirectories(directoryPath, _enumerationOptionsNoRecurse))
                        Directory.Delete(path, recursive: true);
                }
            }
            else
            {
                Directory.Delete(directoryPath, recursive: true);
            }
        }

        public static void DeleteFile(string filePath, bool contentOnly, bool includingBom)
        {
            if (contentOnly)
            {
                FileAccess fileAccess = (includingBom) ? FileAccess.Write : FileAccess.ReadWrite;

                using (var stream = new FileStream(filePath, FileMode.Open, fileAccess))
                {
                    int length = 0;

                    if (includingBom)
                    {
                        Encoding? encoding = DetectEncoding(stream);

                        if (encoding != null)
                            length = encoding.Preamble.Length;
                    }

                    stream.SetLength(length);
                }
            }
            else
            {
                File.Delete(filePath);
            }
        }

        public static bool IsEmptyFile(string path)
        {
            var fileInfo = new FileInfo(path);

            return IsEmptyFile(fileInfo);
        }

        public static bool IsEmptyFile(FileInfo fileInfo)
        {
            if (fileInfo.Length == 0)
                return true;

            if (fileInfo.Length <= 4)
            {
                using (FileStream stream = fileInfo.OpenRead())
                {
                    Encoding? encoding = DetectEncoding(stream);

                    return encoding?.Preamble.Length == stream.Length;
                }
            }

            return false;
        }

        public static bool IsEmptyDirectory(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static int GetFileNameIndex(string path)
        {
            int rootLength = Path.GetPathRoot(path).Length;

            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (i < rootLength
                    || IsDirectorySeparator(path[i]))
                {
                    return i + 1;
                }
            }

            return 0;
        }

        public static int GetExtensionIndex(string path)
        {
            int length = path.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                char ch = path[i];

                if (ch == '.')
                    return i;

                if (IsDirectorySeparator(ch))
                    break;
            }

            return path.Length;
        }

        public static bool IsDirectorySeparator(char ch)
        {
            return ch == Path.DirectorySeparatorChar
                || ch == Path.AltDirectorySeparatorChar;
        }

        public static long GetDirectorySize(string directoryPath)
        {
            long size = 0;

            foreach (string filePath in EnumerateFiles(directoryPath, _enumerationOptionsRecurse))
            {
                size += new FileInfo(filePath).Length;
            }

            return size;
        }

        public static bool ContainsInvalidFileNameChars(string path, int startIndex)
        {
            for (int i = startIndex; i < path.Length; i++)
            {
                if (InvalidFileNameChars.Contains(path[i]))
                    return true;
            }

            return false;
        }

        public static string CreateNewFilePath(string path)
        {
            int count = 2;
            int extensionIndex = GetExtensionIndex(path);

            if (extensionIndex > 0
                && IsDirectorySeparator(path[extensionIndex - 1]))
            {
                extensionIndex = path.Length;
            }

            string newPath;

            do
            {
                newPath = path.Insert(extensionIndex, count.ToString());

                count++;

            } while (File.Exists(newPath));

            return newPath;
        }

        public static Encoding? DetectEncoding(Stream stream)
        {
            long length = stream.Length;

            if (length < 2)
                return null;

            var buffer = new byte[2];

            stream.Read(buffer, 0, 2);

            if (buffer[0] == 0xEF)
            {
                if (buffer[1] == 0xBB
                    && length >= 3)
                {
                    stream.Read(buffer, 0, 1);

                    if (buffer[0] == 0xBF)
                    {
                        // UTF-8  EF BB BF
                        return Encoding.UTF8;
                    }
                }
            }
            else if (buffer[0] == 0xFE)
            {
                // UTF-16 (BE)  FE FF
                if (buffer[1] == 0xFF)
                    return Encoding.BigEndianUnicode;
            }
            else if (buffer[0] == 0xFF)
            {
                if (buffer[1] == 0xFE)
                {
                    if (length >= 4)
                    {
                        stream.Read(buffer, 0, 2);

                        if (buffer[0] == 0
                            && buffer[1] == 0)
                        {
                            // UTF-32 (LE)  FF FE 00 00
                            return Encoding.UTF32;
                        }

                        stream.Position = 2;
                    }

                    // UTF-16 (LE)  FF FE
                    return Encoding.Unicode;
                }
            }
            else if (buffer[0] == 0)
            {
                if (buffer[1] == 0
                    && length >= 4)
                {
                    stream.Read(buffer, 0, 2);

                    if (buffer[0] == 0xFE
                        && buffer[1] == 0xFF)
                    {
                        // UTF-32 (BE)  00 00 FE FF
                        return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
                    }
                }
            }

            stream.Position = 0;
            return null;
        }

        public static bool TryReadAllText(string path, [NotNullWhen(true)] out string? content, Action<Exception>? exceptionHandler = null)
        {
            try
            {
                content = File.ReadAllText(path);
                return true;
            }
            catch (Exception ex) when (ex is ArgumentException
                || ex is IOException
                || ex is UnauthorizedAccessException)
            {
                exceptionHandler?.Invoke(ex);
                content = null;
                return false;
            }
        }

        public static string EnsureFullPath(string path)
        {
            return (Path.IsPathRooted(path)) ? path : Path.GetFullPath(path);
        }
    }
}
