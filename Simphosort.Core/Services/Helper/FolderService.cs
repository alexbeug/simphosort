// <copyright file="FolderService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class FolderService : IFolderService
    {
        /// <inheritdoc/>
        public bool IsValid(string folder, Action<string> callbackError)
        {
            if (Path.GetInvalidPathChars().AsEnumerable().Any(folder.Contains))
            {
                callbackError($"ERROR: Folder {folder} contains invalid characters!");
                return false;
            }

            return true;
        }

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
        public bool IsEmpty(string folder, Action<string> callbackError)
        {
            try
            {
                if (Directory.EnumerateFiles(folder).Any() || Directory.EnumerateDirectories(folder).Any())
                {
                    callbackError($"ERROR: Folder {folder} is not empty!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                callbackError($"ERROR: Could not check folder {folder} for emptiness! {ex.Message}");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool HasSubFolders(string folder, Action<string> callbackError)
        {
            try
            {
                if (!Directory.EnumerateDirectories(folder).Any())
                {
                    callbackError($"ERROR: Folder {folder} contains not sub folders!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                callbackError($"ERROR: Could not check folder {folder} for sub folders! {ex.Message}");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool HasNoSubFolders(string folder, Action<string> callbackError)
        {
            try
            {
                if (Directory.EnumerateDirectories(folder).Any())
                {
                    callbackError($"ERROR: Folder {folder} already contains sub folders!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                callbackError($"ERROR: Could not check folder {folder} for no sub folders! {ex.Message}");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsUnique(IEnumerable<string> folders, Action<string> callbackError)
        {
            try
            {
                // Check for uniqueness by comparing full paths case insensitive. If number of distinct paths is less than
                // number of folders, there are duplicates. Does not support case sensitive aware testing without running
                // into issues when running on case insensitive file or operating system.
                if (folders.Select(p => Path.GetFullPath(p).ToLowerInvariant()).Distinct().Count() != folders.Count())
                {
                    callbackError($"ERROR: Folders are not unique!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                callbackError($"ERROR: Could not check folders for uniqueness! {ex.Message}");
                return false;
            }

            return true;
        }
    }
}