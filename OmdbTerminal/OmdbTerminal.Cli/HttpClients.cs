namespace OmdbTerminal.Cli
{
    internal class CachedEntriesHttpClient : HttpClient
    {
        public CachedEntriesHttpClient()
        {
            BaseAddress = new Uri("https://localhost:7254/CachedEntries/");
        }
    }
    internal class MoviesHttpClient : HttpClient
    {
        public MoviesHttpClient()
        {
            BaseAddress = new Uri("https://localhost:7254/movies/");
        }
    }

}
