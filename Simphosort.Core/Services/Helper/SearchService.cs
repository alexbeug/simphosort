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
        #region Fields

        /// <inheritdoc cref="IPhotoFileInfoComparerFactory"/>
        private readonly IPhotoFileInfoComparerFactory _photoFileInfoComparerFactory;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="photoFileInfoComparerFactory">A <see cref="IPhotoFileInfoComparerFactory"/></param>
        public SearchService(IPhotoFileInfoComparerFactory photoFileInfoComparerFactory)
        {
            _photoFileInfoComparerFactory = photoFileInfoComparerFactory;
        }

        #endregion // Constructor

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
            PhotoFileInfoEqualityComparerConfig fileInfoEqualityComparerConfig = new()
            {
                // Compare by file name by default
                CompareFileName = true,

                // Do not force case insensitive file name comparison by default
                CompareFileNameCaseInSensitive = false,

                // Always compare file size to identify identical files
                CompareFileSize = true,
            };

            IPhotoFileInfoEqualityComparer photoFileInfoEqualityComparer = _photoFileInfoComparerFactory.CreateEqualityComparer(fileInfoEqualityComparerConfig);

            List<IPhotoFileInfo> resultFiles = new();
            resultFiles.AddRange(workFiles.Where(w => !reduceFiles.TakeWhile(s => !cancellationToken.IsCancellationRequested).Contains(w, photoFileInfoEqualityComparer)));
            return resultFiles;
        }

        /// <inheritdoc/>
        public List<IPhotoFileInfoWithDuplicates> FindDuplicateFiles(IEnumerable<IPhotoFileInfo> files, IPhotoFileInfoEqualityComparer fileInfoEqualityComparer, CancellationToken cancellationToken)
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

            // Get file lengths for optimization, if compare by size is configured
            if (fileInfoEqualityComparer.IsCompareFileSizeConfigured)
            {
                // Group files by length
                Dictionary<long, List<IPhotoFileInfo>> filesByLength = allFiles.GroupBy(f => f.FileInfo.Length).ToDictionary(g => g.Key, g => g.ToList());

                // Only keep file groups with more than one file and flatten to single list
                allFiles = filesByLength.Values.Where(v => v.Count > 1).SelectMany(l => l).ToList();
            }

            foreach (IPhotoFileInfo file in allFiles.TakeWhile(s => !cancellationToken.IsCancellationRequested))
            {
                // Exclude files in the same directory and the file itself
                // TODO: Check same folder exclusion based on configured options for finding duplicates in same folder
                List<IPhotoFileInfo> testFiles = allFiles.Where(
                    f => !string.IsNullOrWhiteSpace(f.FileInfo.DirectoryName)
                    && !f.FileInfo.DirectoryName.Equals(file.FileInfo.DirectoryName)
                    && !f.FileInfo.FullName.Equals(file.FileInfo.FullName)).ToList();

                // Check test files for equalness
                foreach (FileInfo testFileFileInfo in testFiles.TakeWhile(t => !cancellationToken.IsCancellationRequested).Where(testFile => fileInfoEqualityComparer.Equals(file, testFile)).Select(x => x.FileInfo))
                {
                    if (duplicates.TryGetValue(file.FileInfo.FullName, out IPhotoFileInfoWithDuplicates? existingDuplicate))
                    {
                        // Already in dictionary, add duplicate file
                        existingDuplicate.Duplicates.Add(testFileFileInfo);
                        continue;
                    }
                    else
                    {
                        // Not in dictionary, create new entry
                        PhotoFileInfoWithDuplicates newDuplicate = new(file.FileInfo);
                        newDuplicate.Duplicates.Add(testFileFileInfo);
                        duplicates.Add(file.FileInfo.FullName, newDuplicate);
                    }
                }
            }

            return duplicates.Values.ToList();
        }

        #endregion // Methods
    }
}