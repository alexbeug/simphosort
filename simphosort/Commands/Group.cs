// <copyright file="Group.cs" company="Alexander Beug">
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
    /// Group command
    /// </summary>
    [Command("group", Description = "Group files in folder and move them to sub folders", Usage = "simphosort group [command]")]
    [Subcommand]
    public class Group
    {
        /// <summary>
        /// Group files by formatted fixed date
        /// </summary>
        /// <param name="folder">Folder to group to sub folders</param>
        /// <param name="formatString">Format string</param>
        /// <param name="console">Console output</param>
        /// <param name="groupService">A <see cref="IGroupService"/></param>
        /// <param name="ct"><see cref="CancellationToken"/></param>
        /// <returns><see cref="ErrorLevel"/></returns>
        [Command("fixed", Description = "Group files by fixed date", Usage = "simphosort group fixed [options] <folder>")]
        public int FixedMethod(
            [Operand("folder", Description = "Folder containing the photo files to group"), PathReference] DirectoryInfo folder,
            [Option('f', "format", Description = "Format string (e.g. yyyy-MM-dd for daily sub folders)")] string formatString,
            IConsole console,
            IGroupService groupService,
            CancellationToken ct)
        {
            return groupService.GroupFixed(folder.FullName, formatString, console.WriteLine, console.WriteLine, ct).ToInt();
        }
    }
}