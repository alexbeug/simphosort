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
        ///  Sort all photos
        /// </summary>
        /// <param name="workFolder">working folder (source)</param>
        /// <param name="photoFolder">photo folder (compare)</param>
        /// <param name="sortFolder">sort folder (target)</param>
        /// <param name="junkFolder">junk folder (ignore)</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <returns><see cref="ErrorLevel"/></returns>
        ErrorLevel SortPhotos(string workFolder, string photoFolder, string sortFolder, string junkFolder, Action<string> callbackLog, Action<string> callbackError);
    }
}