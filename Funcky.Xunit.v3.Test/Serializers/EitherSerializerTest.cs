using Funcky.Xunit.Serializers;

namespace Funcky.Xunit.Test;

public sealed class EitherSerializerTest
{
    private static readonly EitherSerializer Serializer = new();

    [Theory]
    [MemberData(nameof(EitherProvider))]
    public void RoundtripsEitherValues(IEither either)
    {
        Assert.True(Serializer.IsSerializable(either.GetType(), either, out _));
        var serialized = Serializer.Serialize(either);
        var deserialized = Serializer.Deserialize(either.GetType(), serialized);
        Assert.Equal(either, deserialized);
    }

    public static TheoryData<IEither> EitherProvider()
        => new((IEnumerable<IEither>)[
            Either<string>.Return(10),
            Either<string>.Return(20),
            Either<string, int>.Left("foo"),
            Either<string, int>.Left("bar"),
            Either<string, int>.Left("bar"),
            Either<NonSerializable, int>.Right(42),
            Either<int, NonSerializable>.Left(42),
        ]);

    [Fact]
    public void EitherSideOfNonSerializableTypeIsNotSerializable()
    {
        Assert.False(Serializer.IsSerializable(typeof(Either<NonSerializable, int>), Either<NonSerializable, int>.Left(new NonSerializable()), out _));
        Assert.False(Serializer.IsSerializable(typeof(Either<int, NonSerializable>), Either<int, NonSerializable>.Right(new NonSerializable()), out _));
    }

    public sealed class NonSerializable;
}
