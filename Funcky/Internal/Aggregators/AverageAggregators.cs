namespace Funcky.Internal.Aggregators;

/// <summary>The empty aggregators for every numeric type <c>AverageOrNone</c> supports, and the arithmetic behind them.</summary>
/// <remarks>The sum types and the overflow behaviour follow <see cref="Enumerable.Average(IEnumerable{long})"/> and its siblings:
/// integers are summed exactly in a checked <see cref="long"/>, <see cref="float"/> is summed in <see cref="double"/>.</remarks>
internal static class AverageAggregators
{
    public static readonly AverageAggregator<long, double, Int64Arithmetic> Int64 = default;

    public static readonly AverageAggregator<double, float, SingleArithmetic> Single = default;

    public static readonly AverageAggregator<double, double, DoubleArithmetic> Double = default;

    public static readonly AverageAggregator<decimal, decimal, DecimalArithmetic> Decimal = default;

    extension(AverageAggregator<long, double, Int64Arithmetic> aggregator)
    {
        /// <summary>Adds an <see cref="int"/> term to the <see cref="long"/> aggregator; the widening of a plain <see cref="int"/> is implicit.</summary>
        public AverageAggregator<long, double, Int64Arithmetic> Add(Option<int> term)
            => aggregator.Add(term.Select(static value => (long)value));
    }

    extension(AverageAggregator<double, float, SingleArithmetic> aggregator)
    {
        /// <summary>Adds a <see cref="float"/> term to the <see cref="double"/> aggregator; the widening of a plain <see cref="float"/> is implicit.</summary>
        public AverageAggregator<double, float, SingleArithmetic> Add(Option<float> term)
            => aggregator.Add(term.Select(static value => (double)value));
    }

    internal readonly struct Int64Arithmetic : IAverageArithmetic<long, double>
    {
        public long Add(long sum, long term) => checked(sum + term);

        public double Divide(long sum, long count) => (double)sum / count;
    }

    internal readonly struct SingleArithmetic : IAverageArithmetic<double, float>
    {
        public double Add(double sum, double term) => sum + term;

        public float Divide(double sum, long count) => (float)(sum / count);
    }

    internal readonly struct DoubleArithmetic : IAverageArithmetic<double, double>
    {
        public double Add(double sum, double term) => sum + term;

        public double Divide(double sum, long count) => sum / count;
    }

    internal readonly struct DecimalArithmetic : IAverageArithmetic<decimal, decimal>
    {
        public decimal Add(decimal sum, decimal term) => sum + term;

        public decimal Divide(decimal sum, long count) => sum / count;
    }
}
