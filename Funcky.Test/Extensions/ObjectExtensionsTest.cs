using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public class ObjectExtensionsTest
    {
        [Fact]
        public void GivenAnObjectWeCreateAnIEnumerableWithToEnumerable()
        {
            AcceptIntegers(42.ToEnumerable());

            Unit unit;
            AcceptUnits(unit.ToEnumerable());
        }

        [Fact]
        public void ToEnumerableReturnsEmptyEnumerableWhenReferenceIsNull()
        {
            string? value = null;
            Assert.Empty(value.ToEnumerable());
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenReferenceIsNotNull()
        {
            const string value = "foo";
            Assert.Single(value.ToEnumerable(), value);
        }

        [Fact]
        public void ToEnumerableReturnsEmptyEnumerableWhenNullableValueTypeIsNull()
        {
            int? value = null;
            Assert.Empty(value.ToEnumerable());
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenNullableValueTypeIsNotNull()
        {
            int? value = 10;
            Assert.Single(value.ToEnumerable(), value);
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneValueWhenValueTypeIsNotNull()
        {
            const int value = 10;
            Assert.Single(value.ToEnumerable(), value);
        }

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
    }
}
