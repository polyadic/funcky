using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class AverageOrNoneTest
    {
        // Int32/int Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt32AverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<int>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt32AverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameInt32ValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1337, 42);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt32sAverageOrNoneComputesTheAverage()
        {
            var numbers = new List<int> { 1, 42, 9999, 5, 1337, -13, -1, 0, -1000, 17 };

            FunctionalAssert.IsSome(1038.7, numbers.AverageOrNone());
            Assert.Equal(1038.7, numbers.Average());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt32AverageOrNoneComputesTheAverage()
        {
            var numbers = Enumerable.Empty<Option<int>>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt32AverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42), 1);

            FunctionalAssert.IsSome(42, numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sWhereAllValuesAreNoneAverageOrNoneComputesNone()
        {
            var numbers = new List<Option<int>> { Option<int>.None(), Option<int>.None(), Option<int>.None() };

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt32sMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<int>> { 1, 42, Option<int>.None(), 9999, 5, Option<int>.None(), Option<int>.None(), 1337, -13, -1, 0, -1000, 555, Option<int>.None() };

            FunctionalAssert.IsSome(1092.5, numbers.AverageOrNone());
            Assert.Equal(1092.5, numbers.WhereSelect(Identity).Average());
        }

        // Int64/long Tests
        [Fact]
        public void GivenAnEmptySequenceOfInt64AverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<long>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleInt64AverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42L, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameInt64ValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1337L, 42);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfInt64sAverageOrNoneComputesTheAverage()
        {
            var numbers = new List<long> { 1L, 42L, 9999L, 5L, 1337L, -13L, -1L, 0L, -1000L, 17L };

            FunctionalAssert.IsSome(1038.7, numbers.AverageOrNone());
            Assert.Equal(1038.7, numbers.Average());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionInt64AverageOrNoneComputesTheAverage()
        {
            var numbers = Enumerable.Empty<Option<long>>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionInt64AverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42L), 1);

            FunctionalAssert.IsSome(42, numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sWhereAllValuesAreNoneAverageOrNoneComputesNone()
        {
            var numbers = new List<Option<long>> { Option<long>.None(), Option<long>.None(), Option<long>.None() };

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionInt64sMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<long>> { 1L, 42L, Option<long>.None(), 9999L, 5L, Option<long>.None(), Option<long>.None(), 1337L, -13L, -1L, 0L, -1000L, 555L, Option<long>.None() };

            FunctionalAssert.IsSome(1092.5, numbers.AverageOrNone());
            Assert.Equal(1092.5, numbers.WhereSelect(Identity).Average());
        }

        // Single/float Tests
        [Fact]
        public void GivenAnEmptySequenceOfSinglesAverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<float>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleSinglesAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.42f, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameSingleValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1.4f, 42);

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(numbers.First(), average, 3);
        }

        [Fact]
        public void GivenASequenceOfSinglesAverageOrNoneComputesTheAverage()
        {
            var numbers = new List<float> { 1.7f, 42.42f, 9999.01f, 5f, 1337f, -13f, -1f, 0.0f, -1000.5f, 17.25f };

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1038.788, average, 3);
            Assert.Equal(1038.788, numbers.Average(), 3);
            Assert.Equal(numbers.Average(), average);
        }

        [Fact]
        public void GivenASequenceOfSinglesWithInifinityAverageOrNoneComputesInfinity()
        {
            var numbers = new List<float> { 1.7f, float.PositiveInfinity, 42.42f };

            FunctionalAssert.IsSome(float.PositiveInfinity, numbers.AverageOrNone());
            Assert.Equal(float.PositiveInfinity, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfSinglesWithNegativeInifinityAverageOrNoneComputesNegativeInfinity()
        {
            var numbers = new List<float> { 1.7f, float.NegativeInfinity, 42.42f };

            FunctionalAssert.IsSome(float.NegativeInfinity, numbers.AverageOrNone());
            Assert.Equal(float.NegativeInfinity, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfSinglesWithBothInifinitiesAverageOrNoneComputesNaN()
        {
            var numbers = new List<float> { float.NegativeInfinity, 1.7f, float.PositiveInfinity, 42.42f };

            FunctionalAssert.IsSome(float.NaN, numbers.AverageOrNone());
            Assert.Equal(float.NaN, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfSinglesWithNaNAverageOrNoneComputesNaN()
        {
            var numbers = new List<float> { 1.7f, float.NaN, 42.42f };

            FunctionalAssert.IsSome(float.NaN, numbers.AverageOrNone());
            Assert.Equal(float.NaN, numbers.Average());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionalSinglesAverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<float>>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAOptionalSinglesSingleAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42f), 1);

            FunctionalAssert.IsSome(42.42f, numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameOptionalSingleValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1.4f, 42);

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1.4f, average, 3);
        }

        [Fact]
        public void GivenASequenceOfOptionalSinglesAverageOrNoneComputesTheAverageIgnoingTheNones()
        {
            var numbers = new List<Option<float>> { Option<float>.None(), 1.7f, 42.42f, 9999.01f, 5f, Option<float>.None(), 1337f, -13f, Option<float>.None(), -1f, 0.0f, -1000.5f, 17.25f, Option<float>.None(), Option<float>.None() };

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1038.788, average, 3);
            Assert.Equal(1038.788, numbers.WhereSelect(Identity).Average(), 3);
            Assert.Equal(numbers.WhereSelect(Identity).Average(), average);
        }

        // Double/double Tests
        [Fact]
        public void GivenAnEmptySequenceOfDoublesAverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<double>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleDoubleAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.42, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameDoubleValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1.4, 42);

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(numbers.First(), average, 3);
        }

        [Fact]
        public void GivenASequenceOfDoublesAverageOrNoneComputesTheAverage()
        {
            var numbers = new List<double> { 1.7, 42.42, 9999.0001, 5, 1337, -13, -1, 0.0, -1000.5, 17.2525 };

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1038.787, average, 3);
            Assert.Equal(1038.787, numbers.Average(), 3);
            Assert.Equal(numbers.Average(), average);
        }

        [Fact]
        public void GivenASequenceOfDoublesWithInifinityAverageOrNoneComputesInfinity()
        {
            var numbers = new List<double> { 1.7, double.PositiveInfinity, 42.42 };

            FunctionalAssert.IsSome(double.PositiveInfinity, numbers.AverageOrNone());
            Assert.Equal(double.PositiveInfinity, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfDoublesWithNegativeInifinityAverageOrNoneComputesNegativeInfinity()
        {
            var numbers = new List<double> { 1.7, double.NegativeInfinity, 42.42 };

            FunctionalAssert.IsSome(double.NegativeInfinity, numbers.AverageOrNone());
            Assert.Equal(double.NegativeInfinity, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfDoublesWithBothInifinitiesAverageOrNoneComputesNaN()
        {
            var numbers = new List<double> { double.NegativeInfinity, 1.7, double.PositiveInfinity, 42.42 };

            FunctionalAssert.IsSome(double.NaN, numbers.AverageOrNone());
            Assert.Equal(double.NaN, numbers.Average());
        }

        [Fact]
        public void GivenASequenceOfDoublesWithNaNAverageOrNoneComputesNaN()
        {
            var numbers = new List<double> { 1.7, double.NaN, 42.42 };

            FunctionalAssert.IsSome(double.NaN, numbers.AverageOrNone());
            Assert.Equal(double.NaN, numbers.Average());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionalDoublesAverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<Option<double>>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAOptionalSingleDoubleAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42.42), 1);

            FunctionalAssert.IsSome(42.42, numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameOptionalDoubleValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1.4, 42);

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1.4, average, 3);
        }

        [Fact]
        public void GivenASequenceOfOptionalDoublesAverageOrNoneComputesTheAverageIgnoingTheNones()
        {
            var numbers = new List<Option<double>> { Option<double>.None(), 1.7, 42.4242, 9999.0001, 5d, Option<double>.None(), 1337d, -13d, Option<double>.None(), -1d, 0.0, -1000.5, 17.2525, Option<double>.None(), Option<double>.None() };

            var average = FunctionalAssert.IsSome(numbers.AverageOrNone());

            // floating point equal
            Assert.Equal(1038.788, average, 3);
            Assert.Equal(1038.788, numbers.WhereSelect(Identity).Average(), 3);
            Assert.Equal(numbers.WhereSelect(Identity).Average(), average);
        }

        // decimal
        [Fact]
        public void GivenAnEmptySequenceOfDecimalAverageOrNoneReturnsNone()
        {
            var numbers = Enumerable.Empty<int>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleDecimalAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(42.42m, 1);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceWithAllTheSameDecimalValueAverageOrNoneReturnsTheConstant()
        {
            var numbers = Enumerable.Repeat(1337.4242m, 42);

            FunctionalAssert.IsSome(numbers.First(), numbers.MinOrNone());
        }

        [Fact]
        public void GivenASequenceOfDecimalsAverageOrNoneComputesTheAverage()
        {
            var numbers = new List<decimal> { 1.001m, 42.42m, 9999.9m, 5m, 1337m, -13, -1, 0, -1000.01m, 17.075m };

            FunctionalAssert.IsSome(1038.8386m, numbers.AverageOrNone());
            Assert.Equal(1038.8386m, numbers.Average());
        }

        [Fact]
        public void GivenAnEmptySequenceOfOptionDecimalAverageOrNoneComputesTheAverage()
        {
            var numbers = Enumerable.Empty<Option<decimal>>();

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceWithASingleOptionDecimalAverageOrNoneReturnsTheSingleElement()
        {
            var numbers = Enumerable.Repeat(Option.Some(42), 1);

            FunctionalAssert.IsSome(42, numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsWhereAllValuesAreNoneAverageOrNoneComputesNone()
        {
            var numbers = new List<Option<decimal>> { Option<decimal>.None(), Option<decimal>.None(), Option<decimal>.None() };

            FunctionalAssert.IsNone(numbers.AverageOrNone());
        }

        [Fact]
        public void GivenASequenceOfOptionDecimalsMinOrNoneComputesTheMinIgnoringTheNones()
        {
            var numbers = new List<Option<decimal>> { 1.01m, 42.42m, Option<decimal>.None(), 9999.9m, 4.95m, Option<decimal>.None(), Option<decimal>.None(), 1337m, -13m, -1m, 0.0m, -1000.3m, 555.5m, Option<decimal>.None() };

            FunctionalAssert.IsSome(1092.648m, numbers.AverageOrNone());
            Assert.Equal(1092.648m, numbers.WhereSelect(Identity).Average());
        }
    }
}
