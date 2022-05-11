#if PRIORITY_QUEUE
namespace Funcky.Extensions;

public static class PriorityQueueExtensions
{
    public static Option<(TElement Element, TPriority Priority)> DequeueOrNone<TElement, TPriority>(this PriorityQueue<TElement, TPriority> priorityQueue)
        where TElement : notnull
        => priorityQueue.TryDequeue(out var element, out var priority)
            ? Option.Some((element, priority))
            : Option.None<(TElement, TPriority)>();

    public static Option<(TElement Element, TPriority Priority)> PeekOrNone<TElement, TPriority>(this PriorityQueue<TElement, TPriority> priorityQueue)
        where TElement : notnull
        => priorityQueue.TryPeek(out var element, out var priority)
            ? Option.Some((element, priority))
            : Option.None<(TElement, TPriority)>();
}
#endif
