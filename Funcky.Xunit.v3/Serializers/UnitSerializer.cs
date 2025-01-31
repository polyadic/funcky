using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace Funcky.Xunit.Serializers;

internal sealed class UnitSerializer : IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue) => Unit.Value;

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = string.Empty;
        return type == typeof(Unit);
    }

    public string Serialize(object value) => string.Empty;
}
