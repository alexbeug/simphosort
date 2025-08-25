// <copyright file="IFileInfoComparer.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Special comparer for <see cref="FileInfo"/>
    /// </summary>
    internal interface IFileInfoComparer : IEqualityComparer<FileInfo>
    {
    }
}