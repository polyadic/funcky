using System.Collections.Concurrent;

namespace Funcky.Extensions
{
    public static partial class QueueExtensions
    {
#if QUEUE_TRY_OVERLOADS
        [Pure]
        public static Option<TItem> DequeueOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => queue.TryDequeue(out var result)
                ? result
                : Option<TItem>.None();

        [Pure]
        public static Option<TItem> PeekOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => queue.TryPeek(out var result)
                ? result
                : Option<TItem>.None();
#endif

        [Pure]
        public static Option<TItem> DequeueOrNone<TItem>(this ConcurrentQueue<TItem> concurrentQueue)
            where TItem : notnull
            => concurrentQueue.TryDequeue(out var result)
                ? result
                : Option<TItem>.None();

        [Pure]
        public static Option<TItem> PeekOrNone<TItem>(this ConcurrentQueue<TItem> concurrentQueue)
            where TItem : notnull
            => concurrentQueue.TryPeek(out var result)
                ? result
                : Option<TItem>.None();
    }
}
