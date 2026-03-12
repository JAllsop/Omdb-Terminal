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

        public async Task<bool> CreateAsync(MovieEntity movie)
        {
            var existingMovie = await dbContext.CachedMovies.FindAsync(movie.ImdbId);
            if (existingMovie != null) return false;

            movie.CachedAt = DateTime.UtcNow;
            dbContext.CachedMovies.Add(movie);
            await dbContext.SaveChangesAsync();

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

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var movie = await dbContext.CachedMovies.FindAsync(id);
            if (movie == null) return false;

            dbContext.CachedMovies.Remove(movie);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
