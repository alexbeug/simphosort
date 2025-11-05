// <copyright file="IPhotoFileInfo.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Values
{
    /// <summary>
    /// Photo file information
    /// </summary>
    public interface IPhotoFileInfo
    {
        /// <summary>
        /// Gets or sets FileInfo of the file
        /// </summary>
        FileInfo FileInfo { get; set; }
    }
}