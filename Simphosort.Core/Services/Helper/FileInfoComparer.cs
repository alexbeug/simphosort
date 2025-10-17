// <copyright file="FileInfoComparer.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class FileInfoComparer : IFileInfoComparer
    {
        /// <inheritdoc/>
        public bool Equals(FileInfo? x, FileInfo? y)
        {
            // Check for null and compare file length
            if (x != null && y != null && x.Length.Equals(y.Length))
            {
                // Check if both files exist in each other's directory. This is done to avoid case sensitivity issues and
                // letting File.Exists handle the case sensitivity based on the underlying file system or operating system.
                return File.Exists(Path.Combine(y.DirectoryName ?? string.Empty, x.Name))
                    && File.Exists(Path.Combine(x.DirectoryName ?? string.Empty, y.Name));
            }
            else
            {
                // Simple checks failed, return false
                return false;
            }
        }

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] FileInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}