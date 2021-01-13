using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class MinOrNoneTest
    {
        // Int32/int Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt32MinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<int>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt32MinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt32sMinOrNoneComputesTheMin()
        {
            var numbers = new List<int> { 1, 42, 9999, 5, 1337, -13, -1, 0, -1000 };

            FunctionalAssert.IsSome(-1000, numbers.MinOrNone());
            Assert.Equal(-1000, numbers.Min());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt32MinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<int>>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt32MinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42), 1);

            FunctionalAssert.IsSome(42, numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var numbers = new List<Option<int>> { Option<int>.None(), Option<int>.None(), Option<int>.None() };

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<int>> { 1, 42, Option<int>.None(), 9999, 5, Option<int>.None(), Option<int>.None(), 1337, -13, -1, 0, -1000, Option<int>.None() };

            FunctionalAssert.IsSome(-1000, numbers.MinOrNone());
            Assert.Equal(-1000, numbers.WhereSelect(Identity).Min());
        }

        // Int64/long Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt64MinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<long>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt64MinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42L, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt64sMinOrNoneComputesTheMin()
        {
            var numbers = new List<long> { 1L, 42L, 9999L, 5L, 1337L, -13L, -1L, 0L, -1000L };

            FunctionalAssert.IsSome(-1000L, numbers.MinOrNone());
            Assert.Equal(-1000L, numbers.Min());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt64MinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<long>>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt64MinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42L), 1);

            FunctionalAssert.IsSome(42L, numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var numbers = new List<Option<long>> { Option<long>.None(), Option<long>.None(), Option<long>.None() };

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<long>> { 1L, 42L, Option<long>.None(), 9999L, 5L, Option<long>.None(), Option<long>.None(), 1337L, -13L, -1L, 0L, -1000L, Option<long>.None() };

            FunctionalAssert.IsSome(-1000L, numbers.MinOrNone());
            Assert.Equal(-1000L, numbers.WhereSelect(Identity).Min());
        }

        // Single/float Tests
        [Fact]
        public void GivenAnEmptySequenceOfSinglesMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<float>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASinglesSingleMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0f, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfSinglesMinOrNoneComputesTheMin()
        {
            var numbers = new List<float> { 1.1f, 42.0004f, 9999.001f, 5f, 1337.1337f, -13f, -1.0f, 0f, -1000f };

            FunctionalAssert.IsSome(-1000f, numbers.MinOrNone());
            Assert.Equal(-1000f, numbers.Min());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionSinglesMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<float>>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionSinglesMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42f), 1);

            FunctionalAssert.IsSome(42.42f, numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionSinglesWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var numbers = new List<Option<float>> { Option<float>.None(), Option<float>.None(), Option<float>.None() };

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionSinglesMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<float>> { 1.8f, 42.52f, Option<float>.None(), 9999.0001f, 5f, Option<float>.None(), Option<float>.None(), 1337.1337f, -13f, -1f, -0f, -1000.45f, Option<float>.None() };

            FunctionalAssert.IsSome(-1000.45f, numbers.MinOrNone());
            Assert.Equal(-1000.45f, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void PlusZeroAndMinusZeroAreEqualForSingles()
        {
            var numbers = new List<Option<float>> { +0.0f, -0.0f, };

            FunctionalAssert.IsSome(+0.0f, numbers.MinOrNone());
            FunctionalAssert.IsSome(-0.0f, numbers.MinOrNone());
            Assert.Equal(+0.0f, -0.0f);
        }

        [Fact]
        public void InSequenceOfSinglesWithANegativeInfinityAndNoNaNTheMinReturnsNegativeInfinity()
        {
            var numbers = new List<Option<float>> { float.NegativeInfinity, 1.8f, 42.52f, float.PositiveInfinity };

            FunctionalAssert.IsSome(float.NegativeInfinity, numbers.MinOrNone());
            Assert.Equal(float.NegativeInfinity, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void GivenASequenceOfSinglesWithAnNaNTheMinReturnsNaN()
        {
            // We impose a total order where NaN is smaller than NegativeInfinity (same behaviour as dotnet)
            var numbers = new List<Option<float>> { 1.8f, 42.52f, float.NaN, 42f };

            FunctionalAssert.IsSome(float.NaN, numbers.MinOrNone());
            Assert.Equal(float.NaN, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void GivenASequenceOfOnlySingleNaNsReturnsNaN()
        {
            var numbers = new List<float> { float.NaN, float.NaN };

            FunctionalAssert.IsSome(float.NaN, numbers.MinOrNone());
            Assert.Equal(float.NaN, numbers.Min());
        }

        // Double/double Tests
        [Fact]
        public void GivenAnEmptySequenceOfDoublesMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<double>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithADoublesSingleMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfDoublesMinOrNoneComputesTheMin()
        {
            var numbers = new List<double> { 1.1, 42.0004, 9999.001, 5, 1337.1337, -13, -1, 0, -1000.001 };

            FunctionalAssert.IsSome(-1000.001, numbers.MinOrNone());
            Assert.Equal(-1000.001, numbers.Min());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionDoublesMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<double>>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionDoublesMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42), 1);

            FunctionalAssert.IsSome(42.42, numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDoublesWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var numbers = new List<Option<double>> { Option<double>.None(), Option<double>.None(), Option<double>.None() };

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDoublesMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<double>> { 1.8, 42.52, Option<double>.None(), 9999.0001, 5d, Option<double>.None(), Option<double>.None(), 1337.1337, -13d, -1d, -0d, -1000.45, Option<double>.None() };

            FunctionalAssert.IsSome(-1000.45, numbers.MinOrNone());
            Assert.Equal(-1000.45, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void PlusZeroAndMinusZeroAreEqualForDoubles()
        {
            var numbers = new List<Option<double>> { +0.0, -0.0, };

            FunctionalAssert.IsSome(+0.0, numbers.MinOrNone());
            FunctionalAssert.IsSome(-0.0, numbers.MinOrNone());
            Assert.Equal(+0.0, -0.0);
        }

        [Fact]
        public void InASequenceOfDoublesWithANegativeInfinityAndNoNaNMinReturnsNegativeInfinity()
        {
            var numbers = new List<Option<double>> { double.NegativeInfinity, 1.8, 42.52, double.PositiveInfinity };

            FunctionalAssert.IsSome(double.NegativeInfinity, numbers.MinOrNone());
            Assert.Equal(double.NegativeInfinity, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void GivenASequenceOfDoublesWithAnNaNTheMinReturnsNaN()
        {
            // We impose a total order where NaN is smaller than NegativeInfinity (same behaviour as dotnet)
            var numbers = new List<Option<double>> { double.NegativeInfinity, 1.8, 42.52, double.NaN };

            FunctionalAssert.IsSome(double.NaN, numbers.MinOrNone());
            Assert.Equal(double.NaN, numbers.WhereSelect(Identity).Min());
        }

        [Fact]
        public void GivenASequenceOfOnlyDoubleNaNsReturnsNaN()
        {
            var numbers = new List<double> { double.NaN, double.NaN };

            FunctionalAssert.IsSome(double.NaN, numbers.MinOrNone());
            Assert.Equal(double.NaN, numbers.Min());
        }

        // Decimal/decimal Tests
        [Fact]
        public void GivenAnEmptySequenceOfDecimalsMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<decimal>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithADecimalsSingleMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0m, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfDecimalsMinOrNoneComputesTheMin()
        {
            var numbers = new List<decimal> { 1.1m, 42.0004m, 9999.001m, 5, 1337.1337m, -13, -1, 0, -1000.001m };

            FunctionalAssert.IsSome(-1000.001m, numbers.MinOrNone());
            Assert.Equal(-1000.001m, numbers.Min());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionDecimalsMinOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<decimal>>();

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionDecimalsMinOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42m), 1);

            FunctionalAssert.IsSome(42.42m, numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var numbers = new List<Option<decimal>> { Option<decimal>.None(), Option<decimal>.None(), Option<decimal>.None() };

            FunctionalAssert.IsNone(numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<decimal>> { 1.8m, 42.52m, Option<decimal>.None(), 9999.0001m, 5m, Option<decimal>.None(), Option<decimal>.None(), 1337.1337m, -13m, -1m, -0m, -1000.45m, Option<decimal>.None() };

            FunctionalAssert.IsSome(-1000.45m, numbers.MinOrNone());
            Assert.Equal(-1000.45m, numbers.WhereSelect(Identity).Min());
        }

        // Generic TSource implementing IComparable Tests
        [Fact]
        public void GivenAnEmptySequenceOfAGenericIComparableMinOrNoneReturnsNone()
        {
            var persons = Enumerable.Empty<Person>();

            FunctionalAssert.IsNone(persons.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleGenericIComparableMinOrNoneReturnsTheSingleElement()
        {
            var persons = Enumerable.Repeat(new Person(42), 1);

            FunctionalAssert.IsSome(persons.First(), persons.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfGenericIComparablesMinOrNoneComputesTheMin()
        {
            var persons = new List<Person> { new (42), new (18), new (72), new (33) };

            var person = FunctionalAssert.IsSome(persons.MinOrNone());
            Assert.Equal(person.Age, persons.Min()?.Age);
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionGenericIComparablesMinOrNoneReturnsNone()
        {
            var persons = Enumerable.Empty<Option<Person>>();

            FunctionalAssert.IsNone(persons.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionGenericIComparablesMinOrNoneReturnsTheSingleElement()
        {
            var persons = Enumerable.Repeat(Option.Some(new Person(42)), 1);

            var person = FunctionalAssert.IsSome(persons.MinOrNone());
            Assert.Equal(42, person.Age);
        }

        [Fact]
        public void GivenASequenceOfOptionGenericIComparablesWhereAllValuesAreNoneMinOrNoneComputesNone()
        {
            var persons = new List<Option<Person>> { Option<Person>.None(), Option<Person>.None(), Option<Person>.None() };

            FunctionalAssert.IsNone(persons.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionGenericIComparablesMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var persons = new List<Option<Person>> { new Person(42), new Person(18), Option<Person>.None(), new Person(72), new Person(33), Option<Person>.None(), Option<Person>.None(), new Person(21), Option<Person>.None() };

            var person = FunctionalAssert.IsSome(persons.MinOrNone());
            Assert.Equal(person.Age, persons.WhereSelect(Identity).Min()?.Age);
        }
    }
}
