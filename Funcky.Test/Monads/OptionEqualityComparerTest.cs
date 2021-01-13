using System;
using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Monads
{
    public sealed class OptionEqualityComparerTest
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

        [Property]
        public Property SomeAndSomeAreEqualWhenTheItemsAreEqual(int x, int y)
            => OptionEqualityComparer.Create(new ConstantEqualityComparer<int>(areEqual: true))
                .Equals(Option.Some(x), Option.Some(y))
                .ToProperty();

        [Property]
        public Property SomeAndSomeAreNotEqualWhenTheItemsAreNotEqual(int x, int y)
            => (!OptionEqualityComparer.Create(new ConstantEqualityComparer<int>(areEqual: false))
                .Equals(Option.Some(x), Option.Some(y)))
                .ToProperty();

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
