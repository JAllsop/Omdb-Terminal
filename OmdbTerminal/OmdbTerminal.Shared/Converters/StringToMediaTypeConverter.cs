using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmdbTerminal.Shared.Converters;

public class StringToMediaTypeConverter : JsonConverter<MediaType?>
{
    public override MediaType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
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
