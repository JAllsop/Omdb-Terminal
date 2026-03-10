using Microsoft.EntityFrameworkCore;

namespace OmdbTerminal.ApiService.Data
{
    public class OmdbDbContext(DbContextOptions<OmdbDbContext> options) : DbContext(options)
    {
        public DbSet<MovieEntity> CachedMovies => Set<MovieEntity>();
    }
}
