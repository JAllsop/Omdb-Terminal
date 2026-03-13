using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services
{
    public interface IOmdbClient
    {
        Task<OmdbSearchResponse> SearchMoviesAsync(string title, int page = 1);

        Task<MovieDetails> GetMovieDetailsByIdAsync(string imdbId);

        Task<MovieDetails> GetMovieDetailsByTitleAsync(string title);
    }
}
