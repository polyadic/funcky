#if PRIORITY_QUEUE
using FsCheck;

namespace Funcky.Test.Extensions;

public class PriorityQueueArbitrary
{
    public static Arbitrary<PriorityQueue<string, int>> GeneratePriorityQueue()
        => GeneratePriorityQueueGenerator()
            .ToArbitrary();

    public static Gen<PriorityQueue<string, int>> GeneratePriorityQueueGenerator()
        => from values in Arb.Generate<List<string>>()
            from seed in Arb.Generate<int>()
            let randomPriority = new System.Random(seed)
            let priorityQueue = new PriorityQueue<string, int>(values.Select(v => (v, randomPriority.Next())))
            select priorityQueue;
}
#endif
