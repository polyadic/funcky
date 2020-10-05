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
        public static Option<int> MaxOrNone(this IEnumerable<int> source)
            => source.Aggregate(Option<int>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        public static Option<int> MaxOrNone(this IEnumerable<Option<int>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        public static Option<long> MaxOrNone(this IEnumerable<long> source)
            => source.Aggregate(Option<long>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        public static Option<long> MaxOrNone(this IEnumerable<Option<long>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        public static Option<double> MaxOrNone(this IEnumerable<double> source)
            => source.Aggregate(Option<double>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        public static Option<double> MaxOrNone(this IEnumerable<Option<double>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        public static Option<float> MaxOrNone(this IEnumerable<float> source)
            => source.Aggregate(Option<float>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        public static Option<float> MaxOrNone(this IEnumerable<Option<float>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        public static Option<decimal> MaxOrNone(this IEnumerable<decimal> source)
            => source.Aggregate(Option<decimal>.None(), (max, current) => Option.Some(max.Match(current, m => current > m ? current : m)));

        public static Option<decimal> MaxOrNone(this IEnumerable<Option<decimal>> source)
            => source.WhereSelect(Identity).MaxOrNone();

        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source, RequireClass<TSource>? ω = null)
            where TSource : class
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (max, current) => Option.Some(max.Match(current, m => comparer.Compare(m, current) > 0 ? current : m)));
        }

        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<TSource> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (max, current) => Option.Some(max.Match(current, m => comparer.Compare(m, current) > 0 ? current : m)));
        }

        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireClass<TSource>? ω = null)
            where TSource : class
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (max, current) => (from m in max from c in current select comparer.Compare(m, c) > 0 ? c : m).OrElse(current));
        }

        public static Option<TSource> MaxOrNone<TSource>(this IEnumerable<Option<TSource>> source, RequireStruct<TSource>? ω = null)
            where TSource : struct
        {
            var comparer = Comparer<TSource>.Default;

            return source.Aggregate(Option<TSource>.None(), (max, current) => (from m in max from c in current select comparer.Compare(m, c) > 0 ? c : m).OrElse(current));
        }

        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => source.Aggregate(Option<int>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        public static Option<int> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<int>> selector)
            => source.Aggregate(Option<int>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => source.Aggregate(Option<long>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        public static Option<long> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<long>> selector)
            => source.Aggregate(Option<long>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => source.Aggregate(Option<float>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        public static Option<float> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<float>> selector)
            => source.Aggregate(Option<float>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => source.Aggregate(Option<double>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        public static Option<double> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<double>> selector)
            => source.Aggregate(Option<double>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => source.Aggregate(Option<decimal>.None(), (max, current) => Option.Some(max.Match(selector(current), m => selector(current) > m ? selector(current) : m)));

        public static Option<decimal> MaxOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, Option<decimal>> selector)
            => source.Aggregate(Option<decimal>.None(), (max, current) => (from m in max from c in selector(current) select c > m ? c : m).OrElse(selector(current)));

        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireClass<TResult>? ω = null)
            where TResult : class
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (max, current) => Option.Some(max.Match(selector(current), m => comparer.Compare(m, selector(current)) > 0 ? selector(current) : m)));
        }

        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (max, current) => Option.Some(max.Match(selector(current), m => comparer.Compare(m, selector(current)) > 0 ? selector(current) : m)));
        }

        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireClass<TResult>? ω = null)
            where TResult : class
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (max, current) => (from m in max from c in selector(current) select comparer.Compare(m, c) > 0 ? c : m).OrElse(selector(current)));
        }

        public static Option<TResult> MaxOrNone<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector, RequireStruct<TResult>? ω = null)
            where TResult : struct
        {
            var comparer = Comparer<TResult>.Default;

            return source.Aggregate(Option<TResult>.None(), (max, current) => (from m in max from c in selector(current) select comparer.Compare(m, c) > 0 ? c : m).OrElse(selector(current)));
        }
    }
}
