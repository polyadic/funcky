#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <returns>Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence.</returns>
        [Pure]
        public IAsyncEnumerable<ValueWithFirst<TSource>> WithFirst()
            => WithFirstInternal(ArgumentValidator.NotNull(source));
    }

    private static async IAsyncEnumerable<ValueWithFirst<TSource>> WithFirstInternal<TSource>(IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        #pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var enumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
        #pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (!await enumerator.MoveNextAsync())
        {
            yield break;
        }

        yield return new ValueWithFirst<TSource>(enumerator.Current, true);

        while (await enumerator.MoveNextAsync())
        {
            yield return new ValueWithFirst<TSource>(enumerator.Current, false);
        }
    }
}
#endif
