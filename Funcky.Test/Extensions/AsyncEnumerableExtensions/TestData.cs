using System.Collections.Generic;
using System.Linq;

namespace Funcky.Test.Extensions.AsyncEnumerableExtensions
{
    internal static class TestData
    {
        public const string FirstItem = "first";

        public const string LastItem = "last";

        public const string MiddleItem = "middle";

        public static readonly IAsyncEnumerable<string> EmptyEnumerable
            = AsyncEnumerable.Empty<string>();

        public static readonly IAsyncEnumerable<string> EnumerableWithOneItem
            = AsyncEnumerable.Repeat(FirstItem, 1);

        public static readonly IAsyncEnumerable<int> EnumerableTwoItems
            = new[] { 42, 1337 }.ToAsyncEnumerable();

        public static readonly IAsyncEnumerable<string> EnumerableWithMoreThanOneItem
            = new[] { FirstItem, MiddleItem, LastItem }.ToAsyncEnumerable();
    }
}
