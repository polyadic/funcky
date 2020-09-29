using System;
using System.Text.Json.Serialization;

namespace Funcky.Monads
{
    internal sealed class JsonOptionConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter? CreateConverter(Type typeToConvert)
            => (JsonConverter?)Activator.CreateInstance(
                typeof(JsonOptionConverter<>)
                    .MakeGenericType(typeToConvert.GetGenericArguments()));
    }
}
