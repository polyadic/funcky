using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class ResultTest
    {
        [Fact]
        public void CreateResultOkAndMatchCorrectly()
        {
            var value = Result<int>.Ok(1000);

            var hasResult = value.Match(
                ok: x => true,
                error: y => false);

            Assert.True(hasResult);
        }

        [Fact]
        public void CreateResultErrorAndMatchCorrectly()
        {
            var value = Result<int>.Error(new ArgumentException());

            var hasResult = value.Match(
                ok: x => true,
                error: y => false);

            Assert.False(hasResult);
        }

        [Theory]
        [ClassData(typeof(IntegerResults))]
        public void CreateResultOkAndMatchASelectedResult(Result<int> value, double reference)
        {
            Result<double> doubleResult = value.Select(i => i * 0.25);

            var result = doubleResult.Match(
                ok: x => x,
                error: y => -1.0);

            Assert.Equal(reference, result);
        }

        [Theory]
        [ClassData(typeof(IntegerSums))]
        public void TheSumsOverResultTypesShouldBeValid(Result<int> firstValue, Result<int> secondValue, Result<int> thirdValue, int? referenceSum)
        {
            var result =
                from first in firstValue
                from second in secondValue
                from third in thirdValue
                select first + second + third;

            var resultSum = result.Match<int?>(
                ok: x => x,
                error: y => null);

            Assert.Equal(referenceSum, resultSum);
        }

        public class IntegerResults : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { Result<int>.Ok(1000), 250.0 };
                yield return new object[] { Result<int>.Ok(44), 11.0 };
                yield return new object[] { Result<int>.Ok(1), 0.25 };
                yield return new object[] { Result<int>.Ok(1000), 250.0 };
                yield return new object[] { Result<int>.Error(new Exception()), -1.0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class IntegerSums : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { Result<int>.Ok(5), Result<int>.Ok(10), Result<int>.Ok(15), 30 };
                yield return new object[] { Result<int>.Ok(42), Result<int>.Ok(1337), Result<int>.Error(new InvalidCastException()), null };
                yield return new object[] { Result<int>.Ok(1337), Result<int>.Ok(42), Result<int>.Ok(99), 1478 };
                yield return new object[] { Result<int>.Ok(45856), Result<int>.Ok(58788), Result<int>.Ok(699554), 804198 };
                yield return new object[] { Result<int>.Error(new InvalidCastException()), Result<int>.Error(new IOException()), Result<int>.Error(new MemberAccessException()), null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
