// <copyright file="CasingExtensions.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities.Casing
{
    /// <summary>
    /// String casing extensions
    /// </summary>
    internal static class CasingExtensions
    {
        /// <summary>
        /// Transform a string to configured casing
        /// </summary>
        /// <param name="str">String to transform</param>
        /// <param name="config">Casing config</param>
        /// <returns>Transformed string</returns>
        public static string ToConfigCase(this string str, CasingExtensionsConfig config)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (config.Mode == CasingExtensionsMode.CaseSensitive)
            {
                // Case sensitive - return unmodified
                return str;
            }

            if (config.Mode == CasingExtensionsMode.CaseInSensitive)
            {
                // Case insensitive - return lower invariant
                return str.ToLowerInvariant();
            }

            // Invalid casing mode
            throw new InvalidOperationException("Invalid casing mode");
        }

        /// <summary>
        /// Compare two strings with configured casing
        /// </summary>
        /// <param name="str">This string</param>
        /// <param name="other">Other string</param>
        /// <param name="config">Casing config</param>
        /// <returns>True when equal</returns>
        public static bool Equals(this string str, string other, CasingExtensionsConfig config)
        {
            if (config.Mode == CasingExtensionsMode.CaseInSensitive)
            {
                // Case insensitive
                return string.Equals(str, other, StringComparison.OrdinalIgnoreCase);
            }

            if (config.Mode == CasingExtensionsMode.CaseSensitive)
            {
                // Case sensitive
                return string.Equals(str, other, StringComparison.Ordinal);
            }

            // Invalid casing mode
            throw new InvalidOperationException("Invalid casing mode");
        }
    }
}