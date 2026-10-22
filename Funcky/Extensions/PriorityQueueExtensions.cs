#if PRIORITY_QUEUE
namespace Funcky.Extensions;

public static class PriorityQueueExtensions
{
    extension<TElement, TPriority>(PriorityQueue<TElement, TPriority> priorityQueue)
        where TElement : notnull
    {
        public Option<(TElement Element, TPriority Priority)> DequeueOrNone()
            => priorityQueue.TryDequeue(out var element, out var priority)
                ? Option.Some((element, priority))
                : Option<(TElement, TPriority)>.None;

        public Option<(TElement Element, TPriority Priority)> PeekOrNone()
            => priorityQueue.TryPeek(out var element, out var priority)
                ? Option.Some((element, priority))
                : Option<(TElement, TPriority)>.None;
    }
}
#endif
