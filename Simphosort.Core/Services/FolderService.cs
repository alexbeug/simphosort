// <copyright file="FolderService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class FolderService : IFolderService
    {
        /// <inheritdoc/>
        public bool Exists(string folder, Action<string> callbackError)
        {
            if (!Directory.Exists(folder))
            {
                callbackError($"ERROR: Folder {folder} does not exist!");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Empty(string folder, Action<string> callbackError)
        {
            if (Directory.EnumerateFiles(folder).Any() || Directory.EnumerateDirectories(folder).Any())
            {
                callbackError($"ERROR: Folder {folder} is not empty!");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Unique(IEnumerable<string> folders, Action<string> callbackError)
        {
            if (folders.Select(Path.GetFullPath).Distinct().Count() != folders.Count())
            {
                callbackError($"ERROR: Folders are not unique!");
                return false;
            }

            return true;
        }
    }
}