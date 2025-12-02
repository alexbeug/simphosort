// <copyright file="PhotoFileInfoWithDuplicates.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Values
{
    /// <inheritdoc/>
    public class PhotoFileInfoWithDuplicates : IPhotoFileInfoWithDuplicates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoFileInfoWithDuplicates"/> class.
        /// </summary>
        /// <param name="fileInfo">File info</param>
        public PhotoFileInfoWithDuplicates(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        /// <inheritdoc/>
        public FileInfo FileInfo { get; set; }

        /// <inheritdoc/>
        public IList<FileInfo> Duplicates { get; } = new List<FileInfo>();
    }
}