// <copyright file="List.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CommandDotNet;
using JetBrains.Annotations;
using Simphosort.Core.Enums;
using Simphosort.Core.Services;
using Simphosort.Core.Utilities;

namespace Simphosort.Commands
{
    /// <summary>
    /// List command
    /// </summary>
    [Command("list", Description = "List photos in a folder and its sub folders with optional details", Usage = "simphosort list [options] <folder>")]
    [Subcommand]
    public class List
    {
        /// <summary>
        /// List photos in a folder and its sub folders with optional details
        /// </summary>
        /// <param name="folder">Folder to search for photo files</param>
        /// <param name="fileDetails">List file details</param>
        /// <param name="onlyDuplicates">List only files with duplicates</param>
        /// <param name="order">Order files by</param>
        /// <param name="searchPatterns">Search patterns</param>
        /// <param name="console">Console output</param>
        /// <param name="listService">A <see cref="IGroupService"/></param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        [DefaultCommand]
        [Command("list", Description = "List photos in a folder and its sub folders with optional details", Usage = "simphosort list [options] <folder>")]
        public int ListMethod(
            [Operand("folder", Description = "Folder containing the photo files to list"), PathReference] DirectoryInfo folder,
            [Option('f', "file-details", Description = "List file details")] bool? fileDetails,
            [Option('d', "duplicates", Description = "List only files with duplicates")] bool? onlyDuplicates,
            [Option('o', "order", Description = "Order files by")] FileOrder[]? order,
            [Option('s', "search", Description = "Specify custom search pattern using wildcards (e.g. *.jpg or IMG_*.nef)")] string[]? searchPatterns,
            IConsole console,
            IListService listService,
            CancellationToken ct)
        {
            return listService.List(folder.FullName, fileDetails ?? false, onlyDuplicates ?? false, order?.Length > 0 ? order : new List<FileOrder> { FileOrder.None }, searchPatterns ?? Constants.CommonJpegExtensions, console.WriteLine, console.WriteLine, ct).ToInt();
        }
    }
}