// <copyright file="ISearchService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Services.Comparer;
using Simphosort.Core.Values;

namespace Simphosort.Core.Services.Helper
{
    /// <summary>
    /// File search service
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Try to search for files in a given folder
        /// </summary>
        /// <param name="folder">Folder to search</param>
        /// <param name="searchPatterns">Search patterns (*.jpg, *.jpeg)</param>
        /// <param name="subfolders">Include subfolders</param>
        /// <param name="filesFound">List of <see cref="FileInfo"/> objects with files found</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True when files searched, result may be zero</returns>
        bool TrySearchFiles(string folder, IEnumerable<string> searchPatterns, bool subfolders, out IEnumerable<IPhotoFileInfo> filesFound, CancellationToken cancellationToken);

        /// <summary>
        /// Reduce files in given workFiles
        /// </summary>
        /// <param name="workFiles">Work files to reduce</param>
        /// <param name="reduceFiles">Files for comparison</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Reduced work files</returns>
        List<IPhotoFileInfo> ReduceFiles(IEnumerable<IPhotoFileInfo> workFiles, IEnumerable<IPhotoFileInfo> reduceFiles, CancellationToken cancellationToken);

        /// <summary>
        /// Find duplicate files by using <see cref="IPhotoFileInfoEqualityComparer.Equals"/>
        /// </summary>
        /// <param name="files">Files to check for equals</param>
        /// <param name="fileInfoEqualityComparer"><see cref="IPhotoFileInfoEqualityComparer"/> to use</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A dictionary with duplicates files</returns>
        List<IPhotoFileInfoWithDuplicates> FindDuplicateFiles(IEnumerable<IPhotoFileInfo> files, IPhotoFileInfoEqualityComparer fileInfoEqualityComparer, CancellationToken cancellationToken);
    }
}