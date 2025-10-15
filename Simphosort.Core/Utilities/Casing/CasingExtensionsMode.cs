// <copyright file="CasingExtensionsMode.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities.Casing
{
    /// <summary>
    /// Casing extensions mode
    /// </summary>
    public enum CasingExtensionsMode
    {
        /// <summary>
        /// Insensitive lower case as default (Windows default)
        /// </summary>
        CaseInSensitive = 0,

        /// <summary>
        /// Case sensitive mode, do not change casing (Linux default)
        /// </summary>
        CaseSensitive = 1,
    }
}