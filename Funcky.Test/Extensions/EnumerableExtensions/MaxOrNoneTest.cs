using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class MaxOrNoneTest
    {
        // Int32/int Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt32MaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<int>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt32MaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt32sMaxOrNoneComputesTheMax()
        {
            var numbers = new List<int> { 1, 42, 9999, 5, 1337, -13, -1, 0, -1000 };

            FunctionalAssert.IsSome(9999, numbers.MaxOrNone());
            Assert.Equal(9999, numbers.Max());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt32MaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<int>>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt32MaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42), 1);

            FunctionalAssert.IsSome(42, numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var numbers = new List<Option<int>> { Option<int>.None(), Option<int>.None(), Option<int>.None() };

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var numbers = new List<Option<int>> { Option.Some(1), Option.Some(42), Option<int>.None(), Option.Some(9999), Option.Some(5), Option<int>.None(), Option<int>.None(), Option.Some(1337), Option.Some(-13), Option.Some(-1), Option.Some(0), Option.Some(-1000), Option<int>.None() };

            FunctionalAssert.IsSome(9999, numbers.MaxOrNone());
            Assert.Equal(9999, numbers.WhereSelect(Identity).Max());
        }

        // Int64/long Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt64MaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<long>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt64MaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42L, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt64sMaxOrNoneComputesTheMax()
        {
            var numbers = new List<long> { 1L, 42L, 9999L, 5L, 1337L, -13L, -1L, 0L, -1000L };

            FunctionalAssert.IsSome(9999L, numbers.MaxOrNone());
            Assert.Equal(9999L, numbers.Max());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt64MaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<long>>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt64MaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42L), 1);

            FunctionalAssert.IsSome(42L, numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var numbers = new List<Option<long>> { Option<long>.None(), Option<long>.None(), Option<long>.None() };

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var numbers = new List<Option<long>> { Option.Some(1L), Option.Some(42L), Option<long>.None(), Option.Some(9999L), Option.Some(5L), Option<long>.None(), Option<long>.None(), Option.Some(1337L), Option.Some(-13L), Option.Some(-1L), Option.Some(0L), Option.Some(-1000L), Option<long>.None() };

            FunctionalAssert.IsSome(9999L, numbers.MaxOrNone());
            Assert.Equal(9999L, numbers.WhereSelect(Identity).Max());
        }

        // Single/float Tests
        [Fact]
        public void GivenAnEmptySequenceOfSinglesMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<float>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleSingleMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0f, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfSinglesMaxOrNoneComputesTheMax()
        {
            var numbers = new List<float> { 1.1f, 42.0004f, 9999.001f, 5f, 1337.1337f, -13f, -1.0f, 0f, -1000f };

            FunctionalAssert.IsSome(9999.001f, numbers.MaxOrNone());
            Assert.Equal(9999.001f, numbers.Max());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionSinglesMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<float>>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionSinglesMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42f), 1);

            FunctionalAssert.IsSome(42.42f, numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionSinglesWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var numbers = new List<Option<float>> { Option<float>.None(), Option<float>.None(), Option<float>.None() };

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionSinglesMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var numbers = new List<Option<float>> { Option.Some(1.8f), Option.Some(42.52f), Option<float>.None(), Option.Some(9999.0001f), Option.Some(5f), Option<float>.None(), Option<float>.None(), Option.Some(1337.1337f), Option.Some(-13f), Option.Some(-1f), Option.Some(-0f), Option.Some(-1000.45f), Option<float>.None() };

            FunctionalAssert.IsSome(9999.0001f, numbers.MaxOrNone());
            Assert.Equal(9999, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void PlusZeroAndMinusZeroAreEqualForSingles()
        {
            var numbers = new List<Option<float>> { Option.Some(+0.0f), Option.Some(-0.0f), };

            FunctionalAssert.IsSome(+0.0f, numbers.MaxOrNone());
            FunctionalAssert.IsSome(-0.0f, numbers.MaxOrNone());
            Assert.Equal(+0.0f, -0.0f);
        }

        [Fact]
        public void InSequenceOfSinglesWithAPositiveInfinityTheMaxReturnsPositiveInfinity()
        {
            var numbers = new List<Option<float>> { Option.Some(float.NegativeInfinity), Option.Some(1.8f), Option.Some(42.52f), Option.Some(float.PositiveInfinity) };

            FunctionalAssert.IsSome(float.PositiveInfinity, numbers.MaxOrNone());
            Assert.Equal(float.PositiveInfinity, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void GivenASequenceOfSinglesWithAnNaNAndNegativeInfinityReturnsNaN()
        {
            var numbers = new List<Option<float>> { Option.Some(float.NegativeInfinity), Option.Some(1.8f), Option.Some(42.52f), Option.Some(float.NaN) };

            FunctionalAssert.IsSome(42.52f, numbers.MaxOrNone());
            Assert.Equal(42.52f, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void GivenASequenceOfOnlySingleNaNsReturnsNaN()
        {
            var numbers = new List<float> { float.NaN, float.NaN };

            FunctionalAssert.IsSome(float.NaN, numbers.MaxOrNone());
            Assert.Equal(float.NaN, numbers.Max());
        }

        // Double/double Tests
        [Fact]
        public void GivenAnEmptySequenceOfDoublesMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<double>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleDoubleMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfDoublesMaxOrNoneComputesTheMax()
        {
            var numbers = new List<double> { 1.1, 42.0004, 9999.001, 5, 1337.1337, -13, -1, 0, -1000.001 };

            FunctionalAssert.IsSome(9999.001, numbers.MaxOrNone());
            Assert.Equal(9999.001, numbers.Max());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionDoublesMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<double>>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionDoublesMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42), 1);

            FunctionalAssert.IsSome(42.42, numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDoublesWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var numbers = new List<Option<double>> { Option<double>.None(), Option<double>.None(), Option<double>.None() };

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDoublesMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var numbers = new List<Option<double>> { Option.Some(1.8), Option.Some(42.52), Option<double>.None(), Option.Some(9999.0001), Option.Some(5d), Option<double>.None(), Option<double>.None(), Option.Some(1337.1337), Option.Some(-13d), Option.Some(-1d), Option.Some(-0d), Option.Some(-1000.45), Option<double>.None() };

            FunctionalAssert.IsSome(9999.0001, numbers.MaxOrNone());
            Assert.Equal(9999.0001, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void PlusZeroAndMinusZeroAreEqualForDoubles()
        {
            var numbers = new List<Option<double>> { Option.Some(+0.0), Option.Some(-0.0), };

            FunctionalAssert.IsSome(+0.0, numbers.MaxOrNone());
            FunctionalAssert.IsSome(-0.0, numbers.MaxOrNone());
            Assert.Equal(+0.0, -0.0);
        }

        [Fact]
        public void InASequenceOfDoublesWithAPositiveInfinityTheMaxReturnsPositiveInfinity()
        {
            var numbers = new List<Option<double>> { Option.Some(double.NegativeInfinity), Option.Some(1.8), Option.Some(42.52), Option.Some(double.PositiveInfinity) };

            FunctionalAssert.IsSome(double.PositiveInfinity, numbers.MaxOrNone());
            Assert.Equal(double.PositiveInfinity, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void GivenASequenceOfDoublesWithAnNaNMaxOrNoneDoesNotReturnNaN()
        {
            var numbers = new List<Option<double>> { Option.Some(double.NegativeInfinity), Option.Some(1.8), Option.Some(42.52), Option.Some(double.NaN) };

            FunctionalAssert.IsSome(42.52, numbers.MaxOrNone());
            Assert.Equal(42.52, numbers.WhereSelect(Identity).Max());
        }

        [Fact]
        public void GivenASequenceOfOnlyDoubleNaNsReturnsNaN()
        {
            var numbers = new List<double> { double.NaN, double.NaN };

            FunctionalAssert.IsSome(double.NaN, numbers.MaxOrNone());
            Assert.Equal(double.NaN, numbers.Max());
        }

        // Decimal/decimal Tests
        [Fact]
        public void GivenAnEmptySequenceOfDecimalsMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<decimal>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleDecimalMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.0m, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfDecimalsMaxOrNoneComputesTheMax()
        {
            var numbers = new List<decimal> { 1.1m, 42.0004m, 9999.001m, 5, 1337.1337m, -13, -1, 0, -1000.001m };

            FunctionalAssert.IsSome(9999.001m, numbers.MaxOrNone());
            Assert.Equal(9999.001m, numbers.Max());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionDecimalsMaxOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<decimal>>();

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionDecimalsMaxOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42m), 1);

            FunctionalAssert.IsSome(42.42m, numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var numbers = new List<Option<decimal>> { Option<decimal>.None(), Option<decimal>.None(), Option<decimal>.None() };

            FunctionalAssert.IsNone(numbers.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var numbers = new List<Option<decimal>> { Option.Some(1.8m), Option.Some(42.52m), Option<decimal>.None(), Option.Some(9999.0001m), Option.Some(5m), Option<decimal>.None(), Option<decimal>.None(), Option.Some(1337.1337m), Option.Some(-13m), Option.Some(-1m), Option.Some(-0m), Option.Some(-1000.45m), Option<decimal>.None() };

            FunctionalAssert.IsSome(9999.0001m, numbers.MaxOrNone());
            Assert.Equal(9999.0001m, numbers.WhereSelect(Identity).Max());
        }

        // Generic TSource implementing IComparable Tests
        [Fact]
        public void GivenAnEmptySequenceOfAGenericIComparableMaxOrNoneReturnsNone()
        {
            var persons = Enumerable.Empty<Person>();

            FunctionalAssert.IsNone(persons.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleGenericIComparableMaxOrNoneReturnsTheSingleElement()
        {
            var persons = Enumerable.Repeat(new Person(42), 1);

            FunctionalAssert.IsSome(persons.First(), persons.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfGenericIComparablesMaxOrNoneComputesTheMax()
        {
            var persons = new List<Person> { new Person(42), new Person(18), new Person(72), new Person(33) };

            var person = FunctionalAssert.IsSome(persons.MaxOrNone());
            Assert.Equal(person.Age, persons.Max()?.Age);
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionGenericIComparablesMaxOrNoneReturnsNone()
        {
            var persons = Enumerable.Empty<Option<Person>>();

            FunctionalAssert.IsNone(persons.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionGenericIComparablesMaxOrNoneReturnsTheSingleElement()
        {
            var persons = Enumerable.Repeat(Option.Some(new Person(42)), 1);

            var person = FunctionalAssert.IsSome(persons.MaxOrNone());
            Assert.Equal(42, person.Age);
        }

        [Fact]
        public void GivenASequenceOfOptionGenericIComparablesWhereAllValuesAreNoneMaxOrNoneComputesNone()
        {
            var persons = new List<Option<Person>> { Option<Person>.None(), Option<Person>.None(), Option<Person>.None() };

            FunctionalAssert.IsNone(persons.MaxOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionGenericIComparablesMaxOrNoneComputesTheMaxIgnoringTheNones()
        {
            var persons = new List<Option<Person>> { Option.Some(new Person(42)), Option.Some(new Person(18)), Option<Person>.None(), Option.Some(new Person(72)), Option.Some(new Person(33)), Option<Person>.None(), Option<Person>.None(), Option.Some(new Person(21)), Option<Person>.None() };

            var person = FunctionalAssert.IsSome(persons.MaxOrNone());
            Assert.Equal(person.Age, persons.WhereSelect(Identity).Max()?.Age);
        }
    }
}
