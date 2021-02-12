using System;
using System.Collections.Generic;
using Funcky.Extensions;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class FirstSingleLastOrNoneTest
    {
        [Theory]
        [MemberData(nameof(ValueReferenceEnumerables))]
        public void GivenAnValueEnumerableFirstLastOrNoneGivesTheCorrectOption(List<int> valueEnumerable, List<string> referenceEnumerable)
        {
            Assert.Equal(ExpectedOptionValue(valueEnumerable), valueEnumerable.FirstOrNone().Match(false, True));
            Assert.Equal(ExpectedOptionValue(referenceEnumerable), referenceEnumerable.FirstOrNone().Match(false, True));

            Assert.Equal(ExpectedOptionValue(valueEnumerable), valueEnumerable.LastOrNone().Match(false, True));
            Assert.Equal(ExpectedOptionValue(referenceEnumerable), referenceEnumerable.LastOrNone().Match(false, True));
        }

        [Theory]
        [MemberData(nameof(ValueReferenceEnumerables))]
        public void GivenAnEnumerableSingleOrNoneGivesTheCorrectOption(List<int> valueEnumerable, List<string> referenceEnumerable)
        {
            ExpectedSingleOrNoneBehaviour(valueEnumerable, () => valueEnumerable.SingleOrNone().Match(false, True));
            ExpectedSingleOrNoneBehaviour(valueEnumerable, () => referenceEnumerable.SingleOrNone().Match(false, True));
        }

        public static TheoryData<List<int>, List<string>> ValueReferenceEnumerables()
            => new()
            {
                { new List<int>(), new List<string>() },
                { new List<int> { 1 }, new List<string> { "a" } },
                { new List<int> { 1, 2, 3 }, new List<string> { "a", "b", "c" } },
            };

        private static bool ExpectedOptionValue<T>(List<T> valueEnumerable) =>
            valueEnumerable.Count switch
            {
                0 => false,
                _ => true,
            };

        private static void ExpectedSingleOrNoneBehaviour<T>(List<T> list, Func<bool> singleOrNone)
        {
            switch (list.Count)
            {
                case 0:
                    Assert.False(singleOrNone());
                    break;
                case 1:
                    Assert.True(singleOrNone());
                    break;
                default:
                    Assert.Throws<InvalidOperationException>(() => singleOrNone());
                    break;
            }
        }
    }
}
