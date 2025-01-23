#if PRIORITY_QUEUE
using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;

namespace Funcky.Test.Extensions;

public sealed class PriorityQueueExtensionsTest
{
    [FunckyProperty]
    public Property PeekOrNoneReturnsLogicallyTheSameAsTryPeek(PriorityQueue<string, int> priorityQueue)
        => (priorityQueue.TryPeek(out var element, out var priority)
            ? priorityQueue.PeekOrNone().Match(none: false, some: IsSame(element, priority))
            : priorityQueue.PeekOrNone().Match(none: true, some: False)).ToProperty();

    [FunckyProperty]
    public Property DequeueOrNoneReturnsLogicallyTheSameAsTryDequeue(PriorityQueue<string, int> priorityQueue)
    {
        // Dequeue has a side-effect, so we need a Copy
        var referenceQueue = new PriorityQueue<string, int>(priorityQueue.UnorderedItems);

        return (referenceQueue.TryDequeue(out var element, out var priority)
            ? priorityQueue.DequeueOrNone().Match(none: false, some: IsSame(element, priority))
            : priorityQueue.DequeueOrNone().Match(none: true, some: False)).ToProperty();
    }

    private static Func<(string Element, int Priority), bool> IsSame(string element, int priority)
        => value
            => value == (element, priority);
}
#endif
