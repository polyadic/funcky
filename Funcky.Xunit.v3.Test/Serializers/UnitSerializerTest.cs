using Funcky.Xunit.Serializers;

namespace Funcky.Xunit.Test;

public sealed class UnitSerializerTest
{
    private static readonly UnitSerializer Serializer = new();

    [Fact]
    public void RoundtripsUnit()
    {
        var serialized = Serializer.Serialize(Unit.Value);
        var deserialized = Serializer.Deserialize(typeof(Unit), serialized);
        Assert.Equal(Unit.Value, deserialized);
    }

    [Fact]
    public void UnitIsSerializable()
    {
        Assert.True(Serializer.IsSerializable(typeof(Unit), Unit.Value, out _));
    }
}
