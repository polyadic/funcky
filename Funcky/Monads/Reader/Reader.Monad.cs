namespace Funcky.Monads;

public static partial class ReaderExtensions
{
    [Pure]
    public static Reader<TEnvironment, TResult> Select<TEnvironment, TSource, TResult>(
        this Reader<TEnvironment, TSource> source, Func<TSource, TResult> selector)
        => source
            .SelectMany(value => Reader<TEnvironment>.Return(selector(value)), (_, result) => result);

    [Pure]
    public static Reader<TEnvironment, TResult> SelectMany<TEnvironment, TSource, TResult>(
        this Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TResult>> selector)
        => source.SelectMany(selector, (_, result) => result);

    [Pure]
    public static Reader<TEnvironment, TResult> SelectMany<TEnvironment, TSource, TReader, TResult>(
        this Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TReader>> selector,
        Func<TSource, TReader, TResult> resultSelector)
            => environment
                =>
                {
                    var value = source(environment);
                    return resultSelector(value, selector(value)(environment));
                };
}
