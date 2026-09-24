#if JSON_SERIALIZER_OPTIONS_TRY_GET_TYPE_INFO
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Funcky.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static Option<JsonTypeInfo> GetTypeInfoOrNone(this JsonSerializerOptions options, Type type)
        => options.TryGetTypeInfo(type, out var typeInfo)
            ? typeInfo
            : Option<JsonTypeInfo>.None;
}
#endif
