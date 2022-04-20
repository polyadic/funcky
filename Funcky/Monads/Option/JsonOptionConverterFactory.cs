using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads;

/// <remarks>This type needs to be public, since the <c>System.Text.Json</c> source generator needs to be able to instantiate this in user code.</remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class JsonOptionConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    public override JsonConverter CreateConverter(Type optionType, JsonSerializerOptions options)
        => (JsonConverter)Activator.CreateInstance(GetConverterType(optionType.GetGenericArguments().Single()), options)!;

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "JsonOptionConverter<> doesn't call anything on typeToConvert")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private static Type GetConverterType(Type typeToConvert) => typeof(JsonOptionConverter<>).MakeGenericType(typeToConvert);
}
