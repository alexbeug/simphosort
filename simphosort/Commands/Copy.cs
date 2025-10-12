// <copyright file="Copy.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CommandDotNet;
using JetBrains.Annotations;
using Simphosort.Core.Services;
using Simphosort.Core.Utilities;

namespace Simphosort.Commands
{
    /// <summary>
    /// Copy command
    /// </summary>
    [Command("copy", Description = "Copy new photos from source folder to target folder with optional checks", Usage = "simphosort copy [options] <source> <target>")]
    [Subcommand]
    public class Copy
    {
        /// <summary>
        /// Copy command
        /// </summary>
        /// <param name="sourceFolder">Source folder</param>
        /// <param name="targetFolder">Target folder</param>
        /// <param name="checkFolders">Check folders</param>
        /// <param name="console">Console output</param>
        /// <param name="copyService">A <see cref="ICopyService"/></param>
        /// <param name="ct">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ErrorLevel"/> value as int.</returns>
        [DefaultCommand]
        [Command("copy", Description = "Copy new photos from source folder to target folder with optional checks")]
        public int CopyMethod(
            [Operand("source", Description = "Source folder (containing the photo files to copy)"), PathReference] DirectoryInfo sourceFolder,
            [Operand("target", Description = "Target folder (has to be empty)"), PathReference] DirectoryInfo targetFolder,
            [Option('c', "check", Description = "Check for duplicate photos at these folders. Duplicate files will not be copied to target."), PathReference] DirectoryInfo[]? checkFolders,
            IConsole console,
            ICopyService copyService,
            CancellationToken ct)
            => copyService.Copy(sourceFolder.FullName, targetFolder.FullName, checkFolders?.Select(c => c.FullName), console.WriteLine, console.WriteLine, ct).ToInt();
    }
}