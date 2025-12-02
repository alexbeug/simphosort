// <copyright file="GroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;
using Simphosort.Core.Values;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class GroupService : IGroupService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Services.GroupService"/> class.
        /// </summary>
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="fileService">A <see cref="IFileService"/>.</param>
        public GroupService(IFolderService folderService, ISearchService searchService, IFileService fileService)
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
        public ErrorLevel GroupFixed(string folder, string formatString, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Group files");
            callbackLog($"   folder : {folder}");
            callbackLog($"   format : {formatString}");
            searchPatterns.ToList().ForEach(s => callbackLog($"   search : {s}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Check before grouping
            ErrorLevel errorLevel = Check(folder, formatString, callbackLog, callbackError, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Search files in folder
            errorLevel = SearchFolder(folder, searchPatterns, callbackLog, callbackError, out IEnumerable<IPhotoFileInfo> files, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Group files by fixed formatted date
            errorLevel = GroupFilesFixed(formatString, callbackLog, callbackError, files, out Dictionary<string, IEnumerable<IPhotoFileInfo>> groupedFiles, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Get total file count before moving (enumeration will be lost after)
            int total = files.Count();

            // Move files to sub folders
            int moved = FileService.MoveGroupedFilesToSubFolders(groupedFiles, folder, callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{moved} files moved\n");

            // Finish grouping operation (print result and return error level)
            return Finish(start, total, moved, callbackLog, callbackError, cancellationToken);
        }

        /// <summary>
        /// Group files by fixed formatted date
        /// </summary>
        /// <param name="formatString">Format string</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">Ungrouped files in folder</param>
        /// <param name="groupedFiles">Grouped files</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="groupedFiles"/></returns>
        private static ErrorLevel GroupFilesFixed(string formatString, Action<string> callbackLog, Action<string> callbackError, IEnumerable<IPhotoFileInfo> files, out Dictionary<string, IEnumerable<IPhotoFileInfo>> groupedFiles, CancellationToken cancellationToken)
        {
            // Create Dictionary for grouped files
            groupedFiles = new();

            foreach (IPhotoFileInfo file in files.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                string dateString = string.Empty;
                try
                {
                    // Get formatted date string
                    dateString = file.FileInfo.LastWriteTime.ToString(formatString);
                }
                catch (FormatException)
                {
                    // Return when format string is not valid for a file (should not happen because of previous check)
                    callbackError($"ERROR: Format string {formatString} is not valid for file {file.FileInfo.Name}!");
                    return ErrorLevel.FormatStringNotValid;
                }

                // Get or create list for date string
                if (!groupedFiles.TryGetValue(dateString, out IEnumerable<IPhotoFileInfo>? group))
                {
                    group = new List<IPhotoFileInfo>();
                    groupedFiles.Add(dateString, group);
                    callbackLog($"{dateString} added as new group");
                }

                // Add file to list
                ((List<IPhotoFileInfo>)group).Add(file);
                callbackLog($"   -> {file.FileInfo.Name} added to group {dateString}");
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled before grouping files\n");
                return ErrorLevel.Canceled;
            }

            // Log number of groups found
            callbackLog($"\n{groupedFiles.Count} groups formed\n");
            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Finish grouping operation
        /// </summary>
        /// <param name="start">Start time</param>
        /// <param name="total">Total files to group</param>
        /// <param name="moved">Moved grouped files</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        private static ErrorLevel Finish(DateTime start, int total, int moved, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Break operation when cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled while grouping files (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            if (moved == total)
            {
                callbackLog($"Group completed successfully (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Ok;
            }
            else
            {
                callbackError($"Group completed with errors (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.GroupFailed;
            }
        }

        /// <summary>
        /// Check grouping operation
        /// </summary>
        /// <param name="folder">Folder containing the files to group</param>
        /// <param name="formatString">Format string</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="files"/></returns>
        private ErrorLevel Check(string folder, string formatString, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Check folder name for validity
            if (!FolderService.IsValid(folder, callbackError))
            {
                // Stop if folder name is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folders for existence
            if (!FolderService.Exists(folder, callbackError))
            {
                // Stop if folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Folder must not have sub folders
            if (!FolderService.HasNoSubFolders(folder, callbackError))
            {
                // Stop if sub folders present
                return ErrorLevel.FoldersPresent;
            }

            // Check format string for emptiness
            if (string.IsNullOrWhiteSpace(formatString))
            {
                callbackError("ERROR: Format string must not be empty!");
                return ErrorLevel.FormatStringEmpty;
            }

            // Check format string for validity
            try
            {
                DateTime.Now.ToString(formatString);
            }
            catch (FormatException)
            {
                callbackError($"ERROR: Format string {formatString} is not valid!");
                return ErrorLevel.FormatStringNotValid;
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled before grouping files\n");
                return ErrorLevel.Canceled;
            }

            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Search files in folder
        /// </summary>
        /// <param name="folder">Folder containing the files to group</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">Ungrouped files in folder</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="files"/></returns>
        private ErrorLevel SearchFolder(string folder, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out IEnumerable<IPhotoFileInfo> files, CancellationToken cancellationToken)
        {
            // Find files in folder (non-recursive)
            callbackLog($"Searching files in folder...");
            if (SearchService.TrySearchFiles(folder, searchPatterns, false, out files, cancellationToken))
            {
                // Break operation if cancellation requested
                if (cancellationToken.IsCancellationRequested)
                {
                    callbackLog($"Group canceled before grouping files\n");
                    return ErrorLevel.Canceled;
                }

                // Log number of files found
                callbackLog($"   -> {files.Count()} files found in folder\n");
            }
            else
            {
                callbackError($"ERROR: Searching files failed!");
                return ErrorLevel.SearchFailed;
            }

            return ErrorLevel.Ok;
        }

        #endregion // Methods
    }
}