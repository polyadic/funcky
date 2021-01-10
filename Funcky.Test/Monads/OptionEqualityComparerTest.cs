using System;
using System.Collections.Generic;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Monads
{
    public class OptionEqualityComparerTest
    {
        [Fact]
        public void TwoNoneOptionsAreEqual()
        {
            Assert.Equal(Option<int>.None(), Option<int>.None(), OptionEqualityComparer.Create(new ThrowingEqualityComparer<int>()));
        }

        [Theory]
        [MemberData(nameof(SomeAndNoneAreNotEqualData))]
        public void SomeAndNoneAreNotEqual(Option<int> x, Option<int> y)
        {
            Assert.NotEqual(x, y, OptionEqualityComparer.Create(new ThrowingEqualityComparer<int>()));
        }

        public static TheoryData<Option<int>, Option<int>> SomeAndNoneAreNotEqualData()
            => new ()
            {
                { Option<int>.None(), Option.Some(13) },
                { Option.Some(13), Option<int>.None() },
                { Option<int>.None(), Option.Some(default(int)) },
                { Option.Some(default(int)), Option<int>.None() },
            };

        [Fact]
        public void GetHashCodePropagatesToTheItem()
        {
            Assert.Equal("foo".GetHashCode(), Option.Some("foo").GetHashCode());
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(10, 20)]
        public void SomeAndSomeAreEqualWhenTheItemsAreEqual(int x, int y)
        {
            Assert.Equal(Option.Some(x), Option.Some(y), OptionEqualityComparer.Create(new ConstantEqualityComparer<int>(areEqual: true)));
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(10, 20)]
        public void SomeAndSomeAreNotEqualWhenTheItemsAreNotEqual(int x, int y)
        {
            Assert.NotEqual(Option.Some(x), Option.Some(y), OptionEqualityComparer.Create(new ConstantEqualityComparer<int>(areEqual: false)));
        }

        private sealed class ThrowingEqualityComparer<T> : EqualityComparer<T>
        {
            public override bool Equals(T? x, T? y) => throw new NotSupportedException();

            public override int GetHashCode(T obj) => throw new NotSupportedException();
        }

        private sealed class ConstantEqualityComparer<T> : EqualityComparer<T>
        {
            private readonly bool _areEqual;

            public ConstantEqualityComparer(bool areEqual) => _areEqual = areEqual;

            public override bool Equals(T? x, T? y) => _areEqual;

            public override int GetHashCode(T obj) => throw new NotSupportedException();
        }
    }
}
