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
        /// Given folders are not unique
        /// </summary>
        FoldersAreNotUnique = 150,

        /// <summary>
        /// Copy failed
        /// </summary>
        CopyFailed = 500,
    }
}