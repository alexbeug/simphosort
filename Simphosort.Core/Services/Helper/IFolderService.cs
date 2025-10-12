// <copyright file="IFolderService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Helper
{
    /// <summary>
    /// Folder services
    /// </summary>
    internal interface IFolderService
    {
        /// <summary>
        /// Checks folder naming for validity
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when folder name valid</returns>
        bool IsValid(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks folder for existence
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when folder exists</returns>
        bool Exists(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks folder for emptiness
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when folder empty</returns>
        bool IsEmpty(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks for existing sub folders
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when sub folders present</returns>
        bool HasSubFolders(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks for sub folders
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when no sub folders present</returns>
        bool HasNoSubFolders(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks for files in a folder
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when no sub folders present</returns>
        bool HasNoFiles(string folder, Action<string> callbackError);

        /// <summary>
        /// Checks folders for uniqueness
        /// </summary>
        /// <param name="folders">Folder paths</param>
        /// <param name="callbackError">Error callback function</param>
        /// <returns>true when identical</returns>
        bool IsUnique(IEnumerable<string> folders, Action<string> callbackError);
    }
}