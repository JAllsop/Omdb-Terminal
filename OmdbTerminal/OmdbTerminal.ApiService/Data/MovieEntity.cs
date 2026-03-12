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

        public string? Rated { get; set; }

        public string? Released { get; set; }

        public string? Runtime { get; set; }

        public string? Genre { get; set; }

        public string? Director { get; set; }

        public string? Plot { get; set; }

        public string? ImdbRating { get; set; }

        public string? PosterUrl { get; set; }

        public bool IsCustom { get; set; } = false;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
