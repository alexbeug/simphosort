// <copyright file="CopyCommand.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Simphosort.Core.Services;

namespace Simphosort
{
    /// <summary>
    /// Copy command
    /// </summary>
    [Command("copy", Description = "Copy new photos from source folder to target folder with optional checks")]
    public class CopyCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCommand"/> class.
        /// </summary>
        /// <param name="copyService">A <see cref="ICopyService"/></param>
        public CopyCommand(ICopyService copyService)
        {
            CopyService = copyService;
        }

        /// <summary>
        /// Gets source folder
        /// </summary>
        [CommandParameter(0, Name = "source", Description = "Source folder (containing the photo files to copy)", IsRequired = true)]
        public required DirectoryInfo SourceFolder { get; init; }

        /// <summary>
        /// Gets target folder
        /// </summary>
        [CommandParameter(1, Name = "target", Description = "Target folder (work folder, has to be empty)", IsRequired = true)]
        public required DirectoryInfo TargetFolder { get; init; }

        /// <summary>
        /// Gets check folders
        /// </summary>
        [CommandOption("check", 'c', Description = "Check for duplicate photos at these folders. Duplicate files will not be copied to target.", IsRequired = false)]
        public DirectoryInfo[]? CheckFolders { get; init; }

        /// <inheritdoc cref="ICopyService"/>
        private ICopyService CopyService { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="console">Console</param>
        /// <returns>Awaitable task value</returns>
        public ValueTask ExecuteAsync(IConsole console)
        {
            // Make the command cancellation-aware
            CancellationToken cancellationToken = console.RegisterCancellationHandler();

            // Call the copy service method
            CopyService.Copy(SourceFolder.FullName, TargetFolder.FullName, CheckFolders?.Select(c => c.FullName), console.Output.WriteLine, console.Error.WriteLine, cancellationToken);

            // If the execution is not meant to be asynchronous,
            // return an empty task at the end of the method.
            return default;
        }
    }
}