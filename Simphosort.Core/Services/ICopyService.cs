// <copyright file="ICopyService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Service to copy files
    /// </summary>
    internal interface ICopyService
    {
        /// <summary>
        /// Copy files to target folder
        /// </summary>
        /// <param name="files">Files to copy</param>
        /// <param name="sortFolder">target folder</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>Copy result</returns>
        bool CopyFiles(IEnumerable<FileInfo> files, string sortFolder, Action<string> callbackError);
    }
}