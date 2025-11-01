// <copyright file="IUngroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Service for ungrouping files from sub folders to parent folder
    /// </summary>
    public interface IUngroupService
    {
        /// <summary>
        /// Ungroup files in sub folders and move them to parent folder
        /// </summary>
        /// <param name="parent">Parent folder</param>
        /// <param name="clean">Delete empty sub folders</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        public ErrorLevel Ungroup(string parent, bool clean, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken);
    }
}