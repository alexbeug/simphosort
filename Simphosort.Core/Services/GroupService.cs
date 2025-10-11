// <copyright file="GroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;

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
        public ErrorLevel GroupFixed(string folder, string formatString, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Group files");
            callbackLog($"   folder : {folder}");
            callbackLog($"   format : {formatString}");
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Prepare grouping and get files in folder
            ErrorLevel errorLevel = Prepare(folder, formatString, callbackLog, callbackError, out List<FileInfo> files, cancellationToken);
            if (errorLevel == ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Group files by fixed formatted date
            errorLevel = GroupFilesFixed(formatString, callbackLog, callbackError, files, out Dictionary<string, List<FileInfo>> groupedFiles, cancellationToken);
            if (errorLevel == ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Move files to sub folders
            int moved = FileService.MoveGroupedFilesToSubFolders(groupedFiles, folder, callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{moved} files moved");

            // Log operation duration and remove milliseconds and microseconds for better readability
            TimeSpan duration = DateTime.UtcNow - start;
            duration = duration.Subtract(new TimeSpan(0, 0, 0, 0, duration.Milliseconds, duration.Microseconds));

            // Break operation when cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled while grouping files (Duration: {duration:g})\n");
                return ErrorLevel.Canceled;
            }

            if (moved == files.Count)
            {
                callbackLog($"Group completed successfully after (Duration: {duration:g})\n");
                return ErrorLevel.Ok;
            }
            else
            {
                callbackError($"Group completed with errors after (Duration: {duration:g})\n");
                return ErrorLevel.GroupFailed;
            }
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
        private static ErrorLevel GroupFilesFixed(string formatString, Action<string> callbackLog, Action<string> callbackError, List<FileInfo> files, out Dictionary<string, List<FileInfo>> groupedFiles, CancellationToken cancellationToken)
        {
            // Create Dictionary for grouped files
            groupedFiles = new();

            foreach (FileInfo file in files.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                string dateString = string.Empty;
                try
                {
                    // Get formatted date string
                    dateString = file.LastWriteTime.ToString(formatString);
                }
                catch (FormatException)
                {
                    // Return when format string is not valid for a file (should not happen because of previous check)
                    callbackError($"ERROR: Format string {formatString} is not valid for file {file.Name}!");
                    return ErrorLevel.Canceled;
                }

                // Get or create list for date string
                if (!groupedFiles.TryGetValue(dateString, out List<FileInfo>? list))
                {
                    list = new List<FileInfo>();
                    groupedFiles.Add(dateString, list);
                    callbackLog($"   -> {dateString} added as new group");
                }

                // Add file to list
                list.Add(file);
                callbackLog($"   -> {file.Name} added to group {dateString}");
            }

            // Log number of groups found
            callbackLog($"{groupedFiles.Count} groups formed");
            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Prepare grouping operation
        /// </summary>
        /// <param name="folder">Folder containing the files to group</param>
        /// <param name="formatString">Format string</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">Ungrouped files in folder</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/> and <paramref name="files"/></returns>
        private ErrorLevel Prepare(string folder, string formatString, Action<string> callbackLog, Action<string> callbackError, out List<FileInfo> files, CancellationToken cancellationToken)
        {
            // Prepare empty file list
            files = new();

            // Check folders for validity
            if (!FolderService.IsValid(folder, callbackError))
            {
                // Stop when a folder is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folders for existence
            if (!FolderService.Exists(folder, callbackError))
            {
                // Stop when a folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Target folder must not have sub folders
            if (!FolderService.HasNoSubFolders(folder, callbackError))
            {
                // Stop when target folder is not empty
                return ErrorLevel.FoldersPresent;
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

            // Break operation when cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled before grouping files\n");
                return ErrorLevel.Canceled;
            }

            // Find files in folder (non-recursive)
            callbackLog($"Searching files in folder...");
            files = SearchService.SearchFiles(folder, Constants.SupportedExtensions, false, cancellationToken);
            callbackLog($"   -> {files.Count} files found in folder\n");

            // Break operation when cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Group canceled before grouping files\n");
                return ErrorLevel.Canceled;
            }

            return ErrorLevel.Ok;
        }

        #endregion // Methods
    }
}