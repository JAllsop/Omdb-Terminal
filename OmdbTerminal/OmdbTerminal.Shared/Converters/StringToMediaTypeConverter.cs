using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmdbTerminal.Shared.Converters;

public class StringToMediaTypeConverter : JsonConverter<MediaType?>
{
    public override MediaType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32().ToString(),
            JsonTokenType.String => reader.GetString(),
            _ => null
        };

        if (string.IsNullOrWhiteSpace(value) || value.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            return null;

        return value.ParseMediaType();
    }

    public override void Write(Utf8JsonWriter writer, MediaType? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToApiString());
        }
        else
        {
            writer.WriteStringValue("N/A");
        }
    }
}
