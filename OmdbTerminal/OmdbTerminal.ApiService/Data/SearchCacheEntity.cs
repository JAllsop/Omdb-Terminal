using Microsoft.EntityFrameworkCore;
using OmdbTerminal.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmdbTerminal.ApiService.Data
{
    [Index(nameof(Query), nameof(Page), nameof(Type), nameof(Year), IsUnique = true)]
    public class SearchCacheEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Query { get; set; } = default!;

        public MediaType? Type { get; set; }

        public string? Year { get; set; }

        public int Page { get; set; } = 1;

        public int TotalResults { get; set; } = 0;

        // JSON mapped list of IMDb IDs
        public List<string> MovieIds { get; set; } = [];

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}