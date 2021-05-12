using System;

namespace Funcky.Monads
{
    public static partial class ReaderExtensions
    {
        public static Reader<TEnvironment, TResult> Select<TEnvironment, TSource, TResult>(
            this Reader<TEnvironment, TSource> source, Func<TSource, TResult> selector)
            => source
                .SelectMany(value => selector(value).Reader<TEnvironment, TResult>(), (_, result) => result);

        public static Reader<TEnvironment, TResult> SelectMany<TEnvironment, TSource, TSelector, TResult>(
            this Reader<TEnvironment, TSource> source,
            Func<TSource, Reader<TEnvironment, TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector)
                => environment
                    =>
                    {
                        TSource value = source(environment);
                        return resultSelector(value, selector(value)(environment));
                    };

        public static Reader<TEnvironment, TSource> Reader<TEnvironment, TSource>(this TSource value)
            => _
                => value;
    }
}
