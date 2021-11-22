using static Funcky.Monads.Option;

namespace Funcky.Internal.Aggregators
{
    internal class DecimalAverageAggregator
    {
        public static readonly DecimalAverageAggregator Empty = new();

        private readonly Option<decimal> _sum;
        private readonly int _count;

        private DecimalAverageAggregator(int count = default, Option<decimal> sum = default)
            => (_count, _sum) = (count, sum);

        public Option<decimal> Average => _sum.Select(sum => sum / _count);

        public DecimalAverageAggregator Add(decimal term)
            => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

        public DecimalAverageAggregator Add(Option<decimal> term)
            => term.Match(none: this, some: Add);
    }
}
