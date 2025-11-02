// <copyright file="IListService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;
using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Service for listing photo files
    /// </summary>
    public interface IListService
    {
        /// <summary>
        /// List files in folder and its sub folders
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <param name="fileDetails">List file details</param>
        /// <param name="fileOrder">File order</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        public ErrorLevel List(string folder, bool fileDetails, IEnumerable<FileOrder> fileOrder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken);
    }
}