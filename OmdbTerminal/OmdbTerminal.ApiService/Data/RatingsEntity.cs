using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmdbTerminal.ApiService.Data
{
    [PrimaryKey(nameof(MovieId), nameof(Source))]
    public class RatingsEntity
    {
        public string MovieId { get; set; } = default!;

        [ForeignKey(nameof(MovieId))]
        public MovieEntity? Movie { get; set; }

        public string Source { get; set; } = default!;

        public string Value { get; set; } = default!;
    }
}
