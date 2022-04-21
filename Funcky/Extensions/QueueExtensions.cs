using System.Collections.Concurrent;
#if QUEUE_TRY_OVERLOADS
#else
using Funcky.Internal;
#endif

namespace Funcky.Extensions;

public static partial class QueueExtensions
{
#if QUEUE_TRY_OVERLOADS
    [Pure]
    public static Option<TItem> DequeueOrNone<TItem>(this Queue<TItem> queue)
        where TItem : notnull
        => queue.TryDequeue(out var result)
            ? result
            : Option<TItem>.None;

    [Pure]
    public static Option<TItem> PeekOrNone<TItem>(this Queue<TItem> queue)
        where TItem : notnull
        => queue.TryPeek(out var result)
            ? result
            : Option<TItem>.None;
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
        => concurrentQueue.TryDequeue(out var result)
            ? result
            : Option<TItem>.None;

    [Pure]
    public static Option<TItem> PeekOrNone<TItem>(this ConcurrentQueue<TItem> concurrentQueue)
        where TItem : notnull
        => concurrentQueue.TryPeek(out var result)
            ? result
            : Option<TItem>.None;
}
