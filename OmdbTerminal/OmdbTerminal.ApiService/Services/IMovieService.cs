using OmdbTerminal.ApiService.Data;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services
{
    public interface IMovieService
    {
        Task<OmdbSearchResponse> SearchAsync(string title, int page = 1);
     
        Task<MovieDetails?> GetDetailsByIdAsync(string imdbId);

        Task<MovieDetails?> GetDetailsByTitleAsync(string title);
    }
}
