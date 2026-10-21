#if !RANDOM_SHUFFLE
namespace Funcky.Internal;

/// <summary>Polyfill of <c>Random.Shuffle(T[])</c>, which exists since .NET 8, with the same draw order.</summary>
internal static class Mixer
{
    extension(Random random)
    {
        /// <summary>Shuffles the array in place in O(n) with a Fisher-Yates shuffle, so every permutation is equally likely.</summary>
        public void Shuffle<TSource>(TSource[] values)
        {
            for (var currentIndex = 0; currentIndex < values.Length - 1; currentIndex++)
            {
                var randomIndex = random.Next(currentIndex, values.Length);

                if (randomIndex != currentIndex)
                {
                    (values[currentIndex], values[randomIndex]) = (values[randomIndex], values[currentIndex]);
                }
            }
        }
    }
}
#endif
