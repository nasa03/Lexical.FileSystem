﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using System;
using System.IO;
using System.Security;

namespace Lexical.FileSystem
{
    /// <summary>File system option for move/rename.</summary>
    [Operations(typeof(FileSystemOptionOperationMove))]
    // <IFileSystemOptionMove>
    public interface IFileSystemOptionMove : IFileSystemOption
    {
        /// <summary>Can Move files within same volume.</summary>
        bool CanMoveLocal { get; }
    }
    // </IFileSystemOptionMove>

    // <doc>
    /// <summary>
    /// File system that can move/rename files and directories.
    /// </summary>
    public interface IFileSystemMove : IFileSystem, IFileSystemOptionMove
    {
        /// <summary>
        /// Tests if can move/rename file from <paramref name="oldPath"/> to <paramref name="newPath"/>.
        /// Returns false if the two locations are on different filesystem volumes, as may occur in VirtualFileSystem.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        new bool CanMoveLocal(string oldPath, string newPath);

        /// <summary>
        /// Move/rename a file or directory within same filesystem volume. 
        /// 
        /// If <paramref name="oldPath"/> and <paramref name="newPath"/> refers to a directory, then the path names 
        /// should end with directory separator character '/'.
        /// 
        /// If <paramref name="oldPath"/> is on different volume than <paramref name="newPath"/>, then <see cref="FileSystemExceptionDifferentVolumes"/> 
        /// is thrown. The consumer of the interface can use the Move() extension method. 
        /// </summary>
        /// <param name="oldPath">old path of a file or directory</param>
        /// <param name="newPath">new path of a file or directory</param>
        /// <exception cref="FileNotFoundException">The specified <paramref name="oldPath"/> is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException">path is null</exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support renaming/moving files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">path refers to non-file device, or an entry already exists at <paramref name="newPath"/></exception>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="FileSystemExceptionDifferentVolumes">If <paramref name="oldPath"/> is on different volume than <paramref name="newPath"/></exception>
        void MoveLocal(string oldPath, string newPath);
    }
    // </doc>
}
