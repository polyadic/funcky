using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<int> source)
        {
            using IEnumerator<int> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(enumerator.Current);
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<long> source)
        {
            using IEnumerator<long> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(enumerator.Current);
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<float> source)
        {
            using IEnumerator<float> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorFloat();

            while (enumerator.MoveNext())
            {
                calculator.Add(enumerator.Current);
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<float> AverageOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<double> source)
        {
            using IEnumerator<double> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(enumerator.Current);
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<decimal> source)
        {
            using IEnumerator<decimal> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDecimal();

            while (enumerator.MoveNext())
            {
                calculator.Add(enumerator.Current);
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<decimal> AverageOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).AverageOrNone();

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(selector(enumerator.Current));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                selector(enumerator.Current).AndThen(c => calculator.Add(c));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(selector(enumerator.Current));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                selector(enumerator.Current).AndThen(c => calculator.Add(c));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorFloat();

            while (enumerator.MoveNext())
            {
                calculator.Add(selector(enumerator.Current));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<float> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorFloat();

            while (enumerator.MoveNext())
            {
                selector(enumerator.Current).AndThen(c => calculator.Add(c));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                calculator.Add(selector(enumerator.Current));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<double> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDouble();

            while (enumerator.MoveNext())
            {
                selector(enumerator.Current).AndThen(c => calculator.Add(c));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDecimal();

            while (enumerator.MoveNext())
            {
                calculator.Add(selector(enumerator.Current));
            }

            return calculator.Average;
        }

        [Pure]
        public static Option<decimal> AverageOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
        {
            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            var calculator = new AverageCalculatorDecimal();

            while (enumerator.MoveNext())
            {
                selector(enumerator.Current).AndThen(c => calculator.Add(c));
            }

            return calculator.Average;
        }

        private class AverageCalculatorDouble
        {
            private Option<double> _sum;
            private int _count;

            public Option<double> Average => _sum.Select(s => s / _count);

            public void Add(int term)
            {
                _sum = Option.Some(_sum.Match(term, s => s + term));
                ++_count;
            }

            public void Add(long term)
            {
                _sum = Option.Some(_sum.Match(term, s => s + term));
                ++_count;
            }

            public void Add(double term)
            {
                _sum = Option.Some(_sum.Match(term, s => s + term));
                ++_count;
            }
        }

        private class AverageCalculatorFloat
        {
            private Option<float> _sum;
            private int _count;

            public Option<float> Average => _sum.Select(s => s / _count);

            public void Add(float term)
            {
                _sum = Option.Some(_sum.Match(term, s => s + term));
                ++_count;
            }
        }

        private class AverageCalculatorDecimal
        {
            private Option<decimal> _sum;
            private int _count;

            public Option<decimal> Average => _sum.Select(s => s / _count);

            public void Add(decimal term)
            {
                _sum = Option.Some(_sum.Match(term, s => s + term));
                ++_count;
            }
        }
    }
}
