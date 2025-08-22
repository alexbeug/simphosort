// <copyright file="ErrorLevelExtensions.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Extension method for <see cref="ErrorLevel"/> enum
    /// </summary>
    public static class ErrorLevelExtensions
    {
        /// <summary>
        /// Cast <see cref="ErrorLevel"/> to int
        /// </summary>
        /// <param name="errorLevel">An <see cref="ErrorLevel"/></param>
        /// <returns><see cref="ErrorLevel"/> casted to int</returns>
        public static int ToInt(this ErrorLevel errorLevel)
        {
            return (int)errorLevel;
        }
    }
}