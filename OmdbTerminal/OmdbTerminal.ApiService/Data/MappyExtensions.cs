using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Data
{
    public static class MappingExtensions
    {
        public static MovieEntity ToEntity(this MovieDetails details)
            => new()
            {
                ImdbId = details.ImdbId,
                Title = details.Title,
                Year = details.Year,
                Plot = details.Plot,
                PosterUrl = details.PosterUrl,
                Genre = details.Genre,
                CachedAt = DateTime.UtcNow
            };
    }
}
