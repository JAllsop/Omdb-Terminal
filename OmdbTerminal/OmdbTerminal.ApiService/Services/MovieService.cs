using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services
{
    public class MovieService(IOmdbClient omdbClient, ICachedEntriesService cacheService, ILogger<MovieService> logger) : IMovieService
    {
        public async Task<OmdbSearchResponse> SearchAsync(string title, int page = 1)
        {
            var searchCache = await cacheService.GetSearchCacheAsync(title, page);
            if (searchCache != null)
            {
                logger.LogInformation("Search Cache HIT for {Query} page {Page}", title, page);

                var dbMovies = await cacheService.GetByIdsAsync(searchCache.MovieIds);
                var cachedMovies = dbMovies.Select(m => m.ToSearchResult()).ToList();

                return new OmdbSearchResponse
                {
                    Response = true,
                    TotalResults = searchCache.TotalResults,
                    Results = cachedMovies
                };
            }

            logger.LogInformation("Search Cache MISS for {Query} page {Page}. Fetching from OMDB...", title, page);
            var response = await omdbClient.SearchMoviesAsync(title, page);

            if (response.Response && response.Results != null)
            {
                var entitiesToCache = response.Results.Select(r => r.ToEntity()).ToList();

                var newSearchCache = new SearchCacheEntity
                {
                    Query = title,
                    Page = page,
                    TotalResults = response.TotalResults,
                    MovieIds = [.. response.Results.Select(r => r.ImdbId)],
                    CachedAt = DateTime.UtcNow
                };

                await cacheService.SaveSearchCacheAsync(newSearchCache, entitiesToCache);
            }

            return response;
        }

        public async Task<MovieDetails?> GetDetailsByIdAsync(string imdbId)
        {
            var cached = await cacheService.GetByIdAsync(imdbId);

            if (cached != null && cached.IsDetailed)
            {
                logger.LogInformation("Cache HIT for detailed {Id}", imdbId);
                return cached.ToDetails();
            }

            logger.LogInformation("Detail Cache MISS (or partial) for {Id} - Fetching from OMDB...", imdbId);
            var details = await omdbClient.GetMovieDetailsByIdAsync(imdbId);

            if (!details.Response) return null;

            var entity = details.ToEntity();

            if (cached != null)
            {
                await cacheService.UpdateAsync(imdbId, entity);
            }
            else
            {
                await cacheService.CreateAsync(entity);
            }

            return details;
        }

        public async Task<MovieDetails?> GetDetailsByTitleAsync(string title)
        {
            // MySQL is case-insensitive by default direct comparison works fine
            var cached = await cacheService.GetQueryable().FirstOrDefaultAsync(m => m.Title == title);
            if (cached != null && cached.IsDetailed)
            {
                logger.LogInformation("Cache HIT for detailed Title {Title}", title);
                return cached.ToDetails();
            }

            logger.LogInformation("Detail Cache MISS (or partial) for Title {Title} - Fetching from OMDB...", title);

            var details = await omdbClient.GetMovieDetailsByTitleAsync(title);

            if (!details.Response) return null;

            var entity = details.ToEntity();

            var existing = await cacheService.GetByIdAsync(entity.Id);
            if (existing != null)
            {
                await cacheService.UpdateAsync(entity.Id, entity);
            }
            else
            {
                await cacheService.CreateAsync(entity);
            }

            return details;
        }
    }
}
