namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    [Pure]
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        => enumerable.SelectMany(Identity);
}
