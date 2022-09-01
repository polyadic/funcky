namespace Funcky.Monads;

public static partial class ReaderExtensions
{
    [Pure]
    public static Reader<TEnvironment, TResult> Select<TEnvironment, TSource, TResult>(
        this Reader<TEnvironment, TSource> source, Func<TSource, TResult> selector)
        where TEnvironment : notnull
        where TSource : notnull
        where TResult : notnull
        => source
            .SelectMany(value => Reader<TEnvironment>.Return(selector(value)), (_, result) => result);

    [Pure]
    public static Reader<TEnvironment, TResult> SelectMany<TEnvironment, TSource, TResult>(
        this Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TResult>> selector)
        where TEnvironment : notnull
        where TSource : notnull
        where TResult : notnull
        => source.SelectMany(selector, (_, result) => result);

    [Pure]
    public static Reader<TEnvironment, TResult> SelectMany<TEnvironment, TSource, TReader, TResult>(
        this Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TReader>> selector,
        Func<TSource, TReader, TResult> resultSelector)
        where TEnvironment : notnull
        where TSource : notnull
        where TReader : notnull
        where TResult : notnull
            => environment
                =>
                {
                    var value = source(environment);
                    return resultSelector(value, selector(value)(environment));
                };
}
