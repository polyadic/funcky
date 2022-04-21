using static Funcky.Monads.Option;

namespace Funcky.Internal.Aggregators;

internal class DoubleAverageAggregator
{
    public static readonly DoubleAverageAggregator Empty = new();

    private readonly int _count;
    private readonly Option<double> _sum;

    private DoubleAverageAggregator(int count = default, Option<double> sum = default)
        => (_count, _sum) = (count, sum);

    public Option<double> Average => _sum.Select(sum => sum / _count);

    public DoubleAverageAggregator Add(int term)
        => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

    public DoubleAverageAggregator Add(Option<int> term)
        => term.Match(none: this, some: Add);

    public DoubleAverageAggregator Add(long term)
        => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

    public DoubleAverageAggregator Add(Option<long> term)
        => term.Match(none: this, some: Add);

    public DoubleAverageAggregator Add(double term)
        => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

    public DoubleAverageAggregator Add(Option<double> term)
        => term.Match(none: this, some: Add);
}
