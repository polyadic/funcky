#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<Option<TSource>> source)
        where TSource : notnull
    {
        /// <summary>
        /// Filters out all the empty values from an <see cref="IAsyncEnumerable{T}"><![CDATA[IAsyncEnumerable<Option<T>>]]></see> and therefore returns an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        [Pure]
        public IAsyncEnumerable<TSource> WhereSelect()
            => source.WhereSelect(Identity);
    }

    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// </summary>
        [Pure]
        public IAsyncEnumerable<TResult> WhereSelect<TResult>(Func<TSource, Option<TResult>> selector)
            where TResult : notnull
            => WhereSelectInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));

        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// The index of each source element is used in the projected form of that element.
        /// </summary>
        [Pure]
        public IAsyncEnumerable<TResult> WhereSelect<TResult>(Func<TSource, int, Option<TResult>> selector)
            where TResult : notnull
            => WhereSelectInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));

        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the asynchronous <paramref name="selector"/>,
        /// which receives the cancellation token of the enumeration.
        /// </summary>
        [Pure]
        [OverloadResolutionPriority(1)]
        public IAsyncEnumerable<TResult> WhereSelect<TResult>(Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector)
            where TResult : notnull
            => WhereSelectAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));

        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the asynchronous <paramref name="selector"/>,
        /// which receives the cancellation token of the enumeration.
        /// The index of each source element is used in the projected form of that element.
        /// </summary>
        [Pure]
        [OverloadResolutionPriority(1)]
        public IAsyncEnumerable<TResult> WhereSelect<TResult>(Func<TSource, int, CancellationToken, ValueTask<Option<TResult>>> selector)
            where TResult : notnull
            => WhereSelectAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(selector));
    }

    private static async IAsyncEnumerable<TResult> WhereSelectInternal<TSource, TResult>(IAsyncEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (selector(item).TryGetValue(out var result))
            {
                yield return result;
            }
        }
    }

    private static async IAsyncEnumerable<TResult> WhereSelectInternal<TSource, TResult>(IAsyncEnumerable<TSource> source, Func<TSource, int, Option<TResult>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var index = 0;

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (selector(item, index).TryGetValue(out var result))
            {
                yield return result;
            }

            checked
            {
                index++;
            }
        }
    }

    private static async IAsyncEnumerable<TResult> WhereSelectAwaitWithCancellationInternal<TSource, TResult>(IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if ((await selector(item, cancellationToken).ConfigureAwait(false)).TryGetValue(out var result))
            {
                yield return result;
            }
        }
    }

    private static async IAsyncEnumerable<TResult> WhereSelectAwaitWithCancellationInternal<TSource, TResult>(IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<Option<TResult>>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var index = 0;

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if ((await selector(item, index, cancellationToken).ConfigureAwait(false)).TryGetValue(out var result))
            {
                yield return result;
            }

            checked
            {
                index++;
            }
        }
    }
}
#endif
