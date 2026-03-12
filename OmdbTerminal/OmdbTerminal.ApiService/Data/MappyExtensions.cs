using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Data
{
    public static class MappingExtensions
    {
        public static MovieEntity ToEntity(this MovieDetails details)
            => new()
            {
                Id = details.ImdbId,
                Title = details.Title,
                Year = details.Year,
                Rated = details.Rated,
                Released = details.Released,
                Runtime = details.Runtime,
                Genre = details.Genre,
                Director = details.Director,
                Plot = details.Plot,
                ImdbRating = details.ImdbRating,
                PosterUrl = details.PosterUrl,
                CachedAt = DateTime.UtcNow
            };
    }
}
