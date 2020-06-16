using System;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class StringExtensionsTest
    {
        private const string Haystack = "haystack";
        private const string NonExistingNeedle = "needle";
        private const char NonExistingNeedleChar = 'n';
        private const string ExistingNeedle = "ystack";
        private const char ExistingNeedleChar = 'y';
        private const int NeedlePosition = 2;

        [Fact]
        public void ReturnsNoneIfNeedleIsNotFound()
        {
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedleChar));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedleChar, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }));
            AssertIsNone(Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0));
            AssertIsNone(Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedleChar));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture));
            AssertIsNone(Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }));
            AssertIsNone(Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0));
            AssertIsNone(Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1));
        }

        [Fact]
        public void ReturnsIndexIfNeedleIsFound()
        {
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedleChar));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedleChar, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0, count: Haystack.Length));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }));
            AssertIsSome(NeedlePosition, Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0));
            AssertIsSome(NeedlePosition, Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0, count: Haystack.Length));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedleChar));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1, count: Haystack.Length));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length, StringComparison.InvariantCulture));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1));
            AssertIsSome(NeedlePosition, Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1, count: Haystack.Length));
        }

        private static void AssertIsNone<TItem>(Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(default, option);

        private static void AssertIsSome<TItem>(TItem value, Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(Option.Some(value), option);
    }
}
