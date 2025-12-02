// <copyright file="FileOrder.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Enums
{
    /// <summary>
    /// File order options
    /// </summary>
    public enum FileOrder
    {
        /// <summary>
        /// No specific order
        /// </summary>
        None = 0,

        /// <summary>
        /// Order by full file name
        /// </summary>
        FullFileName = 100,

        /// <summary>
        /// Order by full file name descending
        /// </summary>
        FullFileNameDesc = 101,

        /// <summary>
        /// Order by full file name (invariant lowercase)
        /// </summary>
        FullFileNameLowerInvariant = 110,

        /// <summary>
        /// Order by full file name (invariant lowercase)
        /// </summary>
        FullFileNameLowerInvariantDesc = 111,

        /// <summary>
        /// File size
        /// </summary>
        Size = 200,

        /// <summary>
        /// File size descending
        /// </summary>
        SizeDesc = 201,

        /// <summary>
        /// File created date/time
        /// </summary>
        Created = 300,

        /// <summary>
        /// File created date/time descending
        /// </summary>
        CreatedDesc = 301,

        /// <summary>
        /// File modified date/time
        /// </summary>
        Modified = 310,

        /// <summary>
        /// File modified date/time descending
        /// </summary>
        ModifiedDesc = 311,

        /// <summary>
        /// File accessed date/time
        /// </summary>
        Accessed = 320,

        /// <summary>
        /// File accessed date/time descending
        /// </summary>
        AccessedDesc = 321,
    }
}