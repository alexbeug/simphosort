// <copyright file="IGroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Service for grouping files in sub folders
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Group files in a folder by formatted fixed file date
        /// </summary>
        /// <param name="folder">Folder containing the files to group</param>
        /// <param name="formatString">Format string</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        ErrorLevel GroupFixed(string folder, string formatString, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken);
    }
}