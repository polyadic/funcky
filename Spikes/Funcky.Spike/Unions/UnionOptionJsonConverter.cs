using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Spike.Unions;

/// <summary>Same contract as Funcky's <c>OptionJsonConverter</c>: Some is transparent, None is <c>null</c>. The union shape
/// changes nothing here, the converter is written against Match/Switch. Without a converter System.Text.Json serializes
/// the generated <c>object Value</c> property, which the tests record.</summary>
public sealed class UnionOptionJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(UnionOption<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments().Single();
        return (JsonConverter)Activator.CreateInstance(typeof(UnionOptionJsonConverter<>).MakeGenericType(itemType), options.GetConverter(itemType))!;
    }
}

internal sealed class UnionOptionJsonConverter<T>(JsonConverter<T> itemConverter) : JsonConverter<UnionOption<T>>
    where T : notnull
{
    public override UnionOption<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType is JsonTokenType.Null
            ? UnionOption<T>.Empty
            : itemConverter.Read(ref reader, typeof(T), options)!;

    public override void Write(Utf8JsonWriter writer, UnionOption<T> value, JsonSerializerOptions options)
        => value.Switch(
            none: writer.WriteNullValue,
            some: item => itemConverter.Write(writer, item, options));
}
