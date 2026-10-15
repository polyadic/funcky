using static Funcky.Monads.Option;

namespace Funcky.Internal.Aggregators;

/// <summary>Accumulates terms and yields their average, or <c>None</c> if no term was added.</summary>
/// <typeparam name="TSum">The type the terms are accumulated in.</typeparam>
/// <typeparam name="TResult">The type of the average.</typeparam>
/// <typeparam name="TArithmetic">The arithmetic used to sum and divide, see <see cref="AverageAggregators"/>.</typeparam>
/// <remarks>The default value is the empty aggregator.</remarks>
internal readonly struct AverageAggregator<TSum, TResult, TArithmetic>
    where TSum : notnull
    where TResult : notnull
    where TArithmetic : struct, IAverageArithmetic<TSum, TResult>
{
    private readonly long _count;
    private readonly Option<TSum> _sum;

    private AverageAggregator(long count, Option<TSum> sum)
        => (_count, _sum) = (count, sum);

    public Option<TResult> Average
    {
        get
        {
            var count = _count;
            return _sum.Select(sum => default(TArithmetic).Divide(sum, count));
        }
    }

    public AverageAggregator<TSum, TResult, TArithmetic> Add(TSum term)
        => new(_count + 1, Some(_sum.Match(none: term, some: sum => default(TArithmetic).Add(sum, term))));

    public AverageAggregator<TSum, TResult, TArithmetic> Add(Option<TSum> term)
        => term.Match(none: this, some: Add);
}
