#nullable enable

using Funcky.Extensions;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class ObjectExtensionTest
    {
        [Fact]
        public void ApplyWorksOnNullableReferenceType()
        {
            var value = GetNullableString("foo");
            var length = value?.Then(GetLength);
            Assert.Equal(3, length);
        }

        [Fact]
        public void ApplyWorksOnNull()
        {
            var value = GetNullableString(null);
            var length = value?.Then(GetLength);
            Assert.Null(length);
        }

        [Fact]
        public void ApplyWorksOnNullableValueType()
        {
            var value = GetNullableInteger(10);
            Assert.Equal(30, value?.Then(Square));
        }

        [Fact]
        public void ApplyWorksOnNullableValueTypeNull()
        {
            var value = GetNullableInteger(null);
            Assert.Null(value?.Then(Square));
        }

        private static int GetLength(string value) => value.Length;

        private static int Square(int value) => value * value;

        private static string? GetNullableString(string? value) => value;

        private static int? GetNullableInteger(int? value) => value;
    }
}
