// <copyright file="GroupService.cs" company="Alexander Beug">
// Copyright (c) Alexander Beug. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Simphosort.Core.Utilities;

namespace Simphosort.Core.Services
{
    /// <inheritdoc/>
    internal class GroupService : IGroupService
    {
        /// <inheritdoc/>
        public ErrorLevel GroupFileDate(string folder, string formatString, Action<string> callbackLog, Action<string> callbackError, CancellationToken cancellationToken)
        {
            return ErrorLevel.Ok;
        }
    }
}