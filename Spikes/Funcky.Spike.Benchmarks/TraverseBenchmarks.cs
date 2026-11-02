using BenchmarkDotNet.Attributes;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Spike.Traits;

namespace Funcky.Spike.Benchmarks;

/// <summary>The #862 question in numbers: the hand-written <c>IEnumerable.Traverse(Func&lt;T, Option&lt;R&gt;&gt;)</c>
/// against the single trait-based Traverse, for the struct Option (boxed into K) and the direct closed record.</summary>
[MemoryDiagnoser]
public class TraverseBenchmarks
{
    private readonly int[] _numbers = Enumerable.Range(0, 1000).ToArray();

    [Benchmark(Baseline = true)]
    public int HandWrittenTraverse()
        => _numbers.Traverse(x => Option.Some(x + 1)).Match(none: 0, some: xs => xs.Count());

    [Benchmark]
    public int TraitTraverseStructOption()
        => OptionKind.As(new Seq<int>(_numbers).Traverse<SeqKind, OptionKind, int, int>(x => OptionKind.Pure(x + 1))).Match(none: 0, some: xs => SeqKind.As(xs).Count());

    [Benchmark]
    public int TraitTraverseClosedRecord()
        => new Seq<int>(_numbers).Traverse<SeqKind, ClosedOptionDirectKind, int, int>(x => ClosedOptionDirectKind.Pure(x + 1)) is DirectOption<K<SeqKind, int>>.Some(var xs) ? SeqKind.As(xs).Count() : 0;
}
