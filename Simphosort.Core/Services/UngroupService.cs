// <copyright file="UngroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Comparer;
using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class UngroupService : IUngroupService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Services.UngroupService"/> class.
        /// </summary>
        /// <param name="folderService">A <see cref="IFolderService"/>.</param>
        /// <param name="searchService">A <see cref="ISearchService"/>.</param>
        /// <param name="fileService">A <see cref="IFileService"/>.</param>
        /// <param name="fileInfoComparerFactory">A <see cref="IFileInfoComparerFactory"/>.</param>
        public UngroupService(IFolderService folderService, ISearchService searchService, IFileService fileService, IFileInfoComparerFactory fileInfoComparerFactory)
        {
            FolderService = folderService;
            SearchService = searchService;
            FileService = fileService;
            FileInfoComparerFactory = fileInfoComparerFactory;
        }

        #endregion // Constructor

        #region Properties

        /// <inheritdoc cref="IFolderService"/>
        private IFolderService FolderService { get; }

        /// <inheritdoc cref="ISearchService"/>
        private ISearchService SearchService { get; }

        /// <inheritdoc cref="IFileService"/>
        private IFileService FileService { get; }

        /// <inheritdoc cref="IFileInfoComparerFactory"/>
        private IFileInfoComparerFactory FileInfoComparerFactory { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public ErrorLevel Ungroup(string parent, bool clean, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Ungroup files");
            callbackLog($"   parent : {parent}");
            callbackLog($"   clean  : {clean}");
            searchPatterns.ToList().ForEach(s => callbackLog($"   search : {s}"));
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

            // Prepare ungrouping and get files in parent folder and sub folders
            ErrorLevel errorLevel = Prepare(parent, searchPatterns, callbackLog, callbackError, out IEnumerable<FileInfo> files, out List<FileInfo> subFiles, cancellationToken);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Check for duplicate file names in sub folders and parent folder
            errorLevel = CheckDuplicates(files, callbackError);
            if (errorLevel != ErrorLevel.Ok)
            {
                return errorLevel;
            }

            // Move files from sub folders to parent folder
            int moved = FileService.MoveFilesFromSubFoldersToFolder(subFiles, parent, callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{moved} files moved\n");

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Ungroup canceled while ungrouping files (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            if (moved == subFiles.Count)
            {
                // Delete empty sub folders
                if (clean)
                {
                    // Get all distinct sub folder paths
                    List<string> paths = subFiles.Select(f => f.DirectoryName).Where(p => p != null).Select(p => p!).Distinct().ToList();

                    // Clean up empty sub folders
                    errorLevel = CleanUp(start, paths, callbackLog, callbackError, cancellationToken);
                    if (errorLevel != ErrorLevel.Ok)
                    {
                        return errorLevel;
                    }
                }

                // Log successful completion
                callbackLog($"Ungroup completed successfully (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Ok;
            }
            else
            {
                // Log completion with errors
                callbackError($"Ungroup completed with errors (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.UngroupFailed;
            }
        }

        /// <summary>
        /// Prepare ungrouping by validating folder and getting files in parent folder and sub folders.
        /// </summary>
        /// <param name="parent">Parent folder</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="files">Files found in parent and sub folders</param>
        /// <param name="subFiles">Files found in sub folders</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/>, <paramref name="parentFiles"/> abd <paramref name="subFiles"/></returns>
        private ErrorLevel Prepare(string parent, IEnumerable<string> searchPatterns, Action<string> callbackLog, Action<string> callbackError, out IEnumerable<FileInfo> files, out List<FileInfo> subFiles, CancellationToken cancellationToken)
        {
            // Always initialize out parameters
            files = new List<FileInfo>();
            subFiles = new List<FileInfo>();

            // Check folder name for validity
            if (!FolderService.IsValid(parent, callbackError))
            {
                // Stop if folder name is not valid
                return ErrorLevel.FolderNotValid;
            }

            // Check folder for existence
            if (!FolderService.Exists(parent, callbackError))
            {
                // Stop if folder does not exist
                return ErrorLevel.FolderDoesNotExist;
            }

            // Parent folder must have sub folders
            if (!FolderService.HasSubFolders(parent, callbackError))
            {
                // Stop if no sub folders are present
                return ErrorLevel.NoSubFolders;
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Ungroup canceled before ungrouping files\n");
                return ErrorLevel.Canceled;
            }

            // Get all files in sub folders (recursive) and parent folder
            callbackLog($"Searching files in parent folder and sub folders...");
            if (!SearchService.TrySearchFiles(parent, searchPatterns, true, out files, cancellationToken))
            {
                callbackError("ERROR: Searching files failed!");
                return ErrorLevel.SearchFailed;
            }

            // Retrieve enumerated files
            List<FileInfo> foundFiles = files.TakeWhile(f => !cancellationToken.IsCancellationRequested).ToList();

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Ungroup canceled before ungrouping files\n");
                return ErrorLevel.Canceled;
            }

            // Separate files in parent folder and sub folders. Casing is not relevant here, because all paths come from the same parent folder
            subFiles = foundFiles.Where(f => !f.DirectoryName!.Equals(parent)).ToList();
            callbackLog($"   -> {subFiles.Count} files found in sub folders");

            // Get files in parent folder by excluding sub folder files from found files
            List<FileInfo> parentFiles = foundFiles.Except(subFiles).ToList();
            callbackLog($"   -> {parentFiles.Count} files found in parent folder\n");

            return ErrorLevel.Ok;
        }

        /// <summary>
        ///  Check for duplicate file names in sub folders and parent folder.
        /// </summary>
        /// <param name="files">Files to check</param>
        /// <param name="callbackError">Error message callback</param>
        /// <returns>An <see cref="ErrorLevel"/></returns>
        private ErrorLevel CheckDuplicates(IEnumerable<FileInfo> files, Action<string> callbackError)
        {
            // Create comparer with desired configuration
            FileInfoComparerConfig fileInfoComparerConfig = new()
            {
                // Do not force case insensitive file name comparison by default
                CompareFileNameCaseInSensitive = false,

                // Only compare file names to identify duplicates
                CompareFileSize = false,
            };

            IFileInfoComparer fileInfoComparer = FileInfoComparerFactory.Create(fileInfoComparerConfig);

            // Check for duplicate file names in sub folders and parent folder with comparer from factory
            Dictionary<FileInfo, IEnumerable<FileInfo>> duplicates = SearchService.FindDuplicateFiles(files, fileInfoComparer, CancellationToken.None);

            if (duplicates.Count > 0)
            {
                callbackError("ERROR: Duplicate file names found!");

                // Log duplicate file names with their paths
                foreach (KeyValuePair<FileInfo, IEnumerable<FileInfo>> duplicate in duplicates)
                {
                    callbackError($"\nDuplicate {duplicate.Key.Name} :");
                    foreach (FileInfo file in duplicate.Value)
                    {
                        callbackError($"   -> found in {file.DirectoryName}");
                    }
                }

                return ErrorLevel.FileNamesNotUnique;
            }

            return ErrorLevel.Ok;
        }

        /// <summary>
        /// Clean up empty sub folders after ungrouping
        /// </summary>
        /// <param name="start">Start date/time for log output</param>
        /// <param name="paths">sub folders to clean when empty</param>
        /// <param name="callbackLog">Log message callback</param>
        /// <param name="callbackError">Error message callback</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        private ErrorLevel CleanUp(DateTime start, List<string> paths, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Collections cannot be modified while enumerating
            List<string> pathsCopy = new(paths);

            // Remove child paths (keep only top level paths below parent folder)
            foreach (string path in pathsCopy)
            {
                // No casing here, because all paths come from the same parent folder
                paths.RemoveAll(p => p != path && p.StartsWith(path + Path.DirectorySeparatorChar));
            }

            // Get all sub folder paths that are now empty (including sub folders of sub folders)
            List<string> emptyPaths = new();

            foreach (string path in paths.TakeWhile(c => !cancellationToken.IsCancellationRequested))
            {
                if (SearchService.TrySearchFiles(path, Constants.AllFilesExtension, true, out IEnumerable<FileInfo> found, cancellationToken))
                {
                    if (!found.Any())
                    {
                        emptyPaths.Add(path);
                    }
                }
                else
                {
                    callbackError($"ERROR: Searching empty paths failed for {path}!");
                    return ErrorLevel.SearchFailed;
                }
            }

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Ungroup canceled before deleting empty sub folders (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Canceled;
            }

            // Log number of empty sub folders found
            callbackLog($"{emptyPaths.Count} out of {paths.Count} sub folders found empty\n");

            // Delete empty sub folders
            int deleted = FileService.DeleteFolders(emptyPaths.ToArray(), callbackLog, callbackError, cancellationToken);
            callbackLog($"\n{deleted} empty sub folders deleted\n");

            // Check if all empty sub folders were deleted
            if (deleted != emptyPaths.Count)
            {
                callbackError($"Ungroup completed with errors (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.DeleteEmptySubFoldersFailed;
            }

            return ErrorLevel.Ok;
        }

        #endregion // Methods
    }
}