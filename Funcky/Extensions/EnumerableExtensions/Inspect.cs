namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
    /// The side effect will be executed when enumerating the result.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
    [Pure]
    public static IEnumerable<TSource> Inspect<TSource>(this IEnumerable<TSource> source, Action<TSource> inspector)
        => source.Select(element
            =>
            {
                inspector(element);
                return element;
            });
}
