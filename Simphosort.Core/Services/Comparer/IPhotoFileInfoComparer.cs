// <copyright file="IPhotoFileInfoComparer.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Values;

namespace Simphosort.Core.Services.Comparer
{
    /// <summary>
    /// Special comparer for <see cref="FileInfo"/> with custom comparison options.
    /// </summary>
    public interface IPhotoFileInfoComparer : IEqualityComparer<IPhotoFileInfo>
    {
    }
}