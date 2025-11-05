// <copyright file="PhotoFileInfoComparerConfig.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Comparer
{
    /// <summary>
    /// Config class for <see cref="IPhotoFileInfoComparerFactory"/>.
    /// </summary>
    internal class PhotoFileInfoComparerConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to compare file names as well.
        /// </summary>
        public bool CompareFileSize { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether file name comparisons should ignore case.
        /// </summary>
        public bool CompareFileNameCaseInSensitive { get; set; } = false;
    }
}