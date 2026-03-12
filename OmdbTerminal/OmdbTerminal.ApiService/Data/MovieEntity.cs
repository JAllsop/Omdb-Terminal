using OmdbTerminal.Shared;
using System.ComponentModel.DataAnnotations;

namespace OmdbTerminal.ApiService.Data
{
    public class MovieEntity
    {
        [Key]
        public string Id { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Year { get; set; } = default!;

        public string Rated { get; set; } = default!;

        public string Released { get; set; } = default!;

        public string Runtime { get; set; } = default!;

        public string Genre { get; set; } = default!;

        public string Director { get; set; } = default!;

        public string Plot { get; set; } = default!;

        public string ImdbRating { get; set; } = default!;

        public string PosterUrl { get; set; } = default!;

        public bool IsCustom { get; set; } = false;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        public MovieDetails ToDetails()
            => new()
            {
                Title = this.Title,
                Year = this.Year,
                Rated = this.Rated,
                Released = this.Released,
                Runtime = this.Runtime,
                Genre = this.Genre,
                Director = this.Director,
                Plot = this.Plot,
                ImdbId = this.Id,
                ImdbRating = this.ImdbRating,
                PosterUrl = this.PosterUrl,
                IsCustom = this.IsCustom
            };
    }
}
