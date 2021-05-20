using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads
{
    internal sealed class JsonOptionConverter<TItem> : JsonConverter<Option<TItem>>
        where TItem : notnull
    {
        private readonly JsonConverter<TItem>? _itemConverter;

        public JsonOptionConverter(JsonSerializerOptions options)
        {
            _itemConverter = (JsonConverter<TItem>)options.GetConverter(typeof(TItem));
        }

        public override Option<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null
                ? Option<TItem>.None
                : _itemConverter is not null
                    ? Option.Some(_itemConverter.Read(ref reader, typeof(TItem), options)!)
                    : JsonSerializer.Deserialize<TItem>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, Option<TItem> value, JsonSerializerOptions options)
            => value.Match(
                none: writer.WriteNullValue,
                some: item =>
                {
                    if (_itemConverter is not null)
                    {
                        _itemConverter.Write(writer, item, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                });
    }
}
