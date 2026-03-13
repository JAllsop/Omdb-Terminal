using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;

namespace OmdbTerminal.ApiService.Services
{
    public class CachedEntriesService(OmdbDbContext dbContext) : ICachedEntriesService
    {
        public IQueryable<MovieEntity> GetQueryable()
        {
            return dbContext.CachedMovies;
        }

        public async Task<MovieEntity?> GetByIdAsync(string id)
        {
            return await dbContext.CachedMovies.FindAsync(id);
        }

        public async Task<List<MovieEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return await dbContext.CachedMovies
                .Where(m => ids.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(MovieEntity movie)
        {
            var existingMovie = await dbContext.CachedMovies.FindAsync(movie.Id);
            if (existingMovie != null) return false;

            movie.CachedAt = DateTime.UtcNow;
            dbContext.CachedMovies.Add(movie);
            var count = await dbContext.SaveChangesAsync();

            if(count == 0) return false;
            return true;
        }

        public async Task<bool> UpdateAsync(string id, MovieEntity updatedMovie)
        {
            var existingMovie = await dbContext.CachedMovies.FindAsync(id);
            if (existingMovie == null) return false;

            existingMovie.Title = updatedMovie.Title;
            existingMovie.Year = updatedMovie.Year;
            existingMovie.Plot = updatedMovie.Plot;
            existingMovie.PosterUrl = updatedMovie.PosterUrl;
            existingMovie.Genre = updatedMovie.Genre;
            existingMovie.CachedAt = DateTime.UtcNow;

            var count = await dbContext.SaveChangesAsync();

            if(count == 0) return false;
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var movie = await dbContext.CachedMovies.FindAsync(id);
            if (movie == null) return false;

            dbContext.CachedMovies.Remove(movie);
            var count = await dbContext.SaveChangesAsync();

            if(count == 0) return false;
            return true;
        }        

        public async Task<SearchCacheEntity?> GetSearchCacheAsync(string query, int page)
        {
            // MySQL is case-insensitive by default direct comparison works fine
            return await dbContext.SearchCache
                .FirstOrDefaultAsync(s => s.Query == query && s.Page == page);
        }

        public async Task<bool> SaveSearchCacheAsync(SearchCacheEntity searchCache, IEnumerable<MovieEntity> moviesFromSearch)
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

            if(count == 0) return false;
            return true;
        }

        public async Task<int> ClearCacheAsync()
        {
            var deletedCount = await dbContext.CachedMovies.ExecuteDeleteAsync();
            deletedCount += await dbContext.SearchCache.ExecuteDeleteAsync();
            return deletedCount;
        }
    }
}
