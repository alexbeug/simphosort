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

            callbackLog($"Copying {files.Count()} new files to {targetFolder}\n");

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

        /// <inheritdoc/>
        public int MoveGroupedFilesToSubFolders(Dictionary<string, List<FileInfo>> groupedFiles, string folder, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            int moved = 0;

            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if canceled
                return moved;
            }

            callbackLog($"Moving {groupedFiles.Sum(g => g.Value.Count)} files to {groupedFiles.Count} sub folders in {folder}\n");

            foreach (KeyValuePair<string, List<FileInfo>> group in groupedFiles.TakeWhile(g => !cancellationToken.IsCancellationRequested))
            {
                // Create sub folder
                string subFolder = Path.Combine(folder, group.Key);

                try
                {
                    Directory.CreateDirectory(subFolder);
                }
                catch
                {
                    callbackError($"ERROR: Could not create folder {subFolder}!");
                    continue;
                }

                // Move files to sub folder
                foreach (FileInfo file in group.Value.TakeWhile(f => !cancellationToken.IsCancellationRequested))
                {
                    callbackLog($"Moving {file.FullName} to {subFolder}");
                    try
                    {
                        File.Move(file.FullName, Path.Combine(subFolder, file.Name));
                        callbackLog($"   -> moved");
                        moved++;
                    }
                    catch
                    {
                        callbackError($"   -> failed");
                    }
                }
            }

            return moved;
        }

        /// <inheritdoc/>
        public int MoveFilesFromSubFoldersToFolder(IEnumerable<FileInfo> files, string folder, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            int moved = 0;

            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if canceled
                return moved;
            }

            callbackLog($"Moving {files.Count()} files to {folder}\n");

            foreach (FileInfo file in files.TakeWhile(f => !cancellationToken.IsCancellationRequested))
            {
                callbackLog($"Moving {file.FullName} to {folder}");
                try
                {
                    File.Move(file.FullName, Path.Combine(folder, file.Name));
                    callbackLog($"   -> moved");
                    moved++;
                }
                catch
                {
                    callbackError($"   -> failed");
                }
            }

            return moved;
        }

        /// <inheritdoc/>
        public int DeleteFolders(string[] folders, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            int deleted = 0;

            if (cancellationToken.IsCancellationRequested)
            {
                // Return directly if canceled
                return deleted;
            }

            callbackLog($"Deleting {folders.Length} folders\n");

            foreach (string folder in folders.TakeWhile(f => !cancellationToken.IsCancellationRequested))
            {
                callbackLog($"Deleting folder {folder}");

                try
                {
                    Directory.Delete(folder, true);
                    callbackLog($"   -> deleted");
                    deleted++;
                }
                catch
                {
                    callbackError($"   -> failed");
                }
            }

            return deleted;
        }
    }
}