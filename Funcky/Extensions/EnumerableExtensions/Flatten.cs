namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    [Pure]
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
#pragma warning disable λ1018 // This is the implementation of Flatten.
        => enumerable.SelectMany(Identity);
#pragma warning restore λ1018
}
