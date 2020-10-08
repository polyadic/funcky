using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<int> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<long> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<float> source)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(element)).Average;

        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<double> source)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(element)).Average;

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<decimal> source)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(element)).Average;

        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.WhereSelect(selector).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.WhereSelect(selector).AverageOrNone();

        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(AverageCalculatorFloat.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.WhereSelect(selector).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(AverageCalculatorDouble.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.WhereSelect(selector).AverageOrNone();

        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(AverageCalculatorDecimal.Empty, (calculator, element) => calculator.Add(selector(element))).Average;

        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.WhereSelect(selector).AverageOrNone();

        private class AverageCalculatorDouble
        {
            public static readonly AverageCalculatorDouble Empty = new AverageCalculatorDouble();

            private readonly int _count;
            private readonly Option<double> _sum;

            public AverageCalculatorDouble(int count = default, Option<double> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<double> Average => _sum.Select(s => s / _count);

            public AverageCalculatorDouble Add(int term)
                => new AverageCalculatorDouble(_count + 1, from sum in _sum select sum + term);

            public AverageCalculatorDouble Add(long term)
                => new AverageCalculatorDouble(_count + 1, from sum in _sum select sum + term);

            public AverageCalculatorDouble Add(double term)
                => new AverageCalculatorDouble(_count + 1, from sum in _sum select sum + term);
        }

        private class AverageCalculatorFloat
        {
            public static readonly AverageCalculatorFloat Empty = new AverageCalculatorFloat();

            private readonly Option<float> _sum;
            private readonly int _count;

            public AverageCalculatorFloat(int count = default, Option<float> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<float> Average => _sum.Select(s => s / _count);

            public AverageCalculatorFloat Add(float term)
                => new AverageCalculatorFloat(_count + 1, from sum in _sum select sum + term);
        }

        private class AverageCalculatorDecimal
        {
            public static readonly AverageCalculatorDecimal Empty = new AverageCalculatorDecimal();

            private readonly Option<decimal> _sum;
            private readonly int _count;

            public AverageCalculatorDecimal(int count = default, Option<decimal> sum = default)
            {
                _count = count;
                _sum = sum;
            }

            public Option<decimal> Average => _sum.Select(s => s / _count);

            public AverageCalculatorDecimal Add(decimal term)
                => new AverageCalculatorDecimal(_count + 1, from sum in _sum select sum + term);
        }
    }
}
