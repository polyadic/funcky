#if ASYNC_SUPPORTED
namespace Funcky.Monads;

public static partial class OptionAsyncExtensions
{
    extension<TItem>(Option<TItem> option)
        where TItem : notnull
    {
        /// <summary>
        /// Returns an <see cref="IAsyncEnumerable{T}"/> that yields exactly one value when the option
        /// has an item and nothing when the option is empty.
        /// </summary>
        [Pure]
        public IAsyncEnumerable<TItem> ToAsyncEnumerable()
            => option.Match(
                none: AsyncEnumerable.Empty<TItem>,
                some: AsyncSequence.Return);
    }
}
#endif
