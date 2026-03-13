namespace OmdbTerminal.Shared;

public enum MediaType
{
    Movie,
    Series,
    Episode
}

public static class MediaTypeExtensions
{
    public static string ToApiString(this MediaType type)
    {
        return type switch
        {
            MediaType.Movie => "movie",
            MediaType.Series => "series",
            MediaType.Episode => "episode",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static MediaType? ParseMediaType(this string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return null;

        if (int.TryParse(type, out int numericType))
        {
            if (Enum.IsDefined(typeof(MediaType), numericType))
                return (MediaType)numericType;
        }

        return type.ToLowerInvariant() switch
        {
            "movie" => MediaType.Movie,
            "series" => MediaType.Series,
            "episode" => MediaType.Episode,
            _ => null
        };
    }
}