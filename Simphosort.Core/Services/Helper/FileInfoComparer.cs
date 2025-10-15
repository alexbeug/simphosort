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
            return x != null
                && y != null
                && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) // TODO: use casing
                && x.Length.Equals(y.Length);
        }

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] FileInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}