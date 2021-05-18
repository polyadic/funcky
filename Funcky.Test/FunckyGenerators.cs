using FsCheck;
using Funcky.Monads;

namespace Funcky.Test
{
    internal static class FunckyGenerators
    {
        public static void Register() => Arb.Register(typeof(FunckyGenerators));

        public static Arbitrary<Either<int, int>> ArbitraryEitherOfInt()
            => Arb.From(Gen.OneOf(
                Arb.Generate<int>().Select(Either<int, int>.Left),
                Arb.Generate<int>().Select(Either<int, int>.Right)));
    }
}
