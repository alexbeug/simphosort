// <copyright file="SearchService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Comparer;
using Simphosort.Core.Values;

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class SearchService : ISearchService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="fileInfoComparerFactory">A <see cref="IPhotoFileInfoComparerFactory"/></param>
        public SearchService(IPhotoFileInfoComparerFactory fileInfoComparerFactory)
        {
            FileInfoComparerFactory = fileInfoComparerFactory;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the <see cref="IPhotoFileInfoComparerFactory"/>
        /// </summary>
        private IPhotoFileInfoComparerFactory FileInfoComparerFactory { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public bool TrySearchFiles(string folder, IEnumerable<string> searchPatterns, bool subfolders, out IEnumerable<IPhotoFileInfo> filesFound, CancellationToken cancellationToken)
        {
            // Always initialize out parameter
            filesFound = new List<IPhotoFileInfo>();

            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled, result ist true with empty list, Canceled state is handled by caller
                return true;
            }

            try
            {
                // Create DirectoryInfo object, may throw exception if folder is invalid
                DirectoryInfo directoryInfo = new(folder);

                // Get files with specified extensions
                foreach (string extension in searchPatterns.TakeWhile(e => !cancellationToken.IsCancellationRequested))
                {
                    // Enumerate files for each extension
                    IEnumerable<FileInfo> foundFiles = directoryInfo.EnumerateFiles(extension, subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                    // Add found files to result list
                    filesFound = filesFound.Concat(foundFiles.Select(x => new PhotoFileInfoWithDuplicates(x)));
                }

                return true;
            }
            catch (Exception)
            {
                // Return empty list on error
                return false;
            }
        }

        /// <inheritdoc/>
        public List<IPhotoFileInfo> ReduceFiles(IEnumerable<IPhotoFileInfo> workFiles, IEnumerable<IPhotoFileInfo> reduceFiles, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new List<IPhotoFileInfo>();
            }

            // Create comparer with desired configuration
            PhotoFileInfoComparerConfig fileInfoComparerConfig = new()
            {
                // Do not force case insensitive file name comparison by default
                CompareFileNameCaseInSensitive = false,

                // Always compare file size to indentify identical files
                CompareFileSize = true,
            };

            IPhotoFileInfoComparer fileInfoComparer = FileInfoComparerFactory.Create(fileInfoComparerConfig);

            List<IPhotoFileInfo> resultFiles = new();
            resultFiles.AddRange(workFiles.Where(w => !reduceFiles.TakeWhile(s => !cancellationToken.IsCancellationRequested).Contains(w, fileInfoComparer)));
            return resultFiles;
        }

        /// <inheritdoc/>
        public List<IPhotoFileInfoWithDuplicates> FindDuplicateFiles(IEnumerable<IPhotoFileInfo> files, IPhotoFileInfoComparer fileInfoComparer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new List<IPhotoFileInfoWithDuplicates>();
            }

            // Dictionary to hold duplicates
            Dictionary<string, IPhotoFileInfoWithDuplicates> duplicates = new();

            // Get each file to prevent multiple enumerations
            List<IPhotoFileInfo> allFiles = files.TakeWhile(s => !cancellationToken.IsCancellationRequested).ToList();

            int x = 0;

            foreach (IPhotoFileInfo file in allFiles.TakeWhile(s => !cancellationToken.IsCancellationRequested))
            {
                // TODO: Debug log - remove later
                x++;
                Console.WriteLine($"Checking file {x} - {file.FileInfo.FullName} for duplicates...");

                // Exclude files in the same directory and the file itself
                List<IPhotoFileInfo> testFiles = allFiles.Where(
                    f => !string.IsNullOrWhiteSpace(f.FileInfo.DirectoryName)
                    && !f.FileInfo.DirectoryName.Equals(file.FileInfo.DirectoryName)
                    && !f.FileInfo.FullName.Equals(file.FileInfo.FullName)).ToList();

                // Check test files for equalness
                foreach (FileInfo testFileFileInfo in testFiles.TakeWhile(t => !cancellationToken.IsCancellationRequested).Where(testFile => fileInfoComparer.Equals(file, testFile)).Select(x => x.FileInfo))
                {
                    if (duplicates.TryGetValue(file.FileInfo.Name, out IPhotoFileInfoWithDuplicates? existingDuplicate))
                    {
                        // Already in dictionary, add duplicate file
                        existingDuplicate.Duplicates.Add(testFileFileInfo);
                        continue;
                    }
                    else
                    {
                        // Not in dictionary, create new entry
                        new PhotoFileInfoWithDuplicates(file.FileInfo).Duplicates.Add(testFileFileInfo);
                        duplicates.Add(file.FileInfo.Name, new PhotoFileInfoWithDuplicates(file.FileInfo));
                    }
                }
            }

            return duplicates.Values.ToList();
        }

        #endregion // Methods
    }
}