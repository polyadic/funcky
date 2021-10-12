using static Funcky.Monads.Option;

namespace Funcky.Internal.Aggregators
{
    internal class FloatAverageAggregator
    {
        public static readonly FloatAverageAggregator Empty = new();

        private readonly Option<float> _sum;
        private readonly int _count;

        private FloatAverageAggregator(int count = default, Option<float> sum = default)
            => (_count, _sum) = (count, sum);

        public Option<float> Average => _sum.Select(sum => sum / _count);

        public FloatAverageAggregator Add(float term)
            => new(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

        public FloatAverageAggregator Add(Option<float> term)
            => term.Match(none: this, some: Add);
    }
}
