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

    /// <summary>Today's struct with the Select/SelectMany a fixed Funcky would have (branch instead of a capturing
    /// lambda, see ShapeFactsTest.TodaysStructOptionSelectAllocatesAClosurePerCall). The fair baseline for the shapes.</summary>
    [Benchmark]
    public int StructOptionBranchSelect()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            var option = Option.Some(i).SelectBranch(x => x + 1).SelectManyBranch(x => x % 3 == 0 ? Option<int>.None : Option.Some(x));
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

    /// <summary>Shape 3: today's layout as a custom union with the non-boxing access members (HasValue/TryGetValue).</summary>
    [Benchmark]
    public int CustomUnionOptionPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            CustomUnionOption<int> option = i;
            sum += option is int value ? value : 0;
        }

        return sum;
    }

    [Benchmark]
    public int CustomUnionOptionStringPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            CustomUnionOption<string> option = "x";
            sum += option is string value ? value.Length : 0;
        }

        return sum;
    }

    /// <summary>The match in code generic over the payload (`is T value`): day 1 boxed here (Value), day 2 goes through
    /// TryGetValue(out T) and does not.</summary>
    [Benchmark]
    public int CustomUnionOptionGenericPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += GenericMatch(CustomUnionOption.Some(i));
        }

        return sum;
    }

    [Benchmark]
    public int StructOptionGenericPattern()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += GenericMatch(Option.Some(i));
        }

        return sum;
    }

    private static int GenericMatch<T>(CustomUnionOption<T> option)
        where T : notnull
        => option is T value ? value.GetHashCode() : 0;

    private static int GenericMatch<T>(Option<T> option)
        where T : notnull
        => option is [var value] ? value.GetHashCode() : 0;

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
