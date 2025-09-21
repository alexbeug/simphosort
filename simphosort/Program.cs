// <copyright file="Program.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

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
        /// Sort command
        /// </summary>
        /// <param name="workFolder">working folder (source)</param>
        /// <param name="photoFolder">photo folder (compare)</param>
        /// <param name="sortFolder">sort folder (target)</param>
        /// <param name="junkFolder">junk folder (ignore)</param>
        /// <returns>An <see cref="ErrorLevel"/> value as int.</returns>
        [Command("sort", Description = "Sort new photos from work folder that to not exist in photo or junk folder to sort folder")]
        public int Sort(
            [Operand("work", Description = "Put the unsorted photos here"), PathReference] DirectoryInfo workFolder,
            [Operand("photo", Description = "Where your existing photos are saved"), PathReference] DirectoryInfo photoFolder,
            [Operand("sort", Description = "New photos are copied here"), PathReference] DirectoryInfo sortFolder,
            [Option('j', "junk", Description = "Treat photos from here as existing"), PathReference] DirectoryInfo? junkFolder)
            => _provider.GetRequiredService<IMainService>().SortPhotos(workFolder.FullName, photoFolder.FullName, sortFolder.FullName, junkFolder?.FullName ?? string.Empty, DisplayCallback, DisplayCallback).ToInt();

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