using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Applies an accumulator function over a sequence, using the first element as the seed. If the sequence is empty it returns None.
    /// </summary>
    /// <remarks>
    /// This is the seedless <see cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"/>, which throws for an empty sequence.
    /// The accumulator function is never invoked for a sequence with a single element.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence to aggregate over.</param>
    /// <param name="func">An accumulator function to be invoked on each element after the first.</param>
    /// <returns>The final accumulator value or None.</returns>
    [Pure]
    public static Option<TSource> AggregateOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        where TSource : notnull
    {
        ArgumentValidator.NotNull(source);
        ArgumentValidator.NotNull(func);

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return Option<TSource>.None;
        }

        var accumulator = enumerator.Current;

        while (enumerator.MoveNext())
        {
            accumulator = func(accumulator, enumerator.Current);
        }

        return accumulator;
    }
}
