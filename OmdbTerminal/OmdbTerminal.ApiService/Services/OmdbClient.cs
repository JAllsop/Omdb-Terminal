using System.Net.Http.Json;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Services;

public class OmdbClient(HttpClient httpClient, IConfiguration configuration) : IOmdbClient
{
    private readonly string _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDB API Key is missing");

    public async Task<OmdbSearchResponse> SearchMoviesAsync(string title, int page = 1)
    {
        var url = $"?apikey={_apiKey}&s={Uri.EscapeDataString(title)}&page={page}";
        var response = await httpClient.GetFromJsonAsync<OmdbSearchResponse>(url);

        return response ?? new OmdbSearchResponse();
    }

    public async Task<MovieDetails> GetMovieDetailsAsync(string imdbId)
    {
        var url = $"?apikey={_apiKey}&i={imdbId}&plot=full";
        var response = await httpClient.GetFromJsonAsync<MovieDetails>(url);

        return response ?? new MovieDetails();
    }
}