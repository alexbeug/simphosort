// <copyright file="Program.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using Simphosort.Core.Services;
using Simphosort.Core.Utilities;

namespace Simphosort
{
    /// <summary>
    /// Program class
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Program main function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns><see cref="ErrorLevel"/> as int value</returns>
        private static int Main(string[] args)
        {
            // Register services and get the 2 needed services for Main function
            IServiceProvider provider = RegisterServices();
            IMainService mainService = provider.GetRequiredService<IMainService>();

            // Display program title
            DisplayTitle();

            if (args.Length > 2 && args.Length < 5)
            {
                // Call sorting with arguments (3 or 4)
                return mainService.SortPhotos(args[0], args[1], args[2], args.Length > 3 ? args[3] : string.Empty, DisplayCallback, DisplayCallback).ToInt();
            }
            else
            {
                if (args.Length > 0)
                {
                    // Invalid number of arguments (fewer than 3 or more than 4)
                    DisplayArgsError();

                    // Display usage for information
                    DisplayUsage();

                    // Return error
                    return ErrorLevel.ArgumentsIncorrent.ToInt();
                }

                // Usage without arguments or when called incorrectly
                DisplayUsage();

                // Return error
                return ErrorLevel.ArgumentsUsage.ToInt();
            }
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
        /// Display title and copyright
        /// </summary>
        private static void DisplayTitle()
        {
            Console.WriteLine("simphosort - Simple Photo Sorter");
            Console.WriteLine("COPYRIGHT 2025 Alexander Beug");
            Console.WriteLine("https://www.alexpage.de");
            Console.WriteLine();
        }

        /// <summary>
        /// Display program usage
        /// </summary>
        private static void DisplayUsage()
        {
            Console.WriteLine("simphosort [work folder] [photo folder] [sort folder] [junk folder]");
            Console.WriteLine();
            Console.WriteLine("[work folder]    put the unsorted photos here");
            Console.WriteLine("[photo folder]   where your existing photos are saved");
            Console.WriteLine("[sort folder]    new photos are moved here after check");
            Console.WriteLine("[junk folder]    put junk photos here to ignore then in the next sort (optional)");
            Console.WriteLine();
        }

        /// <summary>
        /// Display args error
        /// </summary>
        private static void DisplayArgsError()
        {
            Console.WriteLine("ERROR: Invalid number of arguments!");
            Console.WriteLine();
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