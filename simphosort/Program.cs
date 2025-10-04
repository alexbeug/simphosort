// <copyright file="Program.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CliFx;

using Microsoft.Extensions.DependencyInjection;

using Simphosort.Core.Utilities;

namespace Simphosort
{
    /// <summary>
    /// Program class
    /// </summary>
    // [Command(Description = $"simphosort - Simple Photo Sorter - Copyright (c) 2025 Alexander Beug - https://www.alexpage.de")]
    internal static class Program
    {
        /// <summary>
        /// Program main function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns><see cref="ErrorLevel"/> as int value</returns>
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CopyCommand))]
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .SetExecutableName("simphosort")
                .UseTypeActivator(commandTypes =>
                {
                    IServiceCollection container = new ServiceCollection();
                    container = Simphosort.Core.Utilities.RegisterServices.Register(container);

                    // Register commands
                    foreach (Type commandType in commandTypes)
                    {
                        container.AddTransient(commandType);
                    }

                    return container.BuildServiceProvider();
                })                
                .Build()
                .RunAsync();
    }
}