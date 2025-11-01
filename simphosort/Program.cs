// <copyright file="Program.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Simphosort.Commands;
using Simphosort.Core.Utilities;

namespace Simphosort
{
    /// <summary>
    /// Program class
    /// </summary>
    [Command(Description = $"simphosort - Simple Photo Sorter - Copyright (c) 2025 Alexander Beug - https://www.alexpage.de", Usage = "simphosort [command] [options]")]
    internal class Program
    {
        /// <summary>
        /// Gets or sets copy command
        /// </summary>
        [Subcommand]
        public Copy? Copy { get; set; }

        /// <summary>
        /// Gets or sets group command
        /// </summary>
        [Subcommand]
        public Group? Group { get; set; }

        /// <summary>
        /// Gets or sets ungroup command
        /// </summary>
        [Subcommand]
        public Ungroup? Ungroup { get; set; }

        /// <summary>
        /// Program main function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns><see cref="ErrorLevel"/> as int value</returns>
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Program))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Copy))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Group))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Ungroup))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ErrorLevel))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DirectoryInfo))]
        private static int Main(string[] args)
        {
            // Register services for DI and all command classes
            IServiceProvider provider = RegisterServices();

            // Configure the app runner
            AppRunner appRunner = new AppRunner<Program>()
                .UseVersionMiddleware() // Adds a version option and command
                .UseTypoSuggestions() // Suggests correct command names if user makes a typo
                .UseCancellationHandlers() // Enables Ctrl+C cancellation
                .UseMicrosoftDependencyInjection(provider); // Use Microsoft.Extensions.DependencyInjection for DI

            // Run the app
            return appRunner.Run(args);
        }

        /// <summary>
        /// Registers all services for DI
        /// </summary>
        /// <param name="appRunner">The <see cref="AppRunner"/> instance</param>
        /// <returns>Service provider</returns>
        private static IServiceProvider RegisterServices()
        {
            IServiceCollection container = new ServiceCollection();

            // Register core services
            container = Simphosort.Core.Utilities.RegisterServices.Register(container);

            // Register command classes
            container.AddSingleton<Copy, Copy>();
            container.AddSingleton<Group, Group>();
            container.AddSingleton<Ungroup, Ungroup>();

            IServiceProvider provider = container.BuildServiceProvider();
            return provider;
        }
    }
}