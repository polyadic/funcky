namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Scan generates a sequence known as the inclusive prefix sum.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The seed or neutral element (identity).</param>
    /// <param name="accumulator">a binary operator to aggregate a value.</param>
    /// <returns>A sequence of aggregated values.</returns>
    public static IEnumerable<TAccumulate> InclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            seed = accumulator(seed, enumerator.Current);
            yield return seed;
        }
    }

    /// <summary>
    /// Scan generates a sequence known as the exclusive prefix sum.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The seed or neutral element (identity).</param>
    /// <param name="accumulator">a binary operator to aggregate a value.</param>
    /// <returns>A sequence of aggregated values.</returns>
    public static IEnumerable<TAccumulate> ExclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return seed;
            seed = accumulator(seed, enumerator.Current);
        }
    }
}
