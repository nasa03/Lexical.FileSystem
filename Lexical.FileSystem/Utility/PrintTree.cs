﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           23.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.FileSystem.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.FileSystem
{
    /// <summary>
    /// <see cref="IFileSystem"/> extension methods.
    /// </summary>
    public static class FileSystemPrintTree
    {
        /// <summary>
        /// Print tree structure of the whole filesystem. 
        /// 
        /// Starts at <paramref name="startPath"/> if provided, otherwise starts at root "".
        /// <paramref name="maxLevel"/> sets maximum visit depths.
        /// 
        /// ""
        /// ├──""
        /// │  ├──"mnt"
        /// │  ├──"tmp"
        /// │  │  └──"helloworld.txt"
        /// │  └──"usr"
        /// │     └──"lex"
        /// └──"c:"
        ///    └──"dir"
        ///       └──"dir"
        /// 
        /// 
        /// Any thrown exceptions are printed into the line that produced the error.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="output">output such as <see cref="Console.Out"/></param>
        /// <param name="startPath"></param>
        /// <param name="maxLevel"></param>
        public static void PrintTree(this IFileSystem fileSystem, TextWriter output, string startPath = "", int maxLevel = Int32.MaxValue)
        {
            foreach(Line line in fileSystem.VisitTree(startPath, maxLevel))
            {
                for (int l = 0; l < line.Level - 1; l++)
                {
                    output.Write(line.LevelContinues(l) ? "│  " : "   ");
                }
                if (line.Level >= 1)
                {
                    bool levelContinues = line.LevelContinues(line.Level - 1);
                    output.Write(levelContinues ? "├──" : "└──");
                }
                output.Write("\"");
                output.Write(line.Entry.Name);
                output.Write("\"");
                if (line.Error != null)
                {
                    output.Write(" ");
                    output.Write(line.Error.GetType().Name);
                    output.Write(": ");
                    output.Write(line.Error.Message);
                }
                output.Write("\r\n");
            }
        }

        /// <summary>
        /// Vists tree structure of filesystem. 
        /// 
        /// Starts at <paramref name="startPath"/> if provided, otherwise starts at root "".
        /// <paramref name="maxLevel"/> sets maximum visit depths.
        /// 
        /// 
        /// ""
        /// ├──""
        /// │  ├──"mnt"
        /// │  ├──"tmp"
        /// │  │  └──"helloworld.txt"
        /// │  └──"usr"
        /// │     └──"lex"
        /// └──"c:"
        ///    └──"dir"
        ///       └──"dir"
        /// 
        /// 
        /// Any thrown exceptions are printed into the line that produced the error.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="startPath"></param>
        /// <param name="maxLevel"></param>
        public static IEnumerable<Line> VisitTree(this IFileSystem fileSystem, string startPath = "", int maxLevel = Int32.MaxValue)
        {
            LinkedList<Line> queue = new LinkedList<Line>();
            queue.AddLast( new Line(fileSystem.GetEntry(startPath), 0, 0UL) );
            while (queue.Count > 0)
            {
                // Next entry
                Line line = queue.Last.Value;
                queue.RemoveLast();

                // Browse
                if (line.Entry.IsDirectory() && line.Level < maxLevel)
                {
                    try
                    {
                        IFileSystemEntry[] entries = fileSystem.Browse(line.Entry.Path);
                        // Sort
                        Array.Sort(entries, FileSystemEntryComparer.Instance);
                        // Queue
                        for (int i = entries.Length - 1; i >= 0; i--)
                        {
                            int newLevel = line.Level + 1;
                            ulong newLevelContinuesBitMask = line.LevelContinuesBitMask;
                            if (i < entries.Length - 1) newLevelContinuesBitMask |= 1UL << (line.Level - 1);
                            queue.AddLast(new Line(entries[i], newLevel, newLevelContinuesBitMask));
                        }
                    }
                    catch (Exception e)
                    {
                        line.Error = e;
                    }
                }

                // yield line
                yield return line;
            }
        }

        /// <summary>
        /// Tree visitor line.
        /// </summary>
        public struct Line
        {
            /// <summary>
            /// Visited tree entry.
            /// </summary>
            public readonly IFileSystemEntry Entry;

            /// <summary>
            /// Visit depth, 0 is start path.
            /// </summary>
            public readonly int Level;

            /// <summary>
            /// Bitmask for each level on whether the level has more entries to come in the enumerator.
            /// </summary>
            public readonly ulong LevelContinuesBitMask;

            /// <summary>
            /// Browse error is placed here.
            /// </summary>
            public Exception Error;

            /// <summary>
            /// Create line
            /// </summary>
            /// <param name="entry"></param>
            /// <param name="level"></param>
            /// <param name="levelContinuesBitMask"></param>
            /// <param name="error">(optional) initial error</param>
            public Line(IFileSystemEntry entry, int level, ulong levelContinuesBitMask, Exception error = null)
            {
                Entry = entry;
                Level = level;
                LevelContinuesBitMask = levelContinuesBitMask;
                Error = error;
            }

            /// <summary>
            /// Tests whether there will be more entries to specific <paramref name="level"/>.
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            public bool LevelContinues(int level) => level >= 64 ? false : (LevelContinuesBitMask & 1UL << (level - 1)) != 0UL;
        }

    }
}
