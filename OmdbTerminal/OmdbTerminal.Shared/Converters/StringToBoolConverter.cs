using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmdbTerminal.Shared.Converters;

public class StringToBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True)
            return true;

        if (reader.TokenType == JsonTokenType.False)
            return false;

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return bool.TryParse(value, out var result) && result;
        }

        throw new JsonException($"Unexpected token type parsing bool: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}