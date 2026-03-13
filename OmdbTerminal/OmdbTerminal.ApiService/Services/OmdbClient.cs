using System.Net.Http.Json;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services;

public class OmdbClient(HttpClient httpClient, IConfiguration configuration, ILogger<OmdbClient> logger) : IOmdbClient
{
    private readonly string _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDB API Key is missing");

    private const string _searchUrlTemplate = "?apikey={0}&s={1}&page={2}";
    private const string _detailsByIdUrlTemplate = "?apikey={0}&i={1}&plot=full";
    private const string _detailsByTitleUrlTemplate = "?apikey={0}&t={1}&plot=full";

    public async Task<OmdbSearchResponse> SearchMoviesAsync(string title, int page = 1)
    {
        try
        {
            var url = string.Format(_searchUrlTemplate, _apiKey, Uri.EscapeDataString(title), page);
            var response = await httpClient.GetFromJsonAsync<OmdbSearchResponse>(url);

            return response ?? new OmdbSearchResponse();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when searching movies for title: {Title}, page: {Page}", title, page);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error searching movies for title: {Title}, page: {Page}", title, page);
            throw;
        }
    }

    public async Task<MovieDetails> GetMovieDetailsByIdAsync(string imdbId)
    {
        try
        {
            var url = string.Format(_detailsByIdUrlTemplate, _apiKey, imdbId);
            var response = await httpClient.GetFromJsonAsync<MovieDetails>(url);

            return response ?? new MovieDetails();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when getting movie details by ID: {ImdbId}", imdbId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting movie details by ID: {ImdbId}", imdbId);
            throw;
        }
    }

    public async Task<MovieDetails> GetMovieDetailsByTitleAsync(string title)
    {
        try
        {
            var url = string.Format(_detailsByTitleUrlTemplate, _apiKey, Uri.EscapeDataString(title));
            var response = await httpClient.GetFromJsonAsync<MovieDetails>(url);
            return response ?? new MovieDetails();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when getting movie details by title: {Title}", title);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting movie details by title: {Title}", title);
            throw;
        }
    }
}