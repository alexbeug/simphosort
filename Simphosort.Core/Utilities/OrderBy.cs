// <copyright file="OrderBy.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;
using Simphosort.Core.Values;

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Append FileOrder to an IEnumerable of <see cref="FileInfo"/>
    /// </summary>
    internal static class OrderBy
    {
        /// <summary>
        /// Append OrderBy to an IEnumerable of <see cref="IPhotoFileInfo"/>
        /// </summary>
        /// <typeparam name="T">A <see cref="IPhotoFileInfo"/> type</typeparam>
        /// <param name="photoFileInfos">An IEnumerable of <see cref="IPhotoFileInfo"/></param>
        /// <param name="fileOrder">FileOrder to append</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>Ordered IEnumerable</returns>
        public static IEnumerable<T> AppendOrderBy<T>(IEnumerable<T> photoFileInfos, FileOrder fileOrder, CancellationToken cancellationToken)
            where T : IPhotoFileInfo
        {
            return fileOrder switch
            {
                FileOrder.FullFileName => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariant => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariantDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.Size => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.Length),
                FileOrder.SizeDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.Length),
                FileOrder.Created => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.CreationTimeUtc),
                FileOrder.CreatedDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.CreationTimeUtc),
                FileOrder.Modified => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.LastWriteTimeUtc),
                FileOrder.ModifiedDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.LastWriteTimeUtc),
                FileOrder.Accessed => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(f => f.FileInfo.LastAccessTimeUtc),
                FileOrder.AccessedDesc => photoFileInfos.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderByDescending(f => f.FileInfo.LastAccessTimeUtc),
                _ => photoFileInfos,
            };
        }

        /// <summary>
        /// Append ThenBy to an IOrderedEnumerable of <see cref="IPhotoFileInfo"/>
        /// </summary>
        /// <typeparam name="T">A <see cref="IPhotoFileInfo"/> type</typeparam>
        /// <param name="fileInfos">An IOrderedEnumerable of <see cref="IPhotoFileInfo"/></param>
        /// <param name="fileOrder">FileOrder to append</param>
        /// <returns>Ordered IEnumerable</returns>
        public static IEnumerable<T> AppendThenBy<T>(IOrderedEnumerable<T> fileInfos, FileOrder fileOrder)
            where T : IPhotoFileInfo
        {
            return fileOrder switch
            {
                FileOrder.FullFileName => fileInfos.ThenBy(f => f.FileInfo.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameDesc => fileInfos.ThenByDescending(f => f.FileInfo.FullName, StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariant => fileInfos.ThenBy(f => f.FileInfo.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.FullFileNameLowerInvariantDesc => fileInfos.ThenByDescending(f => f.FileInfo.FullName.ToLowerInvariant(), StringComparer.InvariantCulture),
                FileOrder.Size => fileInfos.ThenBy(f => f.FileInfo.Length),
                FileOrder.SizeDesc => fileInfos.ThenByDescending(f => f.FileInfo.Length),
                FileOrder.Created => fileInfos.ThenBy(f => f.FileInfo.CreationTimeUtc),
                FileOrder.CreatedDesc => fileInfos.ThenByDescending(f => f.FileInfo.CreationTimeUtc),
                FileOrder.Modified => fileInfos.ThenBy(f => f.FileInfo.LastWriteTimeUtc),
                FileOrder.ModifiedDesc => fileInfos.ThenByDescending(f => f.FileInfo.LastWriteTimeUtc),
                FileOrder.Accessed => fileInfos.ThenBy(f => f.FileInfo.LastAccessTimeUtc),
                FileOrder.AccessedDesc => fileInfos.ThenByDescending(f => f.FileInfo.LastAccessTimeUtc),
                _ => fileInfos,
            };
        }
    }
}