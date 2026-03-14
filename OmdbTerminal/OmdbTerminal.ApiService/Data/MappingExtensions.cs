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
                Writer = details.Writer,
                Actors = details.Actors,
                Plot = details.Plot,
                Language = details.Language,
                Country = details.Country,
                Awards = details.Awards,
                PosterUrl = details.PosterUrl,
                Metascore = details.Metascore,
                ImdbRating = details.ImdbRating,
                ImdbVotes = details.ImdbVotes,
                Type = details.Type,
                DVD = details.DVD,
                BoxOffice = details.BoxOffice,
                Production = details.Production,
                Website = details.Website,
                Ratings = [.. details.Ratings.Select(r => new RatingsEntity { MovieId = details.ImdbId, Source = r.Source, Value = r.Value })],
                CachedAt = DateTime.UtcNow,
                IsDetailed = details.IsDetailed,
                IsCustom = details.IsCustom
            };

        public static MovieEntity ToEntity(this MovieSearchResult searchResult)
            => new()
            {
                Id = searchResult.ImdbId,
                Title = searchResult.Title,
                Year = searchResult.Year,
                Type = searchResult.Type,
                PosterUrl = searchResult.PosterUrl,
                CachedAt = DateTime.UtcNow,
                IsDetailed = false
            };

        public static MovieSearchResult ToSearchResult(this MovieEntity entity)
            => new()
            {
                Title = entity.Title,
                Year = entity.Year,
                ImdbId = entity.Id,
                Type = entity.Type,
                PosterUrl = entity.PosterUrl
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
                Writer = entity.Writer,
                Actors = entity.Actors,
                Plot = entity.Plot,
                Language = entity.Language,
                Country = entity.Country,
                Awards = entity.Awards,
                ImdbId = entity.Id,
                PosterUrl = entity.PosterUrl,
                Metascore = entity.Metascore,
                ImdbRating = entity.ImdbRating,
                ImdbVotes = entity.ImdbVotes,
                Type = entity.Type,
                DVD = entity.DVD,
                BoxOffice = entity.BoxOffice,
                Production = entity.Production,
                Website = entity.Website,
                Ratings = [.. entity.Ratings.Select(r => new RatingDetail { Source = r.Source, Value = r.Value })],
                IsDetailed = entity.IsDetailed,
                IsCustom = entity.IsCustom
            };
    }
}
