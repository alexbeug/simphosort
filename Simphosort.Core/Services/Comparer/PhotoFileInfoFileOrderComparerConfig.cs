// <copyright file="PhotoFileInfoFileOrderComparerConfig.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;

namespace Simphosort.Core.Services.Comparer
{
    /// <summary>
    /// Config class for <see cref="IPhotoFileInfoComparerFactory"/>.
    /// </summary>
    internal class PhotoFileInfoFileOrderComparerConfig
    {
        /// <summary>
        /// Gets or sets the file order criteria for comparison.
        /// </summary>
        public IEnumerable<FileOrder> CompareFileOrder { get; set; } = Array.Empty<FileOrder>();
    }
}