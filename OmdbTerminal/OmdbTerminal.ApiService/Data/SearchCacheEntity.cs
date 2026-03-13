using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OmdbTerminal.ApiService.Data
{
    [PrimaryKey(nameof(Query), nameof(Page))]
    public class SearchCacheEntity
    {
        public string Query { get; set; } = default!;

        public int Page { get; set; } = 1;

        public int TotalResults { get; set; } = 0;

        // JSON mapped list of IMDb IDs
        public List<string> MovieIds { get; set; } = [];

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}