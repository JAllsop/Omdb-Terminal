using Microsoft.EntityFrameworkCore;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Data
{
    public class OmdbDbContext(DbContextOptions<OmdbDbContext> options) : DbContext(options)
    {
        public DbSet<MovieEntity> CachedMovies => Set<MovieEntity>();

        public DbSet<SearchCacheEntity> SearchCache => Set<SearchCacheEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieEntity>()
                .Property(m => m.Type)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToApiString() : "N/A",
                    v => v.ParseMediaType()
                );

            modelBuilder.Entity<SearchCacheEntity>()
                .Property(s => s.Type)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToApiString() : null,
                    v => string.IsNullOrEmpty(v) ? null : v.ParseMediaType()
                );
        }
    }
}
