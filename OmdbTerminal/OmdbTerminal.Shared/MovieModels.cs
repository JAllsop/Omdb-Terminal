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
    [JsonConverter(typeof(StringToMediaTypeConverter))]
    public MediaType? Type { get; set; }

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
    [JsonConverter(typeof(StringToIntConverter))]
    public int TotalResults { get; set; } = 0;

    [JsonPropertyName("Response")]
    [JsonConverter(typeof(StringToBoolConverter))]
    public bool Response { get; set; } = false;

    [JsonPropertyName("Error")]
    public string? Error { get; set; }
}

/// <summary>
/// Represents a single rating from a specific source (e.g., Rotten Tomatoes, Metacritic)
/// </summary>
public class RatingDetail
{
    [JsonPropertyName("Source")]
    public string Source { get; set; } = "N/A";

    [JsonPropertyName("Value")]
    public string Value { get; set; } = "N/A";
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
    public string Rated { get; set; } = "N/A";

    [JsonPropertyName("Released")]
    public string Released { get; set; } = "N/A";

    [JsonPropertyName("Runtime")]
    public string Runtime { get; set; } = "N/A";

    [JsonPropertyName("Genre")]
    public string Genre { get; set; } = "N/A";

    [JsonPropertyName("Director")]
    public string Director { get; set; } = "N/A";

    [JsonPropertyName("Writer")]
    public string Writer { get; set; } = "N/A";

    [JsonPropertyName("Actors")]
    public string Actors { get; set; } = "N/A";

    [JsonPropertyName("Plot")]
    public string Plot { get; set; } = "N/A";

    [JsonPropertyName("Language")]
    public string Language { get; set; } = "N/A";

    [JsonPropertyName("Country")]
    public string Country { get; set; } = "N/A";

    [JsonPropertyName("Awards")]
    public string Awards { get; set; } = "N/A";

    [JsonPropertyName("Poster")]
    public string PosterUrl { get; set; } = "N/A";

    [JsonPropertyName("Ratings")]
    public List<RatingDetail> Ratings { get; set; } = [];

    [JsonPropertyName("Metascore")]
    public string Metascore { get; set; } = "N/A";

    [JsonPropertyName("imdbRating")]
    public string ImdbRating { get; set; } = "N/A";

    [JsonPropertyName("imdbVotes")]
    public string ImdbVotes { get; set; } = "N/A";

    [JsonPropertyName("imdbID")]
    public string ImdbId { get; set; } = default!;

    [JsonPropertyName("Id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ODataId
    {
        get => null;
        set { if (value != null) ImdbId = value; }
    }

    [JsonPropertyName("Type")]
    [JsonConverter(typeof(StringToMediaTypeConverter))]
    public MediaType? Type { get; set; }

    [JsonPropertyName("DVD")]
    public string DVD { get; set; } = "N/A";

    [JsonPropertyName("BoxOffice")]
    public string BoxOffice { get; set; } = "N/A";

    [JsonPropertyName("Production")]
    public string Production { get; set; } = "N/A";

    [JsonPropertyName("Website")]
    public string Website { get; set; } = "N/A";

    [JsonPropertyName("Response")]
    [JsonConverter(typeof(StringToBoolConverter))]
    public bool Response { get; set; } = false;

    public bool IsDetailed { get; set; } = false;

    public bool IsCustom { get; set; } = false;
}