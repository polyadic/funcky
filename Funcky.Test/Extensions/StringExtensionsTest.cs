using System;
using System.Linq;
using System.Reflection;
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

        [Theory]
        [MemberData(nameof(InvalidIndexes))]
        public void ReturnsNoneIfNeedleIsNotFound(Option<int> index)
        {
            Assert.Equal(Option<int>.None(), index);
        }

        public static TheoryData<Option<int>> InvalidIndexes()
            => new TheoryData<Option<int>>
            {
                Haystack.IndexOfOrNone(NonExistingNeedleChar),
                Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0),
                Haystack.IndexOfOrNone(NonExistingNeedleChar, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1),
                Haystack.IndexOfOrNone(NonExistingNeedle),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1),
                Haystack.IndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0),
                Haystack.IndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0),
                Haystack.LastIndexOfOrNone(NonExistingNeedleChar, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedle),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(NonExistingNeedle, startIndex: 0, count: 1, StringComparison.InvariantCulture),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0),
                Haystack.LastIndexOfAnyOrNone(new[] { NonExistingNeedleChar }, startIndex: 0, count: 1),
            };

        [Theory]
        [MemberData(nameof(ValidIndexes))]
        public void ReturnsIndexIfNeedleIsFound(Option<int> index)
        {
            Assert.Equal(Option.Some(NeedlePosition), index);
        }

        public static TheoryData<Option<int>> ValidIndexes()
            => new TheoryData<Option<int>>
            {
                Haystack.IndexOfOrNone(ExistingNeedleChar),
                Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0),
                Haystack.IndexOfOrNone(ExistingNeedleChar, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(ExistingNeedleChar, startIndex: 0, count: Haystack.Length),
                Haystack.IndexOfOrNone(ExistingNeedle),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length),
                Haystack.IndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, StringComparison.InvariantCulture),
                Haystack.IndexOfOrNone(ExistingNeedle, startIndex: 0, count: Haystack.Length, StringComparison.InvariantCulture),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0),
                Haystack.IndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: 0, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfOrNone(ExistingNeedleChar, startIndex: Haystack.Length - 1, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedle),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length),
                Haystack.LastIndexOfOrNone(ExistingNeedle, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, StringComparison.InvariantCulture),
                Haystack.LastIndexOfOrNone(ExistingNeedle, startIndex: Haystack.Length - 1, count: Haystack.Length, StringComparison.InvariantCulture),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1),
                Haystack.LastIndexOfAnyOrNone(new[] { ExistingNeedleChar }, startIndex: Haystack.Length - 1, count: Haystack.Length),
            };

        [Fact]
        public void ValidIndexesCoversAllOverloads()
        {
            var overloadCount = CountIndexOfOverloads();
            var validIndexesCount = ValidIndexes().Count();
            Assert.Equal(overloadCount, validIndexesCount);
        }

        [Fact]
        public void InvalidIndexesCoversAllOverloads()
        {
            var overloadCount = CountIndexOfOverloads();
            var validIndexesCount = InvalidIndexes().Count();
            Assert.Equal(overloadCount, validIndexesCount);
        }

        private static int CountIndexOfOverloads()
            => typeof(StringExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Length;
    }
}
