namespace Funcky.Test.TestUtils;

internal static class EnumerableExtensions
{
    /// <summary>
    /// Makes sure it is really a enumerable, and not a special case like a <see cref="List{T}"/> or <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    internal static IEnumerable<TItem> ToEnumerable<TItem>(this IEnumerable<TItem> source)
    {
        return source.Select(Identity);
    }
}
