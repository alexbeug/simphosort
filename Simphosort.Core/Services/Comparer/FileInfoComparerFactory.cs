// <copyright file="FileInfoComparerFactory.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Simphosort.Core.Services.Comparer
{
    /// <inheritdoc cref="IFileInfoComparerFactory"/>
    internal class FileInfoComparerFactory : IFileInfoComparerFactory
    {
        #region Methods

        /// <inheritdoc/>
        public IFileInfoComparer Create(FileInfoComparerConfig config)
        {
            return new FileInfoComparer(config);
        }

        #endregion // Methods

        #region Nested Types

        /// <inheritdoc/>
        private sealed class FileInfoComparer : IFileInfoComparer
        {
            private readonly FileInfoComparerConfig _config;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileInfoComparer"/> class.
            /// </summary>
            /// <param name="config">A <see cref="FileInfoComparerConfig"/></param>
            /// <remarks>Internal constructor in a private sealed class to make this object only created by it's factory</remarks>
            internal FileInfoComparer(FileInfoComparerConfig config)
            {
                _config = config;
            }

            /// <inheritdoc/>
            public bool Equals(FileInfo? x, FileInfo? y)
            {
                // Check for null and compare file length
                if (x == null || y == null)
                {
                    return false;
                }

                // Compare file sizes if configured to do so
                if (_config.CompareFileSize && !x.Length.Equals(y.Length))
                {
                    return false;
                }

                if (_config.CompareFileNameCaseInSensitive)
                {
                    // Compare file names in a case insensitive way. This can be forced on case sensitive file systems to
                    // avoid unwanted dulicates because of casing differences in files that are identical from size and content.
                    return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    // Check if both files exist in each other's directory. This is done to avoid case sensitivity issues and
                    // letting File.Exists handle the case sensitivity based on the underlying file system or operating system.
                    return File.Exists(Path.Combine(y.DirectoryName ?? string.Empty, x.Name))
                        && File.Exists(Path.Combine(x.DirectoryName ?? string.Empty, y.Name));
                }
            }

            /// <inheritdoc/>
            public int GetHashCode([DisallowNull] FileInfo obj)
            {
                int hashCode = 17;

                if (_config.CompareFileSize)
                {
                    hashCode = (hashCode * 31) + obj.Length.GetHashCode();
                }

                if (_config.CompareFileNameCaseInSensitive)
                {
                    hashCode = (hashCode * 31) + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
                }
                else
                {
                    hashCode = (hashCode * 31) + obj.Name.GetHashCode();
                }

                return hashCode;
            }
        }

        #endregion // Nested Types
    }
}