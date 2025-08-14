// <copyright file="SortService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class SortService : ISortService
    {
        /// <inheritdoc/>
        public void SortPhotos(string workFolder, string photoFolder, string sortFolder, string junkFolder, Action<string> callbackError)
        {
            callbackError("ERROR: Nothing implemented yet!");
        }
    }
}