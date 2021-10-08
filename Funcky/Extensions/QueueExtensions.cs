using System.Collections.Concurrent;
using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class QueueExtensions
    {
#if QUEUE_TRY_OVERLOADS
        [Pure]
        public static Option<TItem> DequeueOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => FailToOption<TItem>.FromTryPattern(queue.TryDequeue);

        [Pure]
        public static Option<TItem> PeekOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => FailToOption<TItem>.FromTryPattern(queue.TryPeek);
#else
        [Pure]
        public static Option<TItem> DequeueOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => FailToOption<TItem>.FromException<InvalidOperationException>(queue.Dequeue);

        [Pure]
        public static Option<TItem> PeekOrNone<TItem>(this Queue<TItem> queue)
            where TItem : notnull
            => FailToOption<TItem>.FromException<InvalidOperationException>(queue.Peek);

#endif

        [Pure]
        public static Option<TItem> DequeueOrNone<TItem>(this ConcurrentQueue<TItem> concurrentQueue)
            where TItem : notnull
            => FailToOption<TItem>.FromTryPattern(concurrentQueue.TryDequeue);

        [Pure]
        public static Option<TItem> PeekOrNone<TItem>(this ConcurrentQueue<TItem> concurrentQueue)
            where TItem : notnull
            => FailToOption<TItem>.FromTryPattern(concurrentQueue.TryPeek);
    }
}
