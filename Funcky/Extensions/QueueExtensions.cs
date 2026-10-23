using System.Collections.Concurrent;
#if QUEUE_TRY_OVERLOADS
#else
using Funcky.Internal;
#endif

namespace Funcky.Extensions;

public static partial class QueueExtensions
{
#if QUEUE_TRY_OVERLOADS
    extension<TItem>(Queue<TItem> queue)
        where TItem : notnull
    {
        [Pure]
        public Option<TItem> DequeueOrNone()
            => queue.TryDequeue(out var result)
                ? result
                : Option<TItem>.None;

        [Pure]
        public Option<TItem> PeekOrNone()
            => queue.TryPeek(out var result)
                ? result
                : Option<TItem>.None;
    }
#else
    extension<TItem>(Queue<TItem> queue)
        where TItem : notnull
    {
        [Pure]
        public Option<TItem> DequeueOrNone()
            => FailToOption<TItem>.FromException<InvalidOperationException>(queue.Dequeue);

        [Pure]
        public Option<TItem> PeekOrNone()
            => FailToOption<TItem>.FromException<InvalidOperationException>(queue.Peek);
    }
#endif

    extension<TItem>(ConcurrentQueue<TItem> concurrentQueue)
        where TItem : notnull
    {
        [Pure]
        public Option<TItem> DequeueOrNone()
            => concurrentQueue.TryDequeue(out var result)
                ? result
                : Option<TItem>.None;

        [Pure]
        public Option<TItem> PeekOrNone()
            => concurrentQueue.TryPeek(out var result)
                ? result
                : Option<TItem>.None;
    }
}
