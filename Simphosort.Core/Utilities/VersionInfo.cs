// <copyright file="VersionInfo.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Reflection;

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Provide version and copyright information
    /// </summary>
    public static class VersionInfo
    {
        /// <summary>
        /// URL to author's page
        /// </summary>
        private const string AuthorUrl = "www.alexpage.de";

        /// <summary>
        /// Return version  and copyright information
        /// </summary>
        /// <returns>version and copyright information</returns>
        public static string GetVersionString()
        {
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            string name = assembly.GetName().Name ?? string.Empty;
            string version = assembly.GetName().Version?.ToString() ?? string.Empty;
            string copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;
            return $"{name} {version} - {copyright} - {AuthorUrl}";
        }
    }
}