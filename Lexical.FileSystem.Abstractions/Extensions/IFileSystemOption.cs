﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Lexical.FileSystem
{
    /// <summary>
    /// Extension methods for <see cref="IFileSystem"/>.
    /// </summary>
    public static partial class IFileSystemExtensions
    {

        /// <summary>
        /// Get option as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns>Option casted as <typeparamref name="T"/> or null</returns>
        public static T As<T>(this IFileSystemOption option) where T : IFileSystemOption
        {
            if (option is T casted) return casted;
            if (option is IFileSystemOptionAdaptable adaptable && adaptable.GetOption(typeof(T)) is T casted_) return casted_;
            return default;
        }
    }

}
