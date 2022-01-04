using System.Collections.Immutable;

namespace Funcky.Internal
{
    internal static class Mixer
    {
        public static IEnumerable<TSource> ToRandomEnumerable<TSource>(IList<TSource> source, Random random)
            => Enumerable
                .Range(0, source.Count)
                .Aggregate(ImmutableList<TSource>.Empty, AggregateShuffle(source, random));

        public static Func<ImmutableList<TSource>, int, ImmutableList<TSource>> AggregateShuffle<TSource>(IList<TSource> source, Random random)
            => (shuffled, currentIndex)
                => shuffled.Add(UseElement(source, currentIndex, random.Next(currentIndex, source.Count)));

        private static TSource UseElement<TSource>(IList<TSource> source, int currentIndex, int randomIndex)
        {
            var result = source[randomIndex];

            source[randomIndex] = source[currentIndex];

            return result;
        }
    }
}
