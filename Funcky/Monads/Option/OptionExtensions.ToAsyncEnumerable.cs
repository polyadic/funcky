#if INTEGRATED_ASYNC
namespace Funcky.Monads;

public static partial class OptionAsyncExtensions
{
    /// <summary>
    /// Returns an <see cref="IAsyncEnumerable{T}"/> that yields exactly one value when the option
    /// has an item and nothing when the option is empty.
    /// </summary>
    public static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(this Option<TItem> option)
        where TItem : notnull
        => option.Match(
            none: AsyncEnumerable.Empty<TItem>,
            some: AsyncSequence.Return);
}
#endif
