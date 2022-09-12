using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads;

/// <remarks>A JSON converter that serializes <see cref="Option{TItem}.None"/> as <c>null</c>.</remarks>
public sealed class OptionJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(OptionJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments().Single());
        return (JsonConverter)Activator.CreateInstance(converterType, options)!;
    }
}

internal sealed class OptionJsonConverter<TItem> : JsonConverter<Option<TItem>>
    where TItem : notnull
{
    private const string UnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed.";

    private readonly JsonConverter<TItem> _itemConverter;

    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    public OptionJsonConverter(JsonSerializerOptions options)
    {
        _itemConverter = (JsonConverter<TItem>)options.GetConverter(typeof(TItem));
    }

    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "JsonSerializer is annotated with RequiresUnreferencedCode")]
    public override Option<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null
            ? Option<TItem>.None
            : _itemConverter is not null
                ? Option.Some(_itemConverter.Read(ref reader, typeof(TItem), options)!)
                : JsonSerializer.Deserialize<TItem>(ref reader, options)!;

    [RequiresUnreferencedCode(UnreferencedCodeMessage)]
    [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "JsonSerializer is annotated with RequiresUnreferencedCode")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1134:Attributes should not share line", Justification = "Lambda attribute is fine on the same line")]
    public override void Write(Utf8JsonWriter writer, Option<TItem> value, JsonSerializerOptions options)
        => value.Switch(
            none: writer.WriteNullValue,
            some: [RequiresUnreferencedCode(UnreferencedCodeMessage)] (item) =>
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
