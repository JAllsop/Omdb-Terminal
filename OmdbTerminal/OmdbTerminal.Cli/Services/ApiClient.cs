using System.Net.Http.Json;
using OmdbTerminal.Shared;

namespace OmdbTerminal.Cli.Services;

internal class ApiClient(MoviesHttpClient moviesClient, CachedEntriesHttpClient cachedEntriesClient) : IApiClient
{
    public async Task<OmdbSearchResponse?> SearchMoviesAsync(string title, int page = 1, MediaType? type = null, string? year = null)
    {
        var url = $"search?title={Uri.EscapeDataString(title)}&page={page}";
        if (type.HasValue) url += $"&type={type.Value}";
        if (!string.IsNullOrWhiteSpace(year)) url += $"&year={Uri.EscapeDataString(year)}";

        return await moviesClient.GetFromJsonAsync<OmdbSearchResponse>(url);
    }

    public async Task<MovieDetails?> GetMovieDetailsByIdAsync(string id)
    {
        return await moviesClient.GetFromJsonAsync<MovieDetails>($"detailsId/{id}");
    }

    public async Task<MovieDetails?> GetMovieDetailsByTitleAsync(string title, MediaType? type = null, string? year = null)
    {
        var url = $"detailsTitle/{Uri.EscapeDataString(title)}";
        var queryParams = new List<string>();
        if (type.HasValue) queryParams.Add($"type={type.Value}");
        if (!string.IsNullOrWhiteSpace(year)) queryParams.Add($"year={Uri.EscapeDataString(year)}");

        if (queryParams.Count != 0) url += "?" + string.Join("&", queryParams);

        return await moviesClient.GetFromJsonAsync<MovieDetails>(url);
    }

    public async Task<List<MovieDetails>?> GetCachedEntriesAsync(string? filter = null)
    {
        var url = string.IsNullOrWhiteSpace(filter) ? "" : filter.StartsWith('?') ? filter : $"?$filter={Uri.EscapeDataString(filter)}";
        url += "&$expand=Ratings";

        return await cachedEntriesClient.GetFromJsonAsync<List<MovieDetails>>(url);
    }

    public async Task<MovieDetails?> GetCachedEntryByIdAsync(string id)
    {
        return await cachedEntriesClient.GetFromJsonAsync<MovieDetails>(id);
    }

    public async Task<bool> AddCachedEntryAsync(MovieDetails movie)
    {
        movie.ImdbId = Guid.NewGuid().ToString();
        movie.IsDetailed = true;
        movie.IsCustom = true;
        var res = await cachedEntriesClient.PostAsJsonAsync("", movie);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCachedEntryAsync(string id, MovieDetails movie)
    {
        var res = await cachedEntriesClient.PutAsJsonAsync(id, movie);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCachedEntryAsync(string id)
    {
        var res = await cachedEntriesClient.DeleteAsync(id);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCacheAsync()
    {
        var response = await cachedEntriesClient.DeleteFromJsonAsync<ClearCacheResponse>("clear");
        return response?.Success ?? false;
    }
}