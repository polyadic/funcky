using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads;

/// <summary>A JSON converter for <see cref="Option{TItem}"/> that serializes <see cref="Option.Some{TItem}"/> transparently and <see cref="Option{TItem}.None"/> as <c>null</c>.</summary>
public sealed class OptionJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2046:\'RequiresUnreferencedCodeAttribute\' annotations must match across all interface implementations or overrides.")]
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments().Single();
        var converterType = typeof(OptionJsonConverter<>).MakeGenericType(itemType);
        var itemConverter = options.GetConverter(itemType);
        return (JsonConverter)Activator.CreateInstance(converterType, itemConverter)!;
    }
}

internal sealed class OptionJsonConverter<TItem> : JsonConverter<Option<TItem>>
    where TItem : notnull
{
    private readonly JsonConverter<TItem> _itemConverter;

    public OptionJsonConverter(JsonConverter<TItem> itemConverter)
        => _itemConverter = itemConverter;

    public override Option<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null
            ? Option<TItem>.None
            : _itemConverter.Read(ref reader, typeof(TItem), options)!;

    public override void Write(Utf8JsonWriter writer, Option<TItem> value, JsonSerializerOptions options)
        => value.Switch(
            none: writer.WriteNullValue,
            some: item => _itemConverter.Write(writer, item, options));
}
