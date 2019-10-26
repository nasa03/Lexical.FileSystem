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
    /// <summary>
    /// Extension methods for <see cref="IFileSystem"/>.
    /// </summary>
    public static partial class IFileSystemExtensions
    {
        /// <summary>
        /// Test if <paramref name="filesystemOption"/> has CreateDirectory capability.
        /// <param name="filesystemOption"></param>
        /// </summary>
        /// <returns>true, if has CreateDirectory capability</returns>
        public static bool CanCreateDirectory(this IFileSystemOption filesystemOption)
            => filesystemOption.AsOption<IFileSystemOptionCreateDirectory>() is IFileSystemOptionCreateDirectory directoryConstructor ? directoryConstructor.CanCreateDirectory : false;

        /// <summary>
        /// Create a directory, or multiple cascading directories.
        /// 
        /// If directory at <paramref name="path"/> already exists, then returns without exception.
        /// 
        /// <paramref name="path"/> should end with directory separator character '/'.
        /// </summary>
        /// <param name="filesystem"></param>
        /// <param name="path">Relative path to file. Directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <returns>true if directory exists after the method, false if directory doesn't exist</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support create directory</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public static void CreateDirectory(this IFileSystem filesystem, string path)
        {
            if (filesystem is IFileSystemCreateDirectory directoryConstructor) directoryConstructor.CreateDirectory(path);
            else throw new NotSupportedException(nameof(CreateDirectory));
        }
    }

    /// <summary><see cref="IFileSystemOptionCreateDirectory"/> operations.</summary>
    public class FileSystemOptionOperationCreateDirectory : IFileSystemOptionOperationFlatten, IFileSystemOptionOperationIntersection, IFileSystemOptionOperationUnion
    {
        /// <summary>The option type that this class has operations for.</summary>
        public Type OptionType => typeof(IFileSystemOptionCreateDirectory);
        /// <summary>Flatten to simpler instance.</summary>
        public IFileSystemOption Flatten(IFileSystemOption o) => o is IFileSystemOptionCreateDirectory c ? o is FileSystemOptionCreateDirectory ? /*already flattened*/o : /*new instance*/new FileSystemOptionCreateDirectory(c.CanCreateDirectory) : throw new InvalidCastException($"{typeof(IFileSystemOptionCreateDirectory)} expected.");
        /// <summary>Intersection of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Intersection(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionCreateDirectory c1 && o2 is IFileSystemOptionCreateDirectory c2 ? new FileSystemOptionCreateDirectory(c1.CanCreateDirectory && c2.CanCreateDirectory) : throw new InvalidCastException($"{typeof(IFileSystemOptionCreateDirectory)} expected.");
        /// <summary>Union of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Union(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionCreateDirectory c1 && o2 is IFileSystemOptionCreateDirectory c2 ? new FileSystemOptionCreateDirectory(c1.CanCreateDirectory || c2.CanCreateDirectory) : throw new InvalidCastException($"{typeof(IFileSystemOptionCreateDirectory)} expected.");
    }

    /// <summary>File system option for creating directories.</summary>
    public class FileSystemOptionCreateDirectory : IFileSystemOptionCreateDirectory
    {
        /// <summary>Has CreateDirectory capability.</summary>
        public bool CanCreateDirectory { get; protected set; }

        /// <summary>Create file system option for creating directories.</summary>
        public FileSystemOptionCreateDirectory(bool canCreateDirectory)
        {
            CanCreateDirectory = canCreateDirectory;
        }

        /// <inheritdoc/>
        public override string ToString() => CanCreateDirectory ? "CanCreateDirectory" : "NoCreateDirectory";
    }
}
