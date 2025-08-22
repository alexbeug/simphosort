// <copyright file="MainService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class MainService : IMainService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainService"/> class.
        /// </summary>
        /// <param name="folderService">An <see cref="IFolderService"/>.</param>
        public MainService(IFolderService folderService)
        {
            FolderService = folderService;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets folder service
        /// </summary>
        private IFolderService FolderService { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel SortPhotos(string workFolder, string photoFolder, string sortFolder, string junkFolder, Action<string> callbackError)
        {
            // Check folders for existence
            bool folderOk = FolderService.Exists(workFolder, callbackError);
            folderOk = FolderService.Exists(photoFolder, callbackError) && folderOk;
            folderOk = FolderService.Exists(sortFolder, callbackError) && folderOk;

            // Junk folder is optional but has to exist when given as parameter
            folderOk = (string.IsNullOrEmpty(junkFolder) || FolderService.Exists(junkFolder, callbackError)) && folderOk;

            if (!folderOk)
            {
                // Stop when a folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Put the three mandantory folders into a list
            List<string> folders = new()
            {
                workFolder, photoFolder, sortFolder,
            };

            // Add optional junk folder when given
            if (!string.IsNullOrEmpty(junkFolder))
            {
                folders.Add(junkFolder);
            }

            // Check folders in list for uniqueness
            folderOk = FolderService.Unique(folders, callbackError) && folderOk;

            if (!folderOk)
            {
                // Stop when folders are not unique
                return ErrorLevel.FoldersAreNotUnique;
            }

            callbackError("ERROR: Nothing implemented yet!");
            return ErrorLevel.Ok;
        }

        #endregion // Methods
    }
}