namespace Funcky.Monads;

public static partial class IEnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        where T : notnull
        => enumerable.SelectMany(Identity);
}
