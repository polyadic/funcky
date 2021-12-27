namespace Funcky.Internal
{
    internal static class Mixer
    {
        public static IEnumerable<T> ToRandomEnumerable<T>(IList<T> source, Random random)
        {
            foreach (var (currentIndex, randomIndex) in source.RandomIndex(random))
            {
                yield return source[randomIndex];

                source[randomIndex] = source[currentIndex];
            }
        }

        private static IEnumerable<(int Index, int Random)> RandomIndex<T>(this ICollection<T> buffer, Random random)
            => Enumerable.Repeat(Unit.Value, buffer.Count)
                .Select((_, i) => (i, random.Next(i, buffer.Count)));
    }
}
