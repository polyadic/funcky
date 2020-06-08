using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

using static Funcky.Functional;

namespace Funcky.Test
{
    public class EnumerableExtensionTest
    {
        [Fact]
        public void GivenAnObjectWeCreateAnIEnumerableWithYield()
        {
            AcceptIntegers(42.Yield());

            Unit unit;
            AcceptUnits(unit.Yield());
        }

        [Fact]
        public void YieldReturnsEmptyEnumerableWhenReferenceIsNull()
        {
            string? value = null;
            Assert.Empty(value.Yield());
        }

        [Fact]
        public void YieldReturnsEnumerableWithOneValueWhenReferenceIsNotNull()
        {
            const string value = "foo";
            Assert.Single(value.Yield(), value);
        }

        [Fact]
        public void YieldReturnsEmptyEnumerableWhenNullableValueTypeIsNull()
        {
            int? value = null;
            Assert.Empty(value.Yield());
        }

        [Fact]
        public void YieldReturnsEnumerableWithOneValueWhenNullableValueTypeIsNotNull()
        {
            int? value = 10;
            Assert.Single(value.Yield(), value);
        }

        [Fact]
        public void YieldReturnsEnumerableWithOneValueWhenValueTypeIsNotNull()
        {
            const int value = 10;
            Assert.Single(value.Yield(), value);
        }

        [Fact]
        public void GivenAnEnumerableAndInjectWeCanApplySideffectsToEnumberables()
        {
            var sideEffect = 0;
            var numbers = new List<int> { 1, 2, 3, 42 };

            var numbersWithSideEffect = numbers
                .Inspect(n => { ++sideEffect; });

            Assert.Equal(0, sideEffect);

            numbersWithSideEffect.Each(n => { });

            Assert.Equal(numbers.Count, sideEffect);
        }

        [Fact]
        public void WhereSelectFiltersEmptyValues()
        {
            var input = Enumerable.Range(0, 10);
            var expectedResult = new[] { 0, 4, 16, 36, 64 };
            var result = input.WhereSelect(SquareEvenNumbers);
            Assert.Equal(expectedResult, result);
        }

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

        [Fact]
        public void WhereNotNullRemovesNullReferenceValues()
        {
            var input = new[]
            {
                null,
                "foo",
                null,
                "bar",
                null,
            };
            var expectedResult = new[]
            {
                "foo",
                "bar",
            };
            Assert.Equal(expectedResult, input.WhereNotNull());
        }

        [Fact]
        public void WhereNotNullRemovesNullValueTypeValues()
        {
            var input = new int?[]
            {
                null,
                10,
                null,
                20,
                null,
            };
            var expectedResult = new[]
            {
                10,
                20,
            };
            Assert.Equal(expectedResult, input.WhereNotNull());
        }

        public static TheoryData<List<int>, List<string>> ValueReferenceEnumerables()
            => new TheoryData<List<int>, List<string>>
            {
                { new List<int>(), new List<string>() },
                { new List<int> { 1 }, new List<string> { "a" } },
                { new List<int> { 1, 2, 3 }, new List<string> { "a", "b", "c" } },
            };

        private static Option<int> SquareEvenNumbers(int number)
            => IsEven(number) ? Option.Some(number * number) : Option<int>.None();

        private static bool IsEven(int number) => number % 2 == 0;

        private static void AcceptIntegers(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Assert.Equal(42, value);
            }
        }

        private static void AcceptUnits(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                Assert.Equal(default, unit);
            }
        }

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
