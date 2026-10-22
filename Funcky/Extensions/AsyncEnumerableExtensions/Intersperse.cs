#if ASYNC_SUPPORTED

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public IAsyncEnumerable<TSource> Intersperse(TSource element)
            => source.WithFirst().SelectMany(item => item.IsFirst
                ? AsyncSequence.Return(item.Value)
                : AsyncSequence.Return(element).Append(item.Value));
    }
}
#endif
