// <copyright file="IMainService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <summary>
    /// Main sorting service
    /// </summary>
    public interface IMainService
    {
        /// <summary>
        ///  Copy photos
        /// </summary>
        /// <param name="sourceFolder">source folder</param>
        /// <param name="targetFolder">target folder</param>
        /// <param name="checkFolders">check folders (exclude duplicates)</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <returns><see cref="ErrorLevel"/></returns>
        ErrorLevel CopyPhotos(string sourceFolder, string targetFolder, IEnumerable<string>? checkFolders, Action<string> callbackLog, Action<string> callbackError);
    }
}