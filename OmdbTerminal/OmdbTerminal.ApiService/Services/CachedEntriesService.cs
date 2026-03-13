using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services
{
    public class CachedEntriesService(OmdbDbContext dbContext, ILogger<CachedEntriesService> logger) : ICachedEntriesService
    {
        public IQueryable<MovieEntity> GetQueryable()
        {
            return dbContext.CachedMovies;
        }

        public async Task<MovieEntity?> GetByIdAsync(string id)
        {
            try
            {
                return await dbContext.CachedMovies.FindAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching cached movie by id: {Id}", id);
                throw;
            }
        }

        public async Task<List<MovieEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            try
            {
                return await dbContext.CachedMovies
                    .Where(m => ids.Contains(m.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching cached movies by ids");
                throw;
            }
        }

        public async Task<bool> CreateAsync(MovieEntity movie)
        {
            try
            {
                var existingMovie = await dbContext.CachedMovies.FindAsync(movie.Id);
                if (existingMovie != null) return false;

                movie.CachedAt = DateTime.UtcNow;
                dbContext.CachedMovies.Add(movie);
                var count = await dbContext.SaveChangesAsync();

                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating cached movie: {Id}", movie.Id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(string id, MovieEntity updatedMovie)
        {
            try
            {
                var existingMovie = await dbContext.CachedMovies.FindAsync(id);
                if (existingMovie == null)
                {
                    logger.LogWarning("Attempted to update non-existent movie in cache: {Id}", id);
                    return false;
                }

                existingMovie.Title = updatedMovie.Title;
                existingMovie.Year = updatedMovie.Year;
                existingMovie.Plot = updatedMovie.Plot;
                existingMovie.PosterUrl = updatedMovie.PosterUrl;
                existingMovie.Genre = updatedMovie.Genre;
                existingMovie.CachedAt = DateTime.UtcNow;

                var count = await dbContext.SaveChangesAsync();

                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating cached movie: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var movie = await dbContext.CachedMovies.FindAsync(id);
                if (movie == null)
                {
                    logger.LogWarning("Attempted to delete non-existent movie from cache: {Id}", id);
                    return false;
                }

                dbContext.CachedMovies.Remove(movie);
                var count = await dbContext.SaveChangesAsync();

                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting cached movie: {Id}", id);
                throw;
            }
        }

        public async Task<SearchCacheEntity?> GetSearchCacheAsync(string query, int page, MediaType? type, string? year)
        {
            try
            {
                // MySQL is case-insensitive by default direct comparison works fine
                return await dbContext.SearchCache
                    .FirstOrDefaultAsync(s => s.Query == query && s.Page == page && s.Type == type && s.Year == year);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting search cache for query: {Query}, page: {Page}", query, page);
                throw;
            }
        }

        public async Task<bool> SaveSearchCacheAsync(SearchCacheEntity searchCache, IEnumerable<MovieEntity> moviesFromSearch)
        {
            try
            {
                foreach (var movie in moviesFromSearch)
                {
                    var existing = await dbContext.CachedMovies.FindAsync(movie.Id);
                    if (existing == null)
                    {
                        dbContext.CachedMovies.Add(movie);
                    }
                }

                dbContext.SearchCache.Add(searchCache);
                var count = await dbContext.SaveChangesAsync();

                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving search cache for query: {Query}", searchCache.Query);
                throw;
            }
        }

        public async Task<int> ClearCacheAsync()
        {
            try
            {
                var deletedCount = await dbContext.CachedMovies.ExecuteDeleteAsync();
                deletedCount += await dbContext.SearchCache.ExecuteDeleteAsync();
                return deletedCount;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Critical error clearing cache completely");
                throw;
            }
        }
    }
}
