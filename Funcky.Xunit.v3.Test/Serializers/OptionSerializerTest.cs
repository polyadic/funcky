using Funcky.Xunit.Serializers;

namespace Funcky.Xunit.Test;

public sealed class OptionSerializerTest
{
    private static readonly OptionSerializer Serializer = new();

    [Theory]
    [MemberData(nameof(OptionProvider))]
    public void RoundtripsOptionValues(Option<int> option)
    {
        var serialized = Serializer.Serialize(option);
        var deserialized = Serializer.Deserialize(option.GetType(), serialized);
        Assert.Equal(option, deserialized);
    }

    public static TheoryData<Option<int>> OptionProvider()
        => [
            Option.Some(10),
            Option.Some(20),
            Option<int>.None,
        ];

    [Fact]
    public void OptionOfSerializableTypeIsSerializable()
    {
        Assert.True(Serializer.IsSerializable(typeof(Option<int>), Option.Some(10), out _));
        Assert.True(Serializer.IsSerializable(typeof(Option<int>), Option<int>.None, out _));
    }

    [Fact]
    public void OptionOfNonSerializableTypeIsNotSerializable()
    {
        Assert.False(Serializer.IsSerializable(typeof(Option<NonSerializable>), Option.Some(new NonSerializable()), out _));
    }

    [Fact]
    public void NoneIsAlwaysSerializable()
    {
        Assert.True(Serializer.IsSerializable(typeof(Option<NonSerializable>), Option<NonSerializable>.None, out _));
    }

    public sealed class NonSerializable;
}
