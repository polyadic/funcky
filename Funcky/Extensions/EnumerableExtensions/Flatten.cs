namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<T>(IEnumerable<IEnumerable<T>> enumerable)
    {
        [Pure]
        public IEnumerable<T> Flatten()
#pragma warning disable λ1018 // This is the implementation of Flatten.
            => enumerable.SelectMany(Identity);
    }
#pragma warning restore λ1018
}
