#nullable enable

using System;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test
{
    public class ApplyTest
    {
        [Fact]
        public void ApplyWorksOnNullableReferenceType()
        {
            var value = GetStringAsNullable("foo");
            var length = value?.Then<string, int>(GetLength);
            Assert.Equal(3, length);
        }

        [Fact]
        public void ApplyWorksOnNull()
        {
            var value = GetStringAsNullable(null);
            var length = value?.Then(GetLength);
            Assert.Null(length);
        }

        [Fact]
        public void ApplyWorksOnNullableValueType()
        {
            int? value = 10;
            Assert.Equal(30, value?.Then(Square));
        }

        [Fact]
        public void ApplyWorksOnNullableValueTypeNull()
        {
            int? value = null;
            Assert.Null(value?.Then(Square));
        }

        private static int GetLength(string value) => value.Length;

        private static int Square(int value) => value * value;

        private static string? GetStringAsNullable(string? value) => value;
    }
}
