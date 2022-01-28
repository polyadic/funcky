namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<TAccumulate> InclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> elements, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            foreach (var element in elements)
            {
                seed = accumulator(seed, element);
                yield return seed;
            }
        }

        public static IEnumerable<TAccumulate> ExclusiveScan<TSource, TAccumulate>(this IEnumerable<TSource> elements, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            foreach (var element in elements)
            {
                yield return seed;
                seed = accumulator(seed, element);
            }
        }
    }
}
