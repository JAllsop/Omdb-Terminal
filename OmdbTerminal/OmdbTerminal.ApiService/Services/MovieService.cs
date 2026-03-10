using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services
{
    public class MovieService(IOmdbClient omdbClient, OmdbDbContext dbContext, ILogger<MovieService> logger) : IMovieService
    {
        public async Task<OmdbSearchResponse> SearchAsync(string title, int page = 1) => await omdbClient.SearchMoviesAsync(title, page); // Search is live, so we go straight to the client

        public async Task<MovieEntity?> GetDetailsAsync(string imdbId)
        {
            var cached = await dbContext.CachedMovies.FirstOrDefaultAsync(m => m.ImdbId == imdbId);

            if (cached != null)
            {
                logger.LogInformation("Cache HIT for {Id}", imdbId);
                return cached;
            }

            logger.LogInformation("Cache MISS for {Id}. Fetching from OMDB...", imdbId);
            var details = await omdbClient.GetMovieDetailsAsync(imdbId);

            if (!details.Response) return null;

            var entity = details.ToEntity();
            dbContext.CachedMovies.Add(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
