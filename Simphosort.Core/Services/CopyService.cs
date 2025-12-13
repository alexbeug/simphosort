// <copyright file="CopyService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;
using Simphosort.Core.Values;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class CopyService : ICopyService
    {
        #region Fields

        /// <inheritdoc cref="IFolderService"/>
        private readonly IFolderService _folderService;

        /// <inheritdoc cref="ISearchService"/>
        private readonly ISearchService _searchService;

        /// <inheritdoc cref="IFileService"/>
        private readonly IFileService _fileService;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Services.CopyService"/> class.
        /// </summary>
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="fileService">A <see cref="IFileService"/>.</param>
        public CopyService(IFolderService folderService, ISearchService searchService, IFileService fileService)
        {
            _folderService = folderService;
            _searchService = searchService;
            _fileService = fileService;
        }

        #endregion // Constructor

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel Copy(string sourceFolder, string targetFolder, IEnumerable<string>? checkFolders, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Copy files");
            callbackLog($"   source : {sourceFolder}");
            callbackLog($"   target : {targetFolder}");
            checkFolders?.ToList().ForEach(c => callbackLog($"   check  : {c}"));
            searchPatterns.ToList().ForEach(s => callbackLog($"   search : {s}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Check folders
            ErrorLevel errorLevel = Check(sourceFolder, targetFolder, checkFolders, callbackLog, callbackError, cancellationToken);

            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Prepare operation and search source files
            errorLevel = SearchSource(sourceFolder, searchPatterns, callbackLog, callbackError, out IEnumerable<IPhotoFileInfo> sourceFiles, cancellationToken);

            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Prepare list for files to copy
            IEnumerable<IPhotoFileInfo> copyFiles;

            if (checkFolders != null && checkFolders.Any())
            {
                // Find files in check folders (recursive)
                errorLevel = SearchCheck(checkFolders, searchPatterns, callbackLog, callbackError, out List<IPhotoFileInfo> checkFiles, cancellationToken);

                if (errorLevel != ErrorLevel.Ok)
                {
                    return errorLevel;
                }

                // Log number of files found
                callbackLog($"   -> {checkFiles.Count} files found in check folders\n");

                // Compare sourceFiles with existing ones in checkFolders and give a list of files to copy
                callbackLog($"Comparing files in source and check folders...");
                copyFiles = _searchService.ReduceFiles(sourceFiles, checkFiles, cancellationToken);

                // Break operation if cancellation requested
                if (cancellationToken.IsCancellationRequested)
                {
                    callbackLog($"Copy canceled before copying files\n");
                    return ErrorLevel.Canceled;
                }

                // Log number of new files to copy
                callbackLog($"   -> {copyFiles.Count()} new files to copy\n");
            }
            else
            {
                // No check folders given, so all files from source folder are new
                copyFiles = sourceFiles;
                callbackLog($"   -> {copyFiles.Count()} source files to copy\n");
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled before copying files\n");
                return ErrorLevel.Canceled;
            }

            // Copy files to target folder
            int copied = _fileService.CopyFiles(copyFiles, targetFolder, callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{copied} of {copyFiles.Count()} files copied\n");

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled while copying files (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            if (copied == copyFiles.Count())
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

        /// <summary>
        /// Check folders for validity, existence, emptiness and uniqueness
        /// </summary>
        /// <param name="sourceFolder">Source folder</param>
        /// <param name="targetFolder">Target folder</param>
        /// <param name="checkFolders">Check folders for duplicates</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="files"/></returns>
        private ErrorLevel Check(string sourceFolder, string targetFolder, IEnumerable<string>? checkFolders, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
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
            if (!folders.All(x => _folderService.IsValid(x, callbackError)))
            {
                // Stop if a folder name is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folders for existence
            if (!folders.All(x => _folderService.Exists(x, callbackError)))
            {
                // Stop if a folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Target folder has to be empty
            if (!_folderService.IsEmpty(targetFolder, callbackError))
            {
                // Stop if target folder is not empty
                return ErrorLevel.FolderNotEmpty;
            }

            // Check folders in list for uniqueness
            if (!_folderService.IsUnique(folders, callbackError))
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

            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Search source folder for files to copy
        /// </summary>
        /// <param name="sourceFolder">Source folder</param>
        /// <param name="searchPatterns">Search pattern</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">returns found source files to copy</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="files"/></returns>
        private ErrorLevel SearchSource(string sourceFolder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out IEnumerable<IPhotoFileInfo> files, CancellationToken cancellationToken)
        {
            // Find files in source folder (non-recursive)
            callbackLog($"Searching files in source folder...");

            if (_searchService.TrySearchFiles(sourceFolder, searchPatterns, false, out files, cancellationToken))
            {
                // Break operation if cancellation requested
                if (cancellationToken.IsCancellationRequested)
                {
                    callbackLog($"Copy canceled before copying files\n");
                    return ErrorLevel.Canceled;
                }

                // Log number of files found
                callbackLog($"   -> {files.Count()} files found in source folder\n");
            }
            else
            {
                callbackError($"ERROR: Searching files failed!");
                return ErrorLevel.SearchFailed;
            }

            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Check for existing files in check folders
        /// </summary>
        /// <param name="checkFolders">Check folders</param>
        /// <param name="searchPatterns">Search pattern</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="checkFiles">returns found check files to exclude</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="checkFiles"/></returns>
        private ErrorLevel SearchCheck(IEnumerable<string> checkFolders, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out List<IPhotoFileInfo> checkFiles, CancellationToken cancellationToken)
        {
            callbackLog($"Searching files in check folders...");
            checkFiles = new();

            foreach (string folder in checkFolders.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                if (_searchService.TrySearchFiles(folder, searchPatterns, true, out IEnumerable<IPhotoFileInfo> foundFiles, cancellationToken))
                {
                    checkFiles.AddRange(foundFiles.TakeWhile(s => !cancellationToken.IsCancellationRequested));
                }
                else
                {
                    callbackError($"ERROR: Searching files in check folder {folder} failed!");
                    return ErrorLevel.SearchFailed;
                }
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Copy canceled before copying files\n");
                return ErrorLevel.Canceled;
            }

            return ErrorLevel.Ok;
        }

        #endregion // Methods
    }
}