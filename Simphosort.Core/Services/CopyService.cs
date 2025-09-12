// <copyright file="CopyService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class CopyService : ICopyService
    {
        /// <inheritdoc/>
        public bool CopyFiles(IEnumerable<FileInfo> files, string sortFolder, Action<string> callbackError)
        {
            bool copyOk = true;

            foreach (FileInfo file in files)
            {
                try
                {
                    File.Copy(file.FullName, Path.Combine(sortFolder, file.Name));
                }
                catch
                {
                    callbackError($"ERROR: Copying {file.FullName} to {sortFolder} failed!");
                    copyOk = false;
                }
            }

            return copyOk;
        }
    }
}