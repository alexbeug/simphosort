// <copyright file="PhotoFileInfoComparerFactory.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using Simphosort.Core.Values;

namespace Simphosort.Core.Services.Comparer
{
    /// <inheritdoc cref="IPhotoFileInfoComparerFactory"/>
    internal class PhotoFileInfoComparerFactory : IPhotoFileInfoComparerFactory
    {
        #region Methods

        /// <inheritdoc/>
        public IPhotoFileInfoComparer Create(PhotoFileInfoComparerConfig config)
        {
            return new PhotoFileInfoComparer(config);
        }

        #endregion // Methods

        #region Nested Types

        /// <inheritdoc/>
        private sealed class PhotoFileInfoComparer : IPhotoFileInfoComparer
        {
            private readonly PhotoFileInfoComparerConfig _config;

            /// <summary>
            /// Initializes a new instance of the <see cref="PhotoFileInfoComparer"/> class.
            /// </summary>
            /// <param name="config">A <see cref="PhotoFileInfoComparerConfig"/></param>
            /// <remarks>Internal constructor in a private sealed class to make this object only created by it's factory</remarks>
            internal PhotoFileInfoComparer(PhotoFileInfoComparerConfig config)
            {
                _config = config;
            }

            /// <inheritdoc/>
            public bool Equals(IPhotoFileInfo? x, IPhotoFileInfo? y)
            {
                // Check for null and compare file length
                if (x == null || y == null)
                {
                    return false;
                }

                // Do not compare files in the same folder
                if (!string.IsNullOrWhiteSpace(x.FileInfo.DirectoryName)
                    && !string.IsNullOrWhiteSpace(y.FileInfo.DirectoryName)
                    && x.FileInfo.DirectoryName.Equals(y.FileInfo.DirectoryName))
                {
                    throw new ArgumentException($"Comparing files in the same folder is not supported! Folder: {x.FileInfo.DirectoryName}");
                }

                // Compare file sizes if configured to do so
                if (_config.CompareFileSize && !x.FileInfo.Length.Equals(y.FileInfo.Length))
                {
                    return false;
                }

                if (_config.CompareFileNameCaseInSensitive)
                {
                    // Compare file names in a case insensitive way. This can be forced on case sensitive file systems to
                    // avoid unwanted dulicates because of casing differences in files that are identical from size and content.
                    return x.FileInfo.Name.Equals(y.FileInfo.Name, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    // Check if both files exist in each other's directory. This is done to avoid case sensitivity issues and
                    // letting File.Exists handle the case sensitivity based on the underlying file system or operating system.
                    return File.Exists(Path.Combine(y.FileInfo.DirectoryName ?? string.Empty, x.FileInfo.Name))
                        && File.Exists(Path.Combine(x.FileInfo.DirectoryName ?? string.Empty, y.FileInfo.Name));
                }
            }

            /// <inheritdoc/>
            public int GetHashCode([DisallowNull] IPhotoFileInfo obj)
            {
                // Throw exception because GetHashCode is not supported for FileInfoComparer (supports only Equals method). A hash code
                // based comparison is not feasible for FileInfo objects as it would break the case sensitivity handling with File.Exists.
                throw new NotSupportedException("GetHashCode is not supported for PhotoFileInfoComparer!");
            }
        }

        #endregion // Nested Types
    }
}