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
        /// Only usage shown
        /// </summary>
        ArgumentsUsage = 1,

        /// <summary>
        /// Arguments not correct
        /// </summary>
        ArgumentsIncorrent = 2,

        /// <summary>
        /// One of the folders form parameters did not exist
        /// </summary>
        FolderDoesNotExist = 100,

        /// <summary>
        /// Given folders are not unique
        /// </summary>
        FoldersAreNotUnique = 101,
    }
}