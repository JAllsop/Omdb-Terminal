using System;
using System.Collections.Generic;
using System.Text;
using OmdbTerminal.Shared;

namespace OmdbTerminal.Cli.Services
{
    internal interface IApiClient
    {
        Task SearchAndDisplayAsync(string title, MediaType? type = null, string? year = null);

        Task SearchByImdbIdAndDisplayAsync(string id);

        Task SearchByTitleAndDisplayAsync(string title, MediaType? type = null, string? year = null);

        Task ClearCacheAndDisplayAsync();

        Task ManageCustomEntitiesAsync();
    }
}
