using OmdbTerminal.Cli.Services;
using SimpleInjector;

namespace OmdbTerminal.Cli
{
    internal static class IoC
    {
        public static readonly Container Container;

        static IoC()
        {
            Container = new Container();

            Container.RegisterSingleton(() => new CachedEntriesHttpClient());
            Container.RegisterSingleton(() => new MoviesHttpClient());

            Container.Register<IApiClientLegacy, ApiClientLegacy>();
            Container.Register<IApiClient, ApiClient>();
            Container.Verify();
        }
    }
}
