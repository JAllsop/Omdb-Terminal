using OmdbTerminal.Shared.Converters;
using System.Text.Json.Serialization;

namespace OmdbTerminal.Shared;

/// <summary>
/// Represents a movie entry in the list of search results
/// </summary>
public class MovieSearchResult
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("Year")]
    public string Year { get; set; } = default!;

    [JsonPropertyName("imdbID")]
    public string ImdbId { get; set; } = default!;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("Poster")]
    public string PosterUrl { get; set; } = default!;
}

/// <summary>
/// Represents the response from the OMDb API when searching for movies, including a list of results and total count
/// </summary>
public class OmdbSearchResponse
{
    [JsonPropertyName("Search")]
    public List<MovieSearchResult>? Results { get; set; }

    [JsonPropertyName("totalResults")]
    public string? TotalResults { get; set; }

    [JsonPropertyName("Response")]
    [JsonConverter(typeof(StringToBoolConverter))]
    public bool Response { get; set; } = false;

    [JsonPropertyName("Error")]
    public string? Error { get; set; }
}

/// <summary>
/// Represents the detailed information about a specific movie, as returned by the OMDb API when querying by IMDb ID
/// </summary>
public class MovieDetails
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("Year")]
    public string Year { get; set; } = default!;

    [JsonPropertyName("Rated")]
    public string Rated { get; set; } = default!;

    [JsonPropertyName("Released")]
    public string Released { get; set; } = default!;

    [JsonPropertyName("Runtime")]
    public string Runtime { get; set; } = default!;

    [JsonPropertyName("Genre")]
    public string Genre { get; set; } = default!;

    [JsonPropertyName("Director")]
    public string Director { get; set; } = default!;

    [JsonPropertyName("Plot")]
    public string Plot { get; set; } = default!;

    [JsonPropertyName("imdbID")]
    public string ImdbId { get; set; } = default!;

    [JsonPropertyName("imdbRating")]
    public string ImdbRating { get; set; } = default!;

    [JsonPropertyName("Poster")]
    public string PosterUrl { get; set; } = default!;

    [JsonPropertyName("Response")]
    [JsonConverter(typeof(StringToBoolConverter))]
    public bool Response { get; set; } = false;

    public bool IsCustom { get; set; } = false;
}