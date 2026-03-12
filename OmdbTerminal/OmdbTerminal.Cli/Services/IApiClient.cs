using System;
using System.Collections.Generic;
using System.Text;

namespace OmdbTerminal.Cli.Services
{
    internal interface IApiClient
    {
        Task SearchAndDisplayAsync(string title);

        Task SearchByImdbIdAndDisplayAsync(string id);

        Task ClearCacheAndDisplayAsync();

        Task ManageCustomEntitiesAsync();
    }
}
