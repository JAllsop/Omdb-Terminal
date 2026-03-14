using OmdbTerminal.Shared;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OmdbTerminal.ApiService.Data
{
    public class MovieEntity
    {
        [Key]
        public string Id { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Year { get; set; } = default!;

        public string Rated { get; set; } = "N/A";

        public string Released { get; set; } = "N/A";

        public string Runtime { get; set; } = "N/A";

        public string Genre { get; set; } = "N/A";

        public string Director { get; set; } = "N/A";

        public string Writer { get; set; } = "N/A";

        public string Actors { get; set; } = "N/A";

        public string Plot { get; set; } = "N/A";

        public string Language { get; set; } = "N/A";

        public string Country { get; set; } = "N/A";

        public string Awards { get; set; } = "N/A";

        [JsonPropertyName("Poster")]
        public string PosterUrl { get; set; } = "N/A";

        public List<RatingsEntity> Ratings { get; set; } = [];

        public string Metascore { get; set; } = "N/A";

        public string ImdbRating { get; set; } = "N/A";

        public string ImdbVotes { get; set; } = "N/A";

        public MediaType? Type { get; set; }

        public string DVD { get; set; } = "N/A";

        public string BoxOffice { get; set; } = "N/A";

        public string Production { get; set; } = "N/A";

        public string Website { get; set; } = "N/A";

        public bool IsCustom { get; set; } = false;

        // Denotes whether this entry contains the full details from a specific lookup or the more limited data from a search result
        public bool IsDetailed { get; set; } = false;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
