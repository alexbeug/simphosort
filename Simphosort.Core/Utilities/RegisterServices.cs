// <copyright file="RegisterServices.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using Simphosort.Core.Services;
using Simphosort.Core.Services.Helper;

namespace Simphosort.Core.Utilities
{
    /// <summary>
    /// Register Services for DI
    /// </summary>
    public static class RegisterServices
    {
        /// <summary>
        /// Registration of UsbImageTool.Misc.Logging services added to service collection.
        /// </summary>
        /// <param name="services">Servicecollection</param>
        /// <returns>extended Servicecollection</returns>
        public static IServiceCollection Register(IServiceCollection services)
        {
            // Add singleton services (stateless)
            services.AddSingleton<IMainService, MainService>();

            services.AddSingleton<IFileInfoComparer, FileInfoComparer>();
            services.AddSingleton<IFolderService, FolderService>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<ICopyService, CopyService>();

            return services;
        }
    }
}