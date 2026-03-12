using OmdbTerminal.Cli.Services;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmdbTerminal.Cli
{
    internal static class IoC
    {
        public static readonly Container Container;

        static IoC()
        {
            Container = new Container();

            Container.RegisterSingleton(() => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7254/")
            });

            Container.Register<IApiClient, ApiClient>();
            Container.Verify();
        }
    }
}
