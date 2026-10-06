#if ASYNC_SUPPORTED
namespace Funcky.Async.Test.TestUtilities;

/// <summary>Skip reasons for tests that only fail on one of the two async implementations. A <see langword="null"/> reason runs the test.</summary>
internal static class KnownIssues
{
#if SYSTEM_LINQ_ASYNC
    public const string? CycleBufferRewindsExhaustedSource = null;
#else
    public const string? CycleBufferRewindsExhaustedSource = "Tofix: the cycle buffer calls MoveNextAsync again on an exhausted .NET 10 iterator, which rewinds it";
#endif
}
#endif
