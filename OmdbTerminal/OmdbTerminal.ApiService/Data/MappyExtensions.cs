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

        public static MovieDetails ToDetails(this MovieEntity entity)
            => new()
            {
                Title = entity.Title,
                Year = entity.Year,
                Rated = entity.Rated,
                Released = entity.Released,
                Runtime = entity.Runtime,
                Genre = entity.Genre,
                Director = entity.Director,
                Plot = entity.Plot,
                ImdbId = entity.Id,
                ImdbRating = entity.ImdbRating,
                PosterUrl = entity.PosterUrl
            };
    }
}
