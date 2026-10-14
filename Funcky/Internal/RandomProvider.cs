namespace Funcky.Internal;

/// <summary>Provides the random number generator used by the overloads that do not take a <see cref="Random"/>.</summary>
internal static class RandomProvider
{
#if RANDOM_SHARED
    public static Random Shared => Random.Shared;
#else
    private static readonly Random Seeder = new();

    [ThreadStatic]
    private static Random? _threadRandom;

    /// <summary>
    /// One generator per thread, seeded from a single process-wide generator. On .NET Framework <c>new Random()</c>
    /// seeds from the clock, so two instances created in the same tick produce the same sequence; a shared instance
    /// avoids that, and keeping it per thread avoids a lock on every draw.
    /// </summary>
    public static Random Shared => _threadRandom ??= CreateThreadRandom();

    private static Random CreateThreadRandom()
    {
        lock (Seeder)
        {
            return new Random(Seeder.Next());
        }
    }
#endif
}
