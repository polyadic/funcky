using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Scan generates a sequence known as the inclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">a binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IEnumerable<TAccumulate> InclusiveScan<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            => InclusiveScanIterator(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));

        /// <summary>
        /// Scan generates a sequence known as the exclusive prefix sum.
        /// </summary>
        /// <typeparam name="TAccumulate">The seed and target type.</typeparam>
        /// <param name="seed">The seed or neutral element (identity).</param>
        /// <param name="accumulator">a binary operator to aggregate a value.</param>
        /// <returns>A sequence of aggregated values.</returns>
        public IEnumerable<TAccumulate> ExclusiveScan<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            => ExclusiveScanIterator(ArgumentValidator.NotNull(source), seed, ArgumentValidator.NotNull(accumulator));
    }

    private static IEnumerable<TAccumulate> ExclusiveScanIterator<TSource, TAccumulate>(IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return seed;
            seed = accumulator(seed, enumerator.Current);
        }
    }

    private static IEnumerable<TAccumulate> InclusiveScanIterator<TSource, TAccumulate>(IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            seed = accumulator(seed, enumerator.Current);
            yield return seed;
        }
    }
}
