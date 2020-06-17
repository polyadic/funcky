using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    internal class FunctionalAssert
    {
        public static void Unmatched()
            => throw new UnmatchedException();

        public static void Unmatched(string unmatchedCase)
            => throw new UnmatchedException(unmatchedCase);

        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(Option<TItem>.None(), option);

        public static void IsSome<TItem>(TItem expectedValue, Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(Option.Some(expectedValue), option);

        public static void IsSome<TItem>(Option<TItem> option)
            where TItem : notnull
            => Assert.True(option.Match(none: false, some: True));
    }
}
