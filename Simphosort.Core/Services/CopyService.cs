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
        public int CopyFiles(IEnumerable<FileInfo> files, string sortFolder, Action<string> callbackLog, Action<string> callbackError)
        {
            int copied = 0;

            callbackLog($"\nCopying {files.Count()} new image files to {sortFolder}\n");

            foreach (FileInfo file in files)
            {
                callbackLog($"Copying {file.FullName}");

                try
                {
                    File.Copy(file.FullName, Path.Combine(sortFolder, file.Name));
                    callbackLog($"   -> copied");
                    copied++;
                }
                catch
                {
                    callbackError($"   -> failed");
                }
            }

            return copied;
        }
    }
}