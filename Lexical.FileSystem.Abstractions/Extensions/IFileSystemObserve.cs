﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Lexical.FileSystem
{
    /// <summary>
    /// Extension methods for <see cref="IFileSystem"/>.
    /// </summary>
    public static partial class IFileSystemExtensions
    {
        /// <summary>
        /// Test if <paramref name="filesystemOption"/> has Observe capability.
        /// <param name="filesystemOption"></param>
        /// </summary>
        /// <returns>true, if has Observe capability</returns>
        public static bool CanObserve(this IFileSystemOption filesystemOption)
            => filesystemOption.AsOption<IFileSystemOptionObserve>() is IFileSystemOptionObserve observer ? observer.CanObserve : false;

        /// <summary>
        /// Attach an <paramref name="observer"/> on to a directory. 
        /// 
        /// The <paramref name="filter"/> determines the file pattern to observe.
        ///  "*" Matches to any sequence characters within one folder.
        ///  "**" Matches to any sequence characters including directory levels '/'.
        ///  "?" Matches to one and exactly one character.
        /// 
        /// Examples:
        ///   <list type="bullet">
        ///     <item>"**" is any file in any directory.</item>
        ///     <item>"**/file.txt", to monitor "file.txt" in any subdirectory (excluding root due to missing "/")</item>
        ///     <item>"*" is any set of characters file in one directory. For example "mydir/somefile*.txt"</item>
        ///     <item>"", to monitor changes to the root directory itself, but not its files</item>
        ///     <item>"dir", to monitor the dir itself, but not its files</item>
        ///     <item>"dir/", to monitor the dir itself, but not its files</item>
        ///     <item>"dir/file", to monitor one file "file"</item>
        ///     <item>"dir/*", to monitor files in a dir but not subdirectories</item>
        ///     <item>"dir/**", to monitor files in a dir and its subdirectories</item>
        ///   </list>
        /// 
        /// The implementation sends the reference to the observer handle in a <see cref="IFileSystemEventStart"/> event before this method returns to caller.
        /// </summary>
        /// <param name="filesystem"></param>
        /// <param name="filter">glob pattern to filter events. "**" means any directory. For example "mydir/**/somefile.txt", or "**" for <paramref name="filter"/> and sub-directories</param>
        /// <param name="observer"></param>
        /// <param name="state">(optional) </param>
        /// <param name="eventDispatcher">(optional) event dispatcher</param>
        /// <param name="token">(optional) filesystem implementation specific token, such as session, security token or credential. Used for authorizing or facilitating the action.</param>
        /// <returns>handle to the observer, dispose to cancel the observe</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filter"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="filter"/> contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support observe</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="filter"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public static IFileSystemObserver Observe(this IFileSystem filesystem, string filter, IObserver<IFileSystemEvent> observer, object state = null, IFileSystemEventDispatcher eventDispatcher = default, IFileSystemToken token = null)
        {
            if (filesystem is IFileSystemObserve _observer) return _observer.Observe(filter, observer, state, eventDispatcher, token);
            else throw new NotSupportedException(nameof(Observe));
        }
    }

    /// <summary><see cref="IFileSystemOptionObserve"/> operations.</summary>
    public class FileSystemOptionOperationObserve : IFileSystemOptionOperationFlatten, IFileSystemOptionOperationIntersection, IFileSystemOptionOperationUnion
    {
        /// <summary>The option type that this class has operations for.</summary>
        public Type OptionType => typeof(IFileSystemOptionObserve);
        /// <summary>Flatten to simpler instance.</summary>
        public IFileSystemOption Flatten(IFileSystemOption o) => o is IFileSystemOptionObserve c ? o is FileSystemOptionObserve ? /*already flattened*/o : /*new instance*/new FileSystemOptionObserve(c.CanObserve) : throw new InvalidCastException($"{typeof(IFileSystemOptionObserve)} expected.");
        /// <summary>Intersection of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Intersection(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionObserve c1 && o2 is IFileSystemOptionObserve c2 ? new FileSystemOptionObserve(c1.CanObserve && c2.CanObserve) : throw new InvalidCastException($"{typeof(IFileSystemOptionObserve)} expected.");
        /// <summary>Union of <paramref name="o1"/> and <paramref name="o2"/>.</summary>
        public IFileSystemOption Union(IFileSystemOption o1, IFileSystemOption o2) => o1 is IFileSystemOptionObserve c1 && o2 is IFileSystemOptionObserve c2 ? new FileSystemOptionObserve(c1.CanObserve || c2.CanObserve) : throw new InvalidCastException($"{typeof(IFileSystemOptionObserve)} expected.");
    }

    /// <summary>File system option for observe.</summary>
    public class FileSystemOptionObserve : IFileSystemOptionObserve
    {
        /// <summary>Has Observe capability.</summary>
        public bool CanObserve { get; protected set; }

        /// <summary>Create file system option for observe.</summary>
        public FileSystemOptionObserve(bool canObserve)
        {
            CanObserve = canObserve;
        }

        /// <inheritdoc/>
        public override string ToString() => (CanObserve ? "CanObserve" : "NoObserve");
    }

}
