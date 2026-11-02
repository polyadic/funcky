using Funcky.Monads;

namespace Funcky.Spike.Benchmarks;

/// <summary>Select/SelectMany for today's struct Option written as a branch instead of a capturing lambda: what Funcky's
/// own implementation would look like after the side finding in ShapeFactsTest. Allocation-free.</summary>
public static class BranchSelect
{
    public static Option<TResult> SelectBranch<T, TResult>(this Option<T> option, Func<T, TResult> selector)
        where T : notnull
        where TResult : notnull
        => option.TryGetValue(out var value) ? Option.Some(selector(value)) : Option<TResult>.None;

    public static Option<TResult> SelectManyBranch<T, TResult>(this Option<T> option, Func<T, Option<TResult>> selector)
        where T : notnull
        where TResult : notnull
        => option.TryGetValue(out var value) ? selector(value) : Option<TResult>.None;
}
