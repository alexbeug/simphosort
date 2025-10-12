// <copyright file="CopyService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class CopyService : ICopyService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Services.CopyService"/> class.
        /// </summary>
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="fileService">A <see cref="IFileService"/>.</param>
        public CopyService(IFolderService folderService, ISearchService searchService, IFileService fileService)
        {
            FolderService = folderService;
            SearchService = searchService;
            FileService = fileService;
        }

        #endregion // Constructor

        #region Properties

        /// <inheritdoc cref="IFolderService"/>
        private IFolderService FolderService { get; }

        /// <inheritdoc cref="ISearchService"/>
        private ISearchService SearchService { get; }

        /// <inheritdoc cref="IFileService"/>
        private IFileService FileService { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel Copy(string sourceFolder, string targetFolder, IEnumerable<string>? checkFolders, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Copy files");
            callbackLog($"   source : {sourceFolder}");
            callbackLog($"   target : {targetFolder}");
            checkFolders?.ToList().ForEach(f => callbackLog($"   check  : {f}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Put the mandatory folders into a list
            List<string> folders = new()
            {
                sourceFolder, targetFolder,
            };

            // Add optional check folders when given
            if (checkFolders != null)
            {
                folders.AddRange(checkFolders.Where(x => !string.IsNullOrEmpty(x)));
            }

            // Check folder names for validity
            if (!folders.All(x => FolderService.IsValid(x, callbackError)))
            {
                 // Stop if a folder name is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folders for existence
            if (!folders.All(x => FolderService.Exists(x, callbackError)))
            {
                 // Stop if a folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Target folder has to be empty
            if (!FolderService.IsEmpty(targetFolder, callbackError))
            {
                // Stop if target folder is not empty
                return ErrorLevel.FolderNotEmpty;
            }

            // Check folders in list for uniqueness
            if (!FolderService.IsUnique(folders, callbackError))
            {
                // Stop if folders are not unique
                return ErrorLevel.FolderNamesNotUnique;
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled before copying files\n");
                return ErrorLevel.Canceled;
            }

            // Find files in source folder (non-recursive)
            callbackLog($"Searching files in source folder...");
            List<FileInfo> sourceFiles = SearchService.SearchFiles(sourceFolder, Constants.SupportedExtensions, false, cancellationToken);
            callbackLog($"   -> {sourceFiles.Count} files found in source folder\n");

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled before copying files\n");
                return ErrorLevel.Canceled;
            }

            // Prepare list for files to copy
            List<FileInfo> copyFiles;

            if (checkFolders != null && checkFolders.Any())
            {
                // Find files in check folders (recursive)
                callbackLog($"Searching files in check folders...");
                List<FileInfo> checkFiles = new();
                checkFolders.TakeWhile(c => !cancellationToken.IsCancellationRequested).ToList().ForEach(folder => checkFiles.AddRange(SearchService.SearchFiles(folder, Constants.SupportedExtensions, true, cancellationToken).TakeWhile(s => !cancellationToken.IsCancellationRequested)));
                callbackLog($"   -> {checkFiles.Count} files found in check folders\n");

                // Break operation if cancellation requested
                if (cancellationToken.IsCancellationRequested)
                {
                    callbackLog($"Copy canceled before copying files\n");
                    return ErrorLevel.Canceled;
                }

                // Compare sourceFiles with existing ones in checkFolders and give a list of files to copy
                callbackLog($"Comparing files in source and check folders...");
                copyFiles = SearchService.ReduceFiles(sourceFiles, checkFiles, cancellationToken);
                callbackLog($"   -> {copyFiles.Count} new files to copy\n");
            }
            else
            {
                // No check folders given, so all files from source folder are new
                copyFiles = sourceFiles;
                callbackLog($"   -> {copyFiles.Count} source files to copy\n");
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled before copying files\n");
                return ErrorLevel.Canceled;
            }

            // Copy files to target folder
            int copied = FileService.CopyFiles(copyFiles, targetFolder, callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{copied} of {copyFiles.Count} files copied\n");

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled while copying files (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            if (copied == copyFiles.Count)
            {
                callbackLog($"Copy completed successfully (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Ok;
            }
            else
            {
                callbackError($"Copy completed with errors (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.CopyFailed;
            }
        }

        #endregion // Methods
    }
}