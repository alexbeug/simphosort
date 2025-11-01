// <copyright file="IFileInfoComparerFactory.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Comparer
{
    /// <summary>
    /// Factory for <see cref="IFileInfoComparer"/>.
    /// </summary>
    internal interface IFileInfoComparerFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IFileInfoComparer"/> based on the given config.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>A IFileInfoComparer instance</returns>
        IFileInfoComparer Create(FileInfoComparerConfig config);
    }
}