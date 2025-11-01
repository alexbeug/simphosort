// <copyright file="Ungroup.cs" company="Alexander Beug">
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
    /// Ungroup command
    /// </summary>
    [Command("ungroup", Description = "Find files in sub folders and move them to parent folder", Usage = "simphosort ungroup [options] <parent>")]
    [Subcommand]
    public class Ungroup
    {
        /// <summary>
        /// Ungroup command
        /// </summary>
        /// <param name="parent">Folder</param>
        /// <param name="deleteEmpty">Delete empty sub folders</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="console">Console output</param>
        /// <param name="ungroupService">An <see cref="IUngroupService"/></param>
        /// <param name="ct">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ErrorLevel"/> value as int.</returns>
        [DefaultCommand]
        [Command("ungroup", Description = "Find files in sub folders and move them to parent folder")]
        public int UngroupMethod(
            [Operand("parent", Description = "Parent folder containing the sub folders to ungroup"), PathReference] DirectoryInfo parent,
            [Option('c', "clean", Description = "Delete empty sub folders containing no files after ungroup")] bool? deleteEmpty,
            [Option('s', "search", Description = "Specify custom search pattern using wildcards (e.g. *.jpg or IMG_*.nef)")] string[]? searchPatterns,
            IConsole console,
            IUngroupService ungroupService,
            CancellationToken ct)
            => ungroupService.Ungroup(parent.FullName, deleteEmpty ?? false, searchPatterns ?? Constants.CommonJpegExtensions, console.WriteLine, console.WriteLine, ct).ToInt();
    }
}