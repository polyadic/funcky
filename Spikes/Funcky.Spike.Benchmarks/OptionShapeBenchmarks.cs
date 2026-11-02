using BenchmarkDotNet.Attributes;
using Funcky.Monads;
using Funcky.Spike.Unions;

namespace Funcky.Spike.Benchmarks;

/// <summary>Construct, Select, SelectMany and Match, the four things every Option-heavy loop does, per shape.</summary>
[MemoryDiagnoser]
public class OptionShapeBenchmarks
{
    private const int Iterations = 1000;

    [Benchmark(Baseline = true)]
    public int StructOption()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            var option = Option.Some(i).Select(x => x + 1).SelectMany(x => x % 3 == 0 ? Option<int>.None : Option.Some(x));
            sum += option.Match(none: 0, some: x => x);
        }

        return sum;
    }

    [Benchmark]
    public int UnionOption()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            var option = Unions.UnionOption.Some(i).Select(x => x + 1).SelectMany(x => x % 3 == 0 ? UnionOption<int>.Empty : Unions.UnionOption.Some(x));
            sum += option.Match(none: 0, some: x => x);
        }

        return sum;
    }

    [Benchmark]
    public int ClosedOption()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            var option = Unions.ClosedOption.Some(i).Select(x => x + 1).SelectMany(x => x % 3 == 0 ? Unions.ClosedOption.None<int>() : Unions.ClosedOption.Some(x));
            sum += option.Match(none: 0, some: x => x);
        }

        return sum;
    }

    [Benchmark]
    public int BareOption()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            BareOption<int> option = i;
            option = option.Select(x => x + 1);
            sum += option.Match(none: () => 0, some: x => x);
        }

        return sum;
    }

    /// <summary>Pattern matching on the three shapes, no Match delegate.</summary>
    [Benchmark]
    public int StructOptionPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            var option = Option.Some(i);
            sum += option is [var value] ? value : 0;
        }

        return sum;
    }

    [Benchmark]
    public int UnionOptionPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            UnionOption<int> option = i;
            sum += option is Some<int>(var value) ? value : 0;
        }

        return sum;
    }

    [Benchmark]
    public int ClosedOptionPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            ClosedOption<int> option = i;
            sum += option is ClosedOption<int>.Some(var value) ? value : 0;
        }

        return sum;
    }
}
