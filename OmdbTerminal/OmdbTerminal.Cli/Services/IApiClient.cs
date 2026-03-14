using OmdbTerminal.Shared;

namespace OmdbTerminal.Cli.Services;

public interface IApiClient
{
    Task<OmdbSearchResponse?> SearchMoviesAsync(string title, int page = 1, MediaType? type = null, string? year = null);

    Task<MovieDetails?> GetMovieDetailsByIdAsync(string id);

    Task<MovieDetails?> GetMovieDetailsByTitleAsync(string title, MediaType? type = null, string? year = null);
    
    Task<List<MovieDetails>?> GetCachedEntriesAsync(string? filter = null);

    Task<MovieDetails?> GetCachedEntryByIdAsync(string id);

    Task<bool> AddCachedEntryAsync(MovieDetails movie);

    Task<bool> UpdateCachedEntryAsync(string id, MovieDetails movie);

    Task<bool> DeleteCachedEntryAsync(string id);

    Task<bool> ClearCacheAsync();
}
