// <copyright file="Duration.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Simple duration calculation
    /// </summary>
    internal static class Duration
    {
        /// <summary>
        /// Calculates the duration from start time to now and removes milliseconds and microseconds for better readability
        /// </summary>
        /// <param name="start">Start</param>
        /// <returns>Duration with seconds precision</returns>
        public static TimeSpan Calculate(DateTime start)
        {
            // Log operation duration and remove milliseconds and microseconds for better readability
            TimeSpan duration = DateTime.UtcNow - start;
            TimeSpan simpleDuration = new(duration.Days, duration.Hours, duration.Minutes, duration.Seconds);
            return simpleDuration;
        }
    }
}