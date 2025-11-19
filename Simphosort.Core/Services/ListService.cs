// <copyright file="ListService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;
using Simphosort.Core.Services.Comparer;
using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;
using Simphosort.Core.Values;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class ListService : IListService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListService"/> class.
        /// </summary>
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="fileInfoComparerFactory">A <see cref="IPhotoFileInfoComparerFactory"/>.</param>
        public ListService(IFolderService folderService, ISearchService searchService, IPhotoFileInfoComparerFactory fileInfoComparerFactory)
        {
            FolderService = folderService;
            SearchService = searchService;
            FileInfoComparerFactory = fileInfoComparerFactory;
        }

        #endregion // Constructor

        #region Properties

        /// <inheritdoc cref="IFolderService"/>
        private IFolderService FolderService { get; }

        /// <inheritdoc cref="ISearchService"/>
        private ISearchService SearchService { get; }

        /// <inheritdoc cref="IPhotoFileInfoComparerFactory"/>
        private IPhotoFileInfoComparerFactory FileInfoComparerFactory { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel List(string folder, bool fileDetails, bool onlyDuplicates, IEnumerable<FileOrder> fileOrder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"List files");
            callbackLog($"   folder    : {folder}");
            callbackLog($"   details   : {fileDetails}");
            callbackLog($"   duplicates: {onlyDuplicates}");
            fileOrder.ToList().ForEach(o => callbackLog($"   order     : {o}"));
            searchPatterns.ToList().ForEach(s => callbackLog($"   search    : {s}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Prepare ungrouping and get files in parent folder and sub folders
            ErrorLevel errorLevel = Prepare(folder, searchPatterns, callbackLog, callbackError, out IEnumerable<IPhotoFileInfo> files, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Count files found
            int total = 0;

            if (onlyDuplicates)
            {
                // List only duplicate files
                total = ListAllDuplicates(files, fileDetails, fileOrder, callbackLog, cancellationToken);
            }
            else
            {
                // List all files
                total = ListAllFiles(files, fileDetails, fileOrder, callbackLog, cancellationToken);
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"List canceled while listing files (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            // Log total files found
            callbackLog($"\n{total} files found\n");

            // Log successful completion
            callbackLog($"List completed successfully (Duration: {Duration.Calculate(start):g})\n");
            return ErrorLevel.Ok;
        }

        #endregion // Methods

        #region Private methods

        /// <summary>
        /// List file information
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileDetails">List file details</param>
        /// <param name="callbackLog">Log message callback</param>
        private static void ListSingleFile(IPhotoFileInfo file, bool fileDetails, Action<string> callbackLog)
        {
            callbackLog($"{file.FileInfo.FullName}");

            // List file details
            if (fileDetails)
            {
                callbackLog($"   Size    : {file.FileInfo.Length} bytes");
                callbackLog($"   Created : {file.FileInfo.CreationTimeUtc:u}");
                callbackLog($"   Modified: {file.FileInfo.LastWriteTimeUtc:u}");
                callbackLog($"   Accessed: {file.FileInfo.LastAccessTimeUtc:u}");
                callbackLog(string.Empty);
            }
        }

        /// <summary>
        /// List duplicates file information
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileDetails">List file details</param>
        /// <param name="callbackLog">Log message callback</param>
        private static void ListSingleDuplicates(IPhotoFileInfoWithDuplicates file, bool fileDetails, Action<string> callbackLog)
        {
            // List original file
            ListSingleFile(file, fileDetails, callbackLog);

            callbackLog($"   Duplicates: ");

            // List duplicate files
            foreach (FileInfo duplicate in file.Duplicates)
            {
                callbackLog($"      -> {duplicate.FullName}");

                // List file details
                if (fileDetails)
                {
                    callbackLog($"         Size    : {duplicate.Length} bytes");
                    callbackLog($"         Created : {duplicate.CreationTimeUtc:u}");
                    callbackLog($"         Modified: {duplicate.LastWriteTimeUtc:u}");
                    callbackLog($"         Accessed: {duplicate.LastAccessTimeUtc:u}");
                    callbackLog(string.Empty);
                }
            }

            callbackLog(string.Empty);
        }

        /// <summary>
        /// List all files with duplicates only
        /// </summary>
        /// <param name="files">Files to list</param>
        /// <param name="fileDetails">File details</param>
        /// <param name="fileOrder">File sorting order</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>Total files listed</returns>
        private int ListAllDuplicates(IEnumerable<IPhotoFileInfo> files, bool fileDetails, IEnumerable<FileOrder> fileOrder, Action<string> callbackLog, CancellationToken cancellationToken)
        {
            // Total duplicate files listed
            int total = 0;

            PhotoFileInfoEqualityComparerConfig fileInfoComparerConfig = new()
            {
                // Compare file names by default
                CompareFileName = true,

                // Do not force case insensitive file name comparison by default
                CompareFileNameCaseInSensitive = false,

                // Find real duplicate files by comparing file size as well
                CompareFileSize = true,
            };

            // Create comparer with desired configuration
            IPhotoFileInfoEqualityComparer duplicateComparer = FileInfoComparerFactory.CreateEqualityComparer(fileInfoComparerConfig);

            // Find duplicate files
            callbackLog($"   -> Testing for duplicate files...");
            IEnumerable<IPhotoFileInfoWithDuplicates> duplicates = SearchService.FindDuplicateFiles(files, duplicateComparer, cancellationToken);

            if (fileOrder.Any(o => o != FileOrder.None))
            {
                // Create comparer config
                PhotoFileInfoFileOrderComparerConfig fileOrderComparerConfig = new()
                {
                    // Order by given file order criterias
                    CompareFileOrder = fileOrder,
                };

                // Create comparer with desired configuration
                IPhotoFileInfoFileOrderComparer orderComparer = FileInfoComparerFactory.CreateFileOrderComparer(fileOrderComparerConfig);

                // Order duplicates by given order criterias
                duplicates = duplicates.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(k => k, orderComparer);
            }

            foreach (IPhotoFileInfoWithDuplicates duplicate in duplicates.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                ListSingleDuplicates(duplicate, fileDetails, callbackLog);
                total++;
            }

            return total;
        }

        /// <summary>
        /// List all files without filtering duplicates
        /// </summary>
        /// <param name="files">Files to list</param>
        /// <param name="fileDetails">File details</param>
        /// <param name="fileOrder">File sorting order</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>Total files listed</returns>
        private int ListAllFiles(IEnumerable<IPhotoFileInfo> files, bool fileDetails, IEnumerable<FileOrder> fileOrder, Action<string> callbackLog, CancellationToken cancellationToken)
        {
            // Total files listed
            int total = 0;

            // Append file order criterias
            if (fileOrder.Any(o => o != FileOrder.None))
            {
                // Create comparer config
                PhotoFileInfoFileOrderComparerConfig fileInfoComparerConfig = new()
                {
                    // Order by given file order criterias
                    CompareFileOrder = fileOrder,
                };

                // Create comparer with desired configuration
                IPhotoFileInfoFileOrderComparer orderComparer = FileInfoComparerFactory.CreateFileOrderComparer(fileInfoComparerConfig);

                // List files without specific order
                foreach (IPhotoFileInfo file in files.TakeWhile(c => !cancellationToken.IsCancellationRequested).OrderBy(k => k, orderComparer))
                {
                    ListSingleFile(file, fileDetails, callbackLog);
                    total++;
                }
            }
            else
            {
                // List files without specific order
                foreach (IPhotoFileInfo file in files.TakeWhile(c => !cancellationToken.IsCancellationRequested))
                {
                    ListSingleFile(file, fileDetails, callbackLog);
                    total++;
                }
            }

            return total;
        }

        /// <summary>
        /// Prepare listing by validating folder and getting files in folder and sub folders.
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">Files found in folder and sub folders</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/>, <paramref name="parentFiles"/> abd <paramref name="subFiles"/></returns>
        private ErrorLevel Prepare(string folder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out IEnumerable<IPhotoFileInfo> files, CancellationToken cancellationToken)
        {
            // Always initialize out parameters
            files = new List<IPhotoFileInfo>();

            // Check folder name for validity
            if (!FolderService.IsValid(folder, callbackError))
            {
                // Stop if folder name is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folder for existence
            if (!FolderService.Exists(folder, callbackError))
            {
                // Stop if folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"List canceled before listing files\n");
                return ErrorLevel.Canceled;
            }

            // Get all files in folder and sub folders (recursive)
            callbackLog($"Searching files in folder and sub folders...");
            if (!SearchService.TrySearchFiles(folder, searchPatterns, true, out files, cancellationToken))
            {
                callbackError("ERROR: Searching files failed!");
                return ErrorLevel.SearchFailed;
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"List canceled before listing files\n");
                return ErrorLevel.Canceled;
            }

            return ErrorLevel.Ok;
        }

        #endregion // Private methods
    }
}