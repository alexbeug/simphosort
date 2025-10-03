// <copyright file="IFileService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Helper
{
    /// <summary>
    /// File services
    /// </summary>
    internal interface IFileService
    {
        /// <summary>
        /// Copy files to target folder
        /// </summary>
        /// <param name="files">Files to copy</param>
        /// <param name="targetFolder">target folder</param>
        /// <param name="callbackLog">Log callback function</param>
        /// <param name="callbackError">Error callback function</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>Files copied</returns>
        int CopyFiles(IEnumerable<FileInfo> files, string targetFolder, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken);
    }
}