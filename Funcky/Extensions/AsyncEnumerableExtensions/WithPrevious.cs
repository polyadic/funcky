#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
        /// <exception cref="ArgumentNullException">Thrown when any value in <c>source</c> is <see langword="null"/>.</exception>
        [Pure]
        public IAsyncEnumerable<ValueWithPrevious<TSource>> WithPrevious()
            => ArgumentValidator.NotNull(source).WithPreviousInternal();

        private async IAsyncEnumerable<ValueWithPrevious<TSource>> WithPreviousInternal([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var previous = Option<TSource>.None;

            await foreach (var value in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                yield return new ValueWithPrevious<TSource>(value, previous);
                previous = value;
            }
        }
    }
}
#endif
