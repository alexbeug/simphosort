// <copyright file="FileService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Simphosort.Core.Services.Helper
{
    /// <inheritdoc/>
    internal class FileService : IFileService
    {
        /// <inheritdoc/>
        public int CopyFiles(IEnumerable<FileInfo> files, string targetFolder, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            int copied = 0;

            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if canceled
                return copied;
            }

            callbackLog($"\nCopying {files.Count()} new image files to {targetFolder}\n");

            foreach (FileInfo file in files.TakeWhile(f => !cancellationToken.IsCancellationRequested))
            {
                callbackLog($"Copying {file.FullName}");

                try
                {
                    File.Copy(file.FullName, Path.Combine(targetFolder, file.Name));
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