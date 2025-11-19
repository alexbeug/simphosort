// <copyright file="IPhotoFileInfoComparerFactory.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Comparer
{
    /// <summary>
    /// Factory for <see cref="IPhotoFileInfoEqualityComparer"/> and <see cref="IPhotoFileInfoFileOrderComparer"/>.
    /// </summary>
    internal interface IPhotoFileInfoComparerFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IPhotoFileInfoEqualityComparer"/> based on the given config.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>A <see cref="IPhotoFileInfoEqualityComparer"/> instance</returns>
        IPhotoFileInfoEqualityComparer CreateEqualityComparer(PhotoFileInfoEqualityComparerConfig config);

        /// <summary>
        /// Creates a new instance of <see cref="IPhotoFileInfoFileOrderComparer"/> based on the given config.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>A <see cref="IPhotoFileInfoFileOrderComparer"/> instance</returns>
        IPhotoFileInfoFileOrderComparer CreateFileOrderComparer(PhotoFileInfoFileOrderComparerConfig config);
    }
}