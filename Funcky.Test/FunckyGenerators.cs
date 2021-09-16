using FsCheck;
using Result = Funcky.Monads.Result;

namespace Funcky.Test
{
    internal static class FunckyGenerators
    {
        public static void Register() => Arb.Register(typeof(FunckyGenerators));

        public static Arbitrary<Either<int, int>> ArbitraryEitherOfInt()
            => Arb.From(Gen.OneOf(
                Arb.Generate<int>().Select(Either<int, int>.Left),
                Arb.Generate<int>().Select(Either<int, int>.Right)));

        public static Arbitrary<Result<int>> ArbitraryResultOfInt() => Arb.From(Gen.OneOf(
            Arb.Generate<int>().Select(Result.Ok),
            Arb.Generate<string>().Select(message => Result<int>.Error(new EquatableException(message)))));

        private sealed class EquatableException : Exception, IEquatable<EquatableException>
        {
            public EquatableException(string? message)
                : base(message)
            {
            }

            public override bool Equals(object? obj)
                => ReferenceEquals(this, obj) || (obj is EquatableException other && Equals(other));

            public bool Equals(EquatableException? other)
                => other is not null && Message == other.Message;

            public override int GetHashCode() => HashCode.Combine(Message);
        }
    }
}
