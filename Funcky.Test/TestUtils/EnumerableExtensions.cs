namespace Funcky.Test.TestUtils;

internal static class EnumerableExtensions
{
    internal static IEnumerable<TItem> EraseNonEnumeratedCount<TItem>(this IEnumerable<TItem> source)
    {
        // Having our own state machine erases the non enumerated count
        // provided when using LINQ methods such as Select.
        foreach (var element in source)
        {
            yield return element;
        }
    }
}
