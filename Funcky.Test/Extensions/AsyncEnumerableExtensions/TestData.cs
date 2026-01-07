namespace Funcky.Test.Extensions.AsyncEnumerableExtensions;

internal static class TestData
{
    public const string FirstItem = "first";

    public const string LastItem = "last";

    public const string MiddleItem = "middle";

    public static readonly IAsyncEnumerable<string> EmptyEnumerable
        = AsyncEnumerable.Empty<string>();

    public static readonly IAsyncEnumerable<string> EnumerableWithOneItem
        = AsyncSequence.Return(FirstItem);

    public static readonly IAsyncEnumerable<int> EnumerableTwoItems
        = AsyncSequence.Return(42, 1337);

    public static readonly IAsyncEnumerable<string> EnumerableWithMoreThanOneItem
        = AsyncSequence.Return(FirstItem, MiddleItem, LastItem);

    public static readonly IAsyncEnumerable<int> OneToFive
        = AsyncSequence.Return(1, 2, 3, 4, 5);
}
