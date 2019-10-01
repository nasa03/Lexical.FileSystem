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
    // <doc>
    /// <summary>File system option for deleting files and directories.</summary>
    [Operations(typeof(FileSystemOptionOperationDelete))]
    public interface IFileSystemOptionDelete : IFileSystemOption
    {
        /// <summary>Has Delete capability.</summary>
        bool CanDelete { get; }
    }

    /// <summary>
    /// File system that can delete files and directories.
    /// </summary>
    public interface IFileSystemDelete : IFileSystem, IFileSystemOptionDelete
    {
        /// <summary>
        /// Delete a file or directory.
        /// 
        /// If <paramref name="recursive"/> is false and <paramref name="path"/> is a directory that is not empty, then <see cref="IOException"/> is thrown.
        /// If <paramref name="recursive"/> is true, then any file or directory in <paramref name="path"/> is deleted as well.
        /// </summary>
        /// <param name="path">path to a file or directory</param>
        /// <param name="recursive">if path refers to directory, recurse into sub directories</param>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error, or if <paramref name="path"/> refered to a directory that wasn't empty and <paramref name="recursive"/> is false, or trying to delete root when not allowed</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> contains invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support deleting files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="path"/> refers to non-file device</exception>
        /// <exception cref="ObjectDisposedException"/>
        void Delete(string path, bool recursive = false);
    }
    // </doc>

    /// <summary>
    /// Extension methods for <see cref="IFileSystem"/>.
    /// </summary>
    public static partial class IFileSystemExtensions
    {
        /// <summary>
        /// Test if <paramref name="filesystemOption"/> has Delete capability.
        /// <param name="filesystemOption"></param>
        /// </summary>
        /// <returns>true, if has Delete capability</returns>
        public static bool CanDelete(this IFileSystemOption filesystemOption)
            => filesystemOption.As<IFileSystemDelete>() is IFileSystemDelete deleter ? deleter.CanDelete : false;

        /// <summary>
        /// Delete a file or directory.
        /// 
        /// If <paramref name="recursive"/> is false and <paramref name="path"/> is a directory that is not empty, then <see cref="IOException"/> is thrown.
        /// If <paramref name="recursive"/> is true, then any file or directory in <paramref name="path"/> is deleted as well.
        /// </summary>
        /// <param name="filesystem"></param>
        /// <param name="path">path to a file or directory</param>
        /// <param name="recursive">if path refers to directory, recurse into sub directories</param>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error, or if <paramref name="path"/> refered to a directory that wasn't empty and <paramref name="recursive"/> is false, or trying to delete root when not allowed</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> contains invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support deleting files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="path"/> refers to non-file device</exception>
        /// <exception cref="ObjectDisposedException"/>
        public static void Delete(this IFileSystem filesystem, string path, bool recursive = false)
        {
            if (filesystem is IFileSystemDelete deleter) deleter.Delete(path, recursive);
            else throw new NotSupportedException(nameof(Delete));
        }
    }

    /// <summary><see cref="IFileSystemOptionDelete"/> operations.</summary>
    public class FileSystemOptionOperationDelete : IFileSystemOptionOperationFlatten, IFileSystemOptionOperationIntersection, IFileSystemOptionOperationUnion
    {
        /// <summary>The option type that this class has operations for.</summary>
        public Type OptionType => typeof(IFileSystemOptionDelete);
        /// <summary>Flatten to simpler instance.</summary>
        public IFileSystemOption Flatten(IFileSystemOption o) => o is IFileSystemOptionDelete c ? o is FileSystemOptionDelete ? /*already flattened*/o : /*new instance*/new FileSystemOptionDelete(c.CanDelete) : throw new InvalidCastException($"{typeof(IFileSystemOptionDelete)} expected.");
        /// <summary>Intersection of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Intersection(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionDelete c1 && o2 is IFileSystemOptionDelete c2 ? new FileSystemOptionDelete(c1.CanDelete && c2.CanDelete) : throw new InvalidCastException($"{typeof(IFileSystemOptionDelete)} expected.");
        /// <summary>Union of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Union(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionDelete c1 && o2 is IFileSystemOptionDelete c2 ? new FileSystemOptionDelete(c1.CanDelete || c2.CanDelete) : throw new InvalidCastException($"{typeof(IFileSystemOptionDelete)} expected.");
    }

}
