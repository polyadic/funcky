using Funcky.Monads;
using static Funcky.Monads.Option;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        private class AverageCalculatorDouble
        {
            public static readonly AverageCalculatorDouble Empty = new AverageCalculatorDouble();

            private readonly int _count;
            private readonly Option<double> _sum;

            private AverageCalculatorDouble(int count = default, Option<double> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<double> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorDouble Add(int term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<int> term)
                => term.Match(none: this, some: Add);

            public AverageCalculatorDouble Add(long term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<long> term)
                => term.Match(none: this, some: Add);

            public AverageCalculatorDouble Add(double term)
                => new AverageCalculatorDouble(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDouble Add(Option<double> term)
                => term.Match(none: this, some: Add);
        }

        private class AverageCalculatorFloat
        {
            public static readonly AverageCalculatorFloat Empty = new AverageCalculatorFloat();

            private readonly Option<float> _sum;
            private readonly int _count;

            private AverageCalculatorFloat(int count = default, Option<float> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<float> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorFloat Add(float term)
                => new AverageCalculatorFloat(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorFloat Add(Option<float> term)
                => term.Match(none: this, some: Add);
        }

        private class AverageCalculatorDecimal
        {
            public static readonly AverageCalculatorDecimal Empty = new AverageCalculatorDecimal();

            private readonly Option<decimal> _sum;
            private readonly int _count;

            private AverageCalculatorDecimal(int count = default, Option<decimal> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<decimal> Average => _sum.Select(sum => sum / _count);

            public AverageCalculatorDecimal Add(decimal term)
                => new AverageCalculatorDecimal(_count + 1, Some(_sum.Match(none: term, some: sum => sum + term)));

            public AverageCalculatorDecimal Add(Option<decimal> term)
                => term.Match(none: this, some: Add);
        }
    }
}
