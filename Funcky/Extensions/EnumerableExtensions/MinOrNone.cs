using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.GenericConstraints;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static Option<int> MinOrNone(this IEnumerable<int> source)
            => source.Aggregate(Option<int>.None(), (min, current) => Option.Some(min.Match(current, m => current < m ? current : m)));

        public static Option<int> MinOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MinOrNone();

        public static Option<long> MinOrNone(this IEnumerable<long> source)
            => source.Aggregate(Option<long>.None(), (min, current) => Option.Some(min.Match(current, m => current < m ? current : m)));

        public static Option<long> MinOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MinOrNone();

        public static Option<double> MinOrNone(this IEnumerable<double> source)
            => source.Aggregate(Option<double>.None(), (min, current) => Option.Some(min.Match(current, m => current < m ? current : m)));

        public static Option<double> MinOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MinOrNone();

        public static Option<float> MinOrNone(this IEnumerable<float> source)
            => source.Aggregate(Option<float>.None(), (min, current) => Option.Some(min.Match(current, m => current < m ? current : m)));

        public static Option<float> MinOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MinOrNone();

        public static Option<decimal> MinOrNone(this IEnumerable<decimal> source)
            => source.Aggregate(Option<decimal>.None(), (min, current) => Option.Some(min.Match(current, m => current < m ? current : m)));

        public static Option<decimal> MinOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MinOrNone();

        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source, RequireClass<TSource>? ω = null)
            where TSource : class
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (min, current) => Option.Some(min.Match(current, m => comparer.Compare(m, current) < 0 ? current : m)));
        }

        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<TSource> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (min, current) => Option.Some(min.Match(current, m => comparer.Compare(m, current) < 0 ? current : m)));
        }

        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireClass<TSource>? ω = null)
            where TSource : class
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (min, current) => (from m in min from c in current select comparer.Compare(m, c) < 0 ? c : m).OrElse(current));
        }

        public static Option<TSource> MinOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (min, current) => (from m in min from c in current select comparer.Compare(m, c) < 0 ? c : m).OrElse(current));
        }

        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(Option<int>.None(), (min, current) => Option.Some(min.Match(selector(current), m => selector(current) < m ? selector(current) : m)));

        public static Option<int> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.Aggregate(Option<int>.None(), (min, current) => (from m in min from c in selector(current) select c < m ? c : m).OrElse(selector(current)));

        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(Option<long>.None(), (min, current) => Option.Some(min.Match(selector(current), m => selector(current) < m ? selector(current) : m)));

        public static Option<long> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.Aggregate(Option<long>.None(), (min, current) => (from m in min from c in selector(current) select c < m ? c : m).OrElse(selector(current)));

        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(Option<float>.None(), (min, current) => Option.Some(min.Match(selector(current), m => selector(current) < m ? selector(current) : m)));

        public static Option<float> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.Aggregate(Option<float>.None(), (min, current) => (from m in min from c in selector(current) select c < m ? c : m).OrElse(selector(current)));

        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(Option<double>.None(), (min, current) => Option.Some(min.Match(selector(current), m => selector(current) < m ? selector(current) : m)));

        public static Option<double> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.Aggregate(Option<double>.None(), (min, current) => (from m in min from c in selector(current) select c < m ? c : m).OrElse(selector(current)));

        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(Option<decimal>.None(), (min, current) => Option.Some(min.Match(selector(current), m => selector(current) < m ? selector(current) : m)));

        public static Option<decimal> MinOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.Aggregate(Option<decimal>.None(), (min, current) => (from m in min from c in selector(current) select c < m ? c : m).OrElse(selector(current)));

        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireClass<TResult>? ω = null)
            where TResult : class
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (min, current) => Option.Some(min.Match(selector(current), m => comparer.Compare(m, selector(current)) < 0 ? selector(current) : m)));
        }

        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (min, current) => Option.Some(min.Match(selector(current), m => comparer.Compare(m, selector(current)) < 0 ? selector(current) : m)));
        }

        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireClass<TResult>? ω = null)
            where TResult : class
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (min, current) => (from m in min from c in selector(current) select comparer.Compare(m, c) < 0 ? c : m).OrElse(selector(current)));
        }

        public static Option<TResult> MinOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (min, current) => (from m in min from c in selector(current) select comparer.Compare(m, c) < 0 ? c : m).OrElse(selector(current)));
        }
    }
}
