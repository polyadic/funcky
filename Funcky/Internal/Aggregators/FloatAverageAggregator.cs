using static Funcky.Monads.Option;

namespace Funcky.Internal.Aggregators;

internal class FloatAverageAggregator
{
    public static readonly FloatAverageAggregator Empty = new();

    // we calculate with double for higher precision and fewer overflow problems
    // The implementation in .NET seems to do the same, because we seem to have no aberration.
    private readonly Option<double> _sum;
    private readonly int _count;

    private FloatAverageAggregator(int count = default, Option<double> sum = default)
        => (_count, _sum) = (count, sum);

    public Option<float> Average => _sum.Select(sum => (float)(sum / _count));

    public FloatAverageAggregator Add(float term)
        => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

    public FloatAverageAggregator Add(Option<float> term)
        => term.Match(none: this, some: Add);
}
