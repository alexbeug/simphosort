// <copyright file="SearchService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class SearchService : ISearchService
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="fileInfoComparer">A <see cref="IFileInfoComparer"/></param>
        public SearchService(IFileInfoComparer fileInfoComparer)
        {
            FileInfoComparer = fileInfoComparer;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the <see cref="IFileInfoComparer"/>
        /// </summary>
        private IFileInfoComparer FileInfoComparer { get; }

        #endregion // Properties

        #region Methods

        /// <inheritdoc/>
        public List<FileInfo> SearchFiles(string folder, string[] extensions, bool subfolders, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new List<FileInfo>();
            }

            DirectoryInfo directoryInfo = new(folder);
            return extensions.SelectMany(x => directoryInfo.GetFiles(x.ToLower(), subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).TakeWhile(s => !cancellationToken.IsCancellationRequested).ToList();
        }

        /// <inheritdoc/>
        public List<FileInfo> ReduceFiles(IEnumerable<FileInfo> workFiles, IEnumerable<FileInfo> reduceFiles, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if cancelled
                return new List<FileInfo>();
            }

            List<FileInfo> reducedFiles = new();
            reducedFiles.AddRange(workFiles.Where(w => !reduceFiles.Contains(w, FileInfoComparer)).TakeWhile(s => !cancellationToken.IsCancellationRequested));
            return reducedFiles;
        }

        #endregion // Methods
    }
}