using System;

namespace Funcky.Monads.IO
{
    public delegate T Io<out T>();

    public static class IoExtensions
    {
        public static Io<TResult> SelectMany<TSource, TSelector, TResult>(
            this Io<TSource> source,
            Func<TSource, Io<TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector)
            => ()
                => ResultSelector(resultSelector, selector, source());

        // Type Constructor
        public static Io<TSource> Io<TSource>(this TSource value) => () => value;

        public static Io<TResult> Select<TSource, TResult>(
            this Io<TSource> source, Func<TSource, TResult> selector) =>
            source.SelectMany(value => selector(value).Io(), (value, result) => result);

        private static TResult ResultSelector<TSource, TSelector, TResult>(Func<TSource, TSelector, TResult> resultSelector, Func<TSource, Io<TSelector>> selector, TSource value)
            => resultSelector(value, selector(value)());
    }
}
