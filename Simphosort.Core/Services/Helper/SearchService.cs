// <copyright file="SearchService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Comparer;

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class SearchService : ISearchService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="fileInfoComparerFactory">A <see cref="IFileInfoComparerFactory"/></param>
        public SearchService(IFileInfoComparerFactory fileInfoComparerFactory)
        {
            FileInfoComparerFactory = fileInfoComparerFactory;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the <see cref="IFileInfoComparerFactory"/>
        /// </summary>
        private IFileInfoComparerFactory FileInfoComparerFactory { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public bool TrySearchFiles(string folder, string[] extensions, bool subfolders, out IEnumerable<FileInfo> filesFound, CancellationToken cancellationToken)
        {
            // Always initialize out parameter
            filesFound = new List<FileInfo>();

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
                filesFound = extensions.SelectMany(x => directoryInfo.GetFiles(x, subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).TakeWhile(s => !cancellationToken.IsCancellationRequested).ToList();
                return true;
            }
            catch (Exception)
            {
                // Return empty list on error
                return false;
            }
        }

        /// <inheritdoc/>
        public List<FileInfo> ReduceFiles(IEnumerable<FileInfo> workFiles, IEnumerable<FileInfo> reduceFiles, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new List<FileInfo>();
            }

            // Create comparer with desired configuration
            FileInfoComparerConfig fileInfoComparerConfig = new()
            {
                // Do not force case insensitive file name comparison by default
                CompareFileNameCaseInSensitive = false,

                // Always compare file size to indentify identical files
                CompareFileSize = true,
            };

            IFileInfoComparer fileInfoComparer = FileInfoComparerFactory.Create(fileInfoComparerConfig);

            List<FileInfo> reducedFiles = new();
            reducedFiles.AddRange(workFiles.Where(w => !reduceFiles.Contains(w, fileInfoComparer)).TakeWhile(s => !cancellationToken.IsCancellationRequested));
            return reducedFiles;
        }

        /// <inheritdoc/>
        public Dictionary<FileInfo, IEnumerable<FileInfo>> FindDuplicateFiles(IEnumerable<FileInfo> files, IFileInfoComparer fileInfoComparer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new Dictionary<FileInfo, IEnumerable<FileInfo>>();
            }

            // Dictionary to hold duplicates
            Dictionary<FileInfo, IEnumerable<FileInfo>> duplicates = new();

            foreach (FileInfo file in files.TakeWhile(s => !cancellationToken.IsCancellationRequested))
            {
                uint appearanceCount = 0;
                List<FileInfo> equalFiles = new();

                // Exclude files in the same directory and the file itself
                List<FileInfo> testFiles = files.Where(
                    f => !string.IsNullOrWhiteSpace(f.DirectoryName)
                    && !f.DirectoryName.Equals(file.DirectoryName)
                    && !f.FullName.Equals(file.FullName)).ToList();

                // Check test files for equalness
                foreach (var testFile in testFiles.TakeWhile(t => !cancellationToken.IsCancellationRequested).Where(testFile => fileInfoComparer.Equals(file, testFile)))
                {
                    // Count appearances and collect equal files
                    appearanceCount++;

                    // Add to dictionary, when first duplicate found, including the original file
                    if (appearanceCount == 1)
                    {
                        equalFiles.Add(file);

                        // Check if already in dictionary (only exact file name match)
                        if (duplicates.Any(d => d.Key.Name.Equals(file.Name)))
                        {
                            // Already in dictionary, skip adding
                            continue;
                        }

                        duplicates.Add(file, equalFiles);
                    }

                    // Add duplicate file to list
                    equalFiles.Add(testFile);
                }
            }

            return duplicates;
        }

        #endregion // Methods
    }
}