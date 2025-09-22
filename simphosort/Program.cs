// <copyright file="Program.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CommandDotNet;
using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Simphosort.Core.Services;
using Simphosort.Core.Utilities;

namespace Simphosort
{
    /// <summary>
    /// Program class
    /// </summary>
    [Command(Description = $"simphosort - Simple Photo Sorter - COPYRIGHT 2025 Alexander Beug - https://www.alexpage.de")]
    internal class Program
    {
        /// <inheritdoc cref="IServiceProvider"/>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program()
        {
            // Register services
            _provider = RegisterServices();
        }

        /// <summary>
        /// Copy command
        /// </summary>
        /// <param name="sourceFolder">source folder</param>
        /// <param name="targetFolder">target folder</param>
        /// <param name="checkFolders">check folders</param>
        /// <returns>An <see cref="ErrorLevel"/> value as int.</returns>
        [Command("copy", Description = "Copy new photos from source folder to target folder with optional checks")]
        public int Copy(
            [Operand("source", Description = "Source folder (containing the photo files to copy)"), PathReference] DirectoryInfo sourceFolder,
            [Operand("target", Description = "Target folder (work folder, has to be empty)"), PathReference] DirectoryInfo targetFolder,
            [Option('c', "check", Description = "Check for duplicate photos at these folders. Duplicate files will not be copied to target."), PathReference] DirectoryInfo[]? checkFolders)
            => _provider.GetRequiredService<IMainService>().CopyPhotos(sourceFolder.FullName, targetFolder.FullName, checkFolders?.Select(c => c.FullName), DisplayCallback, DisplayCallback).ToInt();

        /// <summary>
        /// Program main function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns><see cref="ErrorLevel"/> as int value</returns>
        private static int Main(string[] args)
        {
            // AppRunner<T> where T is the class defining your commands
            // You can use Program or create commands in another class
            return new AppRunner<Program>()
                .UseVersionMiddleware() // Adds a version option and command
                .UseTypoSuggestions() // Suggests correct command names if user makes a typo
                .Run(args);
        }

        /// <summary>
        /// Registers all services for DI
        /// </summary>
        /// <returns>Service provider</returns>
        private static IServiceProvider RegisterServices()
        {
            IServiceCollection container = new ServiceCollection();
            container = Simphosort.Core.Utilities.RegisterServices.Register(container);

            IServiceProvider provider = container.BuildServiceProvider();
            return provider;
        }

        /// <summary>
        /// Callback for displaying an error or log message
        /// </summary>
        /// <param name="errmsg">Error/log message to display</param>
        private static void DisplayCallback(string errmsg)
        {
            Console.WriteLine(errmsg);
        }
    }
}