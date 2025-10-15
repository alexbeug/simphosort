// <copyright file="UngroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Helper;
using Simphosort.Core.Utilities;
using Simphosort.Core.Utilities.Casing;

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
        public UngroupService(IFolderService folderService, ISearchService searchService, IFileService fileService)
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
        public ErrorLevel Ungroup(string parent, bool clean, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            // Log operation start
            callbackLog($"Ungroup files");
            callbackLog($"   parent : {parent}");
            callbackLog($"   clean  : {clean}");
            callbackLog(string.Empty);

            // Start time
            DateTime start = DateTime.UtcNow;

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

            // Set casing to default (case insensitive) for folder and file operations
            CasingExtensionsConfig casing = new();

            // Get all files in sub folders (recursive) and parent folder
            callbackLog($"Searching files in parent folder and sub folders...");
            List<FileInfo> files = SearchService.SearchFiles(parent, Constants.SupportedExtensions, true, cancellationToken);

            // Separate files in parent folder and sub folders
            List<FileInfo> subFiles = files.Where(f => !f.DirectoryName!.Equals(parent, casing)).ToList();
            callbackLog($"   -> {subFiles.Count} files found in sub folders");

            List<FileInfo> parentFiles = files.Except(subFiles).ToList();
            callbackLog($"   -> {parentFiles.Count} files found in parent folder\n");

            // Break operation if cancellation requested
            if (cancellationToken.IsCancellationRequested)
            {
                callbackLog($"Ungroup canceled before ungrouping files\n");
                return ErrorLevel.Canceled;
            }

            // Check for duplicate file names in sub folders and parent folder
            // TODO: make this also a new command with exact file duplicates
            // TODO: Check with Exists for real existence?
            if (files.Select(f => f.Name.ToConfigCase(casing)).Distinct().Count() != files.Count)
            {
                callbackError("ERROR: Duplicate file names found!");

                // Log duplicate file names with their paths
                Dictionary<string, List<FileInfo>> duplicateFiles = files.GroupBy(f => f.Name.ToConfigCase(casing)).Where(g => g.Count() > 1).ToDictionary(g => g.Key, g => g.ToList());
                foreach (KeyValuePair<string, List<FileInfo>> duplicate in duplicateFiles)
                {
                    callbackError($"\nDuplicate {duplicate.Key} :");
                    foreach (FileInfo file in duplicate.Value)
                    {
                         callbackError($"   -> found in {file.DirectoryName}");
                    }
                }

                return ErrorLevel.FileNamesNotUnique;
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

                    // Collections cannot be modified while enumerating
                    List<string> pathsCopy = new(paths);

                    // Remove child paths (keep only top level paths below parent folder)
                    foreach (string path in pathsCopy)
                    {
                        // No casing here, because all paths come from the same parent folder
                        paths.RemoveAll(p => p != path && p.StartsWith(path + Path.DirectorySeparatorChar));
                    }

                    // Get all sub folder paths that are now empty (including sub folders of sub folders)
                    List<string> emptyPaths = paths.Where(s => SearchService.SearchFiles(s, Constants.AllFilesExtension, true, cancellationToken).Count == 0).ToList();

                    // Log number of empty sub folders found
                    callbackLog($"{emptyPaths.Count} out of {paths.Count} sub folders found empty\n");

                    // Break operation if cancellation requested
                    if (cancellationToken.IsCancellationRequested)
                    {
                        callbackLog($"Ungroup canceled before deleting empty sub folders (Duration: {Duration.Calculate(start):g})\n");
                        return ErrorLevel.Canceled;
                    }

                    // Delete empty sub folders
                    int deleted = FileService.DeleteFolders(emptyPaths.ToArray(), callbackLog, callbackError, cancellationToken);
                    callbackLog($"\n{deleted} empty sub folders deleted\n");

                    if (deleted != emptyPaths.Count)
                    {
                        callbackError($"Ungroup completed with errors (Duration: {Duration.Calculate(start):g})\n");
                        return ErrorLevel.DeleteEmptySubFoldersFailed;
                    }
                }

                callbackLog($"Ungroup completed successfully (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.Ok;
            }
            else
            {
                callbackError($"Ungroup completed with errors (Duration: {Duration.Calculate(start):g})\n");
                return ErrorLevel.UngroupFailed;
            }
        }

        #endregion // Methods
    }
}