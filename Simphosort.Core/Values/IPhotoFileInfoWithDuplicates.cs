// <copyright file="IPhotoFileInfoWithDuplicates.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Values
{
    /// <summary>
    /// File information with duplicates
    /// </summary>
    public interface IPhotoFileInfoWithDuplicates : IPhotoFileInfo
    {
        /// <summary>
        /// Gets duplicate files
        /// </summary>
        IList<FileInfo> Duplicates { get; }
    }
}