// <copyright file="OrderBy.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Append FileOrder to an IEnumerable of <see cref="FileInfo"/>
    /// </summary>
    internal static class OrderBy
    {
        /// <summary>
        /// Append OrderBy to an IEnumerable of <see cref="FileInfo"/>
        /// </summary>
        /// <param name="fileInfos">An IEnumerable of FileInfo</param>
        /// <param name="fileOrder">FileOrder to append</param>
        /// <returns>Ordered IEnumerable</returns>
        public static IEnumerable<FileInfo> AppendOrderBy(IEnumerable<FileInfo> fileInfos, FileOrder fileOrder)
        {
            return fileOrder switch
            {
                FileOrder.FullFileName => fileInfos.OrderBy(f => f.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameDesc => fileInfos.OrderByDescending(f => f.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariant => fileInfos.OrderBy(f => f.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariantDesc => fileInfos.OrderByDescending(f => f.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.Size => fileInfos.OrderBy(f => f.Length),
                FileOrder.SizeDesc => fileInfos.OrderByDescending(f => f.Length),
                FileOrder.Created => fileInfos.OrderBy(f => f.CreationTimeUtc),
                FileOrder.CreatedDesc => fileInfos.OrderByDescending(f => f.CreationTimeUtc),
                FileOrder.Modified => fileInfos.OrderBy(f => f.LastWriteTimeUtc),
                FileOrder.ModifiedDesc => fileInfos.OrderByDescending(f => f.LastWriteTimeUtc),
                FileOrder.Accessed => fileInfos.OrderBy(f => f.LastAccessTimeUtc),
                FileOrder.AccessedDesc => fileInfos.OrderByDescending(f => f.LastAccessTimeUtc),
                _ => fileInfos,
            };
        }

        /// <summary>
        /// Append ThenBy to an IOrderedEnumerable of <see cref="FileInfo"/>
        /// </summary>
        /// <param name="fileInfos">An IOrderedEnumerable of FileInfo</param>
        /// <param name="fileOrder">FileOrder to append</param>
        /// <returns>Ordered IEnumerable</returns>
        public static IEnumerable<FileInfo> AppendThenBy(IOrderedEnumerable<FileInfo> fileInfos, FileOrder fileOrder)
        {
            return fileOrder switch
            {
                FileOrder.FullFileName => fileInfos.ThenBy(f => f.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameDesc => fileInfos.ThenByDescending(f => f.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariant => fileInfos.ThenBy(f => f.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariantDesc => fileInfos.ThenByDescending(f => f.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.Size => fileInfos.ThenBy(f => f.Length),
                FileOrder.SizeDesc => fileInfos.ThenByDescending(f => f.Length),
                FileOrder.Created => fileInfos.ThenBy(f => f.CreationTimeUtc),
                FileOrder.CreatedDesc => fileInfos.ThenByDescending(f => f.CreationTimeUtc),
                FileOrder.Modified => fileInfos.ThenBy(f => f.LastWriteTimeUtc),
                FileOrder.ModifiedDesc => fileInfos.ThenByDescending(f => f.LastWriteTimeUtc),
                FileOrder.Accessed => fileInfos.ThenBy(f => f.LastAccessTimeUtc),
                FileOrder.AccessedDesc => fileInfos.ThenByDescending(f => f.LastAccessTimeUtc),
                _ => fileInfos,
            };
        }
    }
}