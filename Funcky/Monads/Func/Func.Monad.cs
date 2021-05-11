using System;

namespace Funcky.Monads
{
    public static partial class FuncExtensions
    {
        public static Func<TResult> SelectMany<TSource, TSelector, TResult>(
            this Func<TSource> source,
            Func<TSource, Func<TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector)
            => ()
                => ResultSelector(resultSelector, selector, source());

        public static Func<TResult> Select<TSource, TResult>(
            this Func<TSource> source,
            Func<TSource, TResult> selector)
            => source.SelectMany(value => selector(value).Func(), (value, result) => result);

        public static Func<TSource> Func<TSource>(this TSource value)
            => ()
                => value;

        private static TResult ResultSelector<TSource, TSelector, TResult>(
            Func<TSource, TSelector, TResult> resultSelector,
            Func<TSource, Func<TSelector>> selector,
            TSource value)
            => resultSelector(value, selector(value)());
    }
}
