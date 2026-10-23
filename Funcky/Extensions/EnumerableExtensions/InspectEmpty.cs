using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// An IEnumerable that calls a function if and only if the source has no element to enumerate. It can be used to encode side effects on an empty IEnumerable.
        /// The side effect will be executed when enumerating the result.
        /// </summary>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public IEnumerable<TSource> InspectEmpty(Action inspector)
            => InspectEmptyIterator(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(inspector));
    }

    private static IEnumerable<TSource> InspectEmptyIterator<TSource>(IEnumerable<TSource> source, Action inspector)
    {
        using var enumerator = source.GetEnumerator();

        if (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
        else
        {
            inspector();
            yield break;
        }

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}
