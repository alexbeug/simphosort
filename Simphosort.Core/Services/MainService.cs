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
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="copyService">A <see cref="ICopyService"/>.</param>
        public MainService(IFolderService folderService, ISearchService searchService, ICopyService copyService)
        {
            FolderService = folderService;
            SearchService = searchService;
            CopyService = copyService;
        }

        #endregion // Constructor

        #region Properties

        /// <inheritdoc cref="IFolderService"/>
        private IFolderService FolderService { get; }

        /// <inheritdoc cref="ISearchService"/>
        private ISearchService SearchService { get; }

        /// <inheritdoc cref="ICopyService"/>
        private ICopyService CopyService { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel SortPhotos(string workFolder, string photoFolder, string sortFolder, string junkFolder, Action<string> callbackLog, Action<string> callbackError)
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

            // Find files in work folder (non-recursive)
            List<FileInfo> workFiles = SearchService.SearchFiles(workFolder, Constants.SupportedExtensions, false);

            // Log output working folder
            callbackLog($"{workFiles.Count} image files found in work folder");

            // Find files in photo & junk folder (recursive)
            List<FileInfo> photoFiles = SearchService.SearchFiles(photoFolder, Constants.SupportedExtensions, true);
            List<FileInfo> junkFiles = !string.IsNullOrEmpty(junkFolder) ? SearchService.SearchFiles(junkFolder, Constants.SupportedExtensions, true) : new List<FileInfo>();

            // Reduce  workFiles with existing ones in photoFiles and junkFiles and give a list of files to sort
            List<FileInfo> sortFiles = SearchService.ReduceFiles(workFiles, photoFiles.Concat(junkFiles));

            // Log output after reduce
            callbackLog($"{sortFiles.Count} image files new to sort");

            // Copy reduced files to target folder
            return CopyService.CopyFiles(sortFiles, sortFolder, callbackLog, callbackError) ? ErrorLevel.Ok : ErrorLevel.CopyFailed;
        }

        #endregion // Methods
    }
}