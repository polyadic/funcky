using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funcky.Monads
{
    internal sealed class JsonOptionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

        public override JsonConverter CreateConverter(Type optionType, JsonSerializerOptions options)
            => (JsonConverter)Activator.CreateInstance(
                typeof(JsonOptionConverter<>).MakeGenericType(optionType.GetGenericArguments()),
                options)!;
    }
}
