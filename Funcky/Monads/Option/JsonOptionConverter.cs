using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads
{
    internal sealed class JsonOptionConverter<TItem> : JsonConverter<Option<TItem>>
        where TItem : notnull
    {
        public override Option<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null
                ? Option<TItem>.None()
                : Option.Some(JsonSerializer.Deserialize<TItem>(ref reader, options));

        public override void Write(Utf8JsonWriter writer, Option<TItem> value, JsonSerializerOptions options)
            => value.Match(
                none: writer.WriteNullValue,
                some: item => JsonSerializer.Serialize(writer, item, options));
    }
}
