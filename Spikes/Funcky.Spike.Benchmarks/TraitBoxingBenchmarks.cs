using BenchmarkDotNet.Attributes;
using Funcky.Monads;
using Funcky.Spike.Traits;

namespace Funcky.Spike.Benchmarks;

/// <summary>1000 Map steps through the trait, per Option shape, against the direct struct call. The gap between
/// <see cref="DirectStructSelect"/> and <see cref="TraitStructOption"/> is the price of <c>K&lt;F, A&gt;</c> for a struct.</summary>
[MemoryDiagnoser]
public class TraitBoxingBenchmarks
{
    private const int Steps = 1000;

    [Benchmark(Baseline = true)]
    public int DirectStructSelect()
    {
        var option = Option.Some(0);
        for (var i = 0; i < Steps; i++)
        {
            option = option.Select(static x => x + 1);
        }

        return option.GetOrElse(-1);
    }

    /// <summary>The fair baseline: today's struct with a branch-based Select (no closure).</summary>
    [Benchmark]
    public int DirectStructBranchSelect()
    {
        var option = Option.Some(0);
        for (var i = 0; i < Steps; i++)
        {
            option = option.SelectBranch(static x => x + 1);
        }

        return option.GetOrElse(-1);
    }

    [Benchmark]
    public int TraitStructOption() => OptionKind.As(Run<OptionKind>()).GetOrElse(-1);

    [Benchmark]
    public int TraitUnionOption() => UnionOptionKind.As(Run<UnionOptionKind>()).GetOrElse(-1);

    [Benchmark]
    public int TraitClosedRecordViaWrapper() => ClosedOptionKind.As(Run<ClosedOptionKind>()).GetOrElse(-1);

    [Benchmark]
    public int TraitClosedRecordDirect() => ClosedOptionDirectKind.As(Run<ClosedOptionDirectKind>()) is DirectOption<int>.Some(var value) ? value : -1;

    private static K<M, int> Run<M>()
        where M : Monad<M>
    {
        var m = M.Pure(0);
        for (var i = 0; i < Steps; i++)
        {
            m = M.Map(m, static x => x + 1);
        }

        return m;
    }
}
