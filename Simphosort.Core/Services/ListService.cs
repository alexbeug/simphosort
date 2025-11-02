// <copyright file="ListService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Enums;
using Simphosort.Core.Services.Comparer;
using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;

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
        /// <param name="fileInfoComparerFactory">A <see cref="IFileInfoComparerFactory"/>.</param>
        public ListService(IFolderService folderService, ISearchService searchService, IFileInfoComparerFactory fileInfoComparerFactory)
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

        /// <inheritdoc cref="IFileInfoComparerFactory"/>
        private IFileInfoComparerFactory FileInfoComparerFactory { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel List(string folder, bool fileDetails, IEnumerable<FileOrder> fileOrder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"List files");
            callbackLog($"   folder : {folder}");
            callbackLog($"   details: {fileDetails}");
            fileOrder.ToList().ForEach(o => callbackLog($"   order  : {o}"));
            searchPatterns.ToList().ForEach(s => callbackLog($"   search : {s}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Prepare ungrouping and get files in parent folder and sub folders
            ErrorLevel errorLevel = Prepare(folder, searchPatterns, callbackLog, callbackError, out IEnumerable<FileInfo> files, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Count files found
            int total = 0;

            // Append order by criterias
            files = AppendOrderBy(files, fileOrder, callbackLog, cancellationToken);

            // List files
            foreach (FileInfo file in files.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                ListFile(file, fileDetails, callbackLog);
                total++;
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

        /// <summary>
        /// Append order criterias
        /// </summary>
        /// <param name="files">IEnumerable of <see cref="FileInfo"/></param>
        /// <param name="fileOrder"><see cref="FileOrder"/></param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>Ordered IEnumerable</returns>
        private static IEnumerable<FileInfo> AppendOrderBy(IEnumerable<FileInfo> files, IEnumerable<FileOrder> fileOrder, Action<string> callbackLog, CancellationToken cancellationToken)
        {
            if (fileOrder != null && fileOrder.Any() && !fileOrder.All(a => a == FileOrder.None))
            {
                // Log total files found
                callbackLog($"\nAppending order criterias...\n");

                bool firstOrder = true;

                foreach (FileOrder order in fileOrder)
                {
                    if (firstOrder)
                    {
                        files = OrderBy.AppendOrderBy(files, order, cancellationToken);
                        firstOrder = false;
                    }
                    else
                    {
                        files = OrderBy.AppendThenBy((IOrderedEnumerable<FileInfo>)files, order);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// List file information
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileDetails">List file details</param>
        /// <param name="callbackLog">Log message callback</param>
        private static void ListFile(FileInfo file, bool fileDetails, Action<string> callbackLog)
        {
            callbackLog($"{file.FullName}");

            // List file details
            if (fileDetails)
            {
                callbackLog($"   Size    : {file.Length} bytes");
                callbackLog($"   Created : {file.CreationTimeUtc:u}");
                callbackLog($"   Modified: {file.LastWriteTimeUtc:u}");
                callbackLog($"   Accessed: {file.LastAccessTimeUtc:u}");
                callbackLog(string.Empty);
            }
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
        private ErrorLevel Prepare(string folder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out IEnumerable<FileInfo> files, CancellationToken cancellationToken)
        {
            // Always initialize out parameters
            files = new List<FileInfo>();

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

        #endregion // Methods
    }
}