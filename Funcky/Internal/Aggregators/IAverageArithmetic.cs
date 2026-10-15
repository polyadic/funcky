namespace Funcky.Internal.Aggregators;

/// <summary>The arithmetic of one <see cref="AverageAggregator{TSum, TResult, TArithmetic}"/>: how terms are summed and how the sum is divided by the count.</summary>
/// <typeparam name="TSum">The type the terms are accumulated in.</typeparam>
/// <typeparam name="TResult">The type of the average.</typeparam>
/// <remarks>Implemented by empty structs so the JIT specializes the aggregator per arithmetic without virtual calls or delegates.
/// This replaces static abstract interface members, which are only available with generic math on .NET 7 and later.</remarks>
internal interface IAverageArithmetic<TSum, TResult>
{
    TSum Add(TSum sum, TSum term);

    TResult Divide(TSum sum, long count);
}
