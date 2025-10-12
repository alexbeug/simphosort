// <copyright file="ErrorLevel.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Error level for command line return
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>
        /// Ok
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Canceled
        /// </summary>
        Canceled = 1,

        /// <summary>
        /// One of the folders is not valid
        /// </summary>
        FolderNotValid = 100,

        /// <summary>
        /// One of the folders did not exist
        /// </summary>
        FolderDoesNotExist = 101,

        /// <summary>
        /// Sort folder is not empty
        /// </summary>
        FolderNotEmpty = 120,

        /// <summary>
        /// Sub folders are present in the given folder
        /// </summary>
        FoldersPresent = 121,

        /// <summary>
        /// Files are present in the given folder
        /// </summary>
        FilesPresent = 122,

        /// <summary>
        /// No sub folders are present in the given folder
        /// </summary>
        NoSubFolders = 130,

        /// <summary>
        /// Given folder names are not unique
        /// </summary>
        FolderNamesNotUnique = 150,

        /// <summary>
        /// Given file names are not unique (same file name in different sub folders)
        /// </summary>
        FileNamesNotUnique = 151,

        /// <summary>
        /// Format string is empty
        /// </summary>
        FormatStringEmpty = 400,

        /// <summary>
        /// Format string is not valid (for date)
        /// </summary>
        FormatStringNotValid = 401,

        /// <summary>
        /// Copy failed
        /// </summary>
        CopyFailed = 500,

        /// <summary>
        /// Group failed
        /// </summary>
        GroupFailed = 501,

        /// <summary>
        /// Ungroup failed
        /// </summary>
        UngroupFailed = 502,

        /// <summary>
        /// Deletion of emtpy sub folders after ungrouping failed
        /// </summary>
        DeleteEmptySubFoldersFailed = 503,
    }
}