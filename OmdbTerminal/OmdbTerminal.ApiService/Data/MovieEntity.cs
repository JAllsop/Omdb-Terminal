using System.ComponentModel.DataAnnotations;

namespace OmdbTerminal.ApiService.Data
{
    public class MovieEntity
    {
        [Key]
        public string ImdbId { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Year { get; set; } = default!;

        public string Plot { get; set; } = default!;

        public string PosterUrl { get; set; } = default!;

        public string Genre { get; set; } = default!;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
