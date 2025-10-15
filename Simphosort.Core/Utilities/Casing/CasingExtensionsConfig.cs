// <copyright file="CasingExtensionsConfig.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Utilities.Casing
{
    /// <summary>
    /// Casing extensions configuration
    /// </summary>
    public class CasingExtensionsConfig
    {
        #region Constructor

        /// <summary>
        /// Cache for folder casing detection
        /// </summary>
        private static readonly Dictionary<string, CasingExtensionsMode> FolderCasingCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CasingExtensionsConfig"/> class.
        /// </summary>
        public CasingExtensionsConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CasingExtensionsConfig"/> class with pre-set sensitivity.
        /// </summary>
        /// <param name="caseSensitive">CaseSensitive</param>
        public CasingExtensionsConfig(bool caseSensitive)
        {
            Mode = caseSensitive ? CasingExtensionsMode.CaseSensitive : CasingExtensionsMode.CaseInSensitive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CasingExtensionsConfig"/> class with FolderCasing based on reference folder.
        /// </summary>
        /// <param name="referenceFolder">Reference folder for casing detection</param>
        public CasingExtensionsConfig(string referenceFolder)
        {
            Mode = IsFolderCaseSensitive(referenceFolder, out bool fallback);
            IsFallback = fallback;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets a value indicating what casing mode to use
        /// </summary>
        public CasingExtensionsMode Mode { get; } = CasingExtensionsMode.CaseInSensitive;

        /// <summary>
        /// Gets a value indicating whether folder casing detection was not possible (e.g. due to permission issues)
        /// </summary>
        public bool IsFallback { get; } = false;

        #endregion // Properties

        #region Private methods

        /// <summary>
        /// Get folder casing based on config reference folder (with caching)
        /// </summary>
        /// <param name="referenceFolder">Reference folder</param>
        /// <param name="fallback">True when detection failed and fallback to case insensitive applied</param>
        /// <returns><see cref="CasingExtensionsMode"/></returns>
        private static CasingExtensionsMode IsFolderCaseSensitive(string referenceFolder, out bool fallback)
        {
            fallback = false;

            if (!FolderCasingCache.TryGetValue(referenceFolder, out CasingExtensionsMode casingExtensionsMode))
            {
                // Detect folder casing by creating a temporary file
                string tempFileName = Path.Combine(referenceFolder, $"simphosort_temp_{Guid.NewGuid()}.tmp");

                try
                {
                    // Create temporary file with lower case name
                    using (FileStream fs = File.Create(tempFileName))
                    {
                        // Just create and close
                    }

                    // Check if upper case version of the file exists
                    casingExtensionsMode = File.Exists(tempFileName.ToUpperInvariant()) ? CasingExtensionsMode.CaseInSensitive : CasingExtensionsMode.CaseSensitive;
                }
                catch
                {
                    // In case of an error assume case insensitive (more strict)
                    casingExtensionsMode = CasingExtensionsMode.CaseInSensitive;
                    fallback = true;
                }
                finally
                {
                    try
                    {
                        // Clean up temporary file
                        if (File.Exists(tempFileName))
                        {
                            File.Delete(tempFileName);
                        }
                    }
                    catch
                    {
                        // Ignore any error here
                    }
                }

                // Cache result for future use
                FolderCasingCache[referenceFolder] = casingExtensionsMode;
            }

            return casingExtensionsMode;
        }

        #endregion // Private methods
    }
}