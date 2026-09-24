namespace Funcky.Test.TestUtilities;

internal static class EnumerableExtensions
{
    /// <summary>Prevents LINQ from optimizing by hiding the underlying source enumerable.</summary>
    internal static IEnumerable<TItem> PreventLinqOptimizations<TItem>(this IEnumerable<TItem> source)
    {
        foreach (var element in source)
        {
            yield return element;
        }
    }
}
