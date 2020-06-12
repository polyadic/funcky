using System;
using System.IO;
using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public class ResultTest
    {
        [Fact]
        public void CreateResultOkAndMatchCorrectly()
        {
            var value = Result<int>.Ok(1000);

            var hasResult = value.Match(
                ok: True,
                error: False);

            Assert.True(hasResult);
        }

        [Fact]
        public void CreateResultErrorAndMatchCorrectly()
        {
            var value = Result<int>.Error(new ArgumentException());

            var hasResult = value.Match(
                ok: True,
                error: False);

            Assert.False(hasResult);
        }

        [Theory]
        [MemberData(nameof(GetIntegerResults))]
        public void CreateResultOkAndMatchASelectedResult(Result<int> value, double reference)
        {
            var doubleResult = value.Select(i => i * 0.25);

            var result = doubleResult.Match(
                ok: Identity,
                error: y => -1.0);

            Assert.Equal(reference, result);
        }

        public static TheoryData<Result<int>, double> GetIntegerResults()
            => new TheoryData<Result<int>, double>
            {
                { Result<int>.Ok(1000), 250.0 },
                { Result<int>.Ok(44), 11.0 },
                { Result<int>.Ok(1), 0.25 },
                { Result<int>.Ok(1000), 250.0 },
                { Result<int>.Error(new Exception()), -1.0 },
            };

        [Theory]
        [MemberData(nameof(GetIntegerSums))]
        public void TheSumsOverResultTypesShouldBeValid(Result<int> firstValue, Result<int> secondValue, Result<int> thirdValue, Option<int> referenceSum)
        {
            var result =
                from first in firstValue
                from second in secondValue
                from third in thirdValue
                select first + second + third;

            var resultSum = result.Match(
                ok: Option.Some,
                error: _ => Option<int>.None());

            Assert.Equal(referenceSum, resultSum);
        }

        public static TheoryData<Result<int>, Result<int>, Result<int>, Option<int>> GetIntegerSums()
            => new TheoryData<Result<int>, Result<int>, Result<int>, Option<int>>
            {
                { Result<int>.Ok(5), Result<int>.Ok(10), Result<int>.Ok(15), Option.Some(30) },
                { Result<int>.Ok(42), Result<int>.Ok(1337), Result<int>.Error(new InvalidCastException()), Option<int>.None() },
                { Result<int>.Ok(1337), Result<int>.Ok(42), Result<int>.Ok(99), Option.Some(1478) },
                { Result<int>.Ok(45856), Result<int>.Ok(58788), Result<int>.Ok(699554), Option.Some(804198) },
                { Result<int>.Error(new InvalidCastException()), Result<int>.Error(new IOException()), Result<int>.Error(new MemberAccessException()), Option<int>.None() },
            };

        [Theory]
        [MemberData(nameof(GetResults))]
        public void MatchAcceptsActionsAsFunctions(Result<int> result, bool expected)
        {
            result
              .Match(
                ok: v => Assert.True(expected),
                error: e => Assert.False(expected));
        }

        public static TheoryData<Result<int>, bool> GetResults()
            => new TheoryData<Result<int>, bool>
            {
                { Result<int>.Ok(5), true },
                { Result<int>.Ok(42), true },
                { Result<int>.Error(new InvalidCastException()), false },
            };
    }
}
