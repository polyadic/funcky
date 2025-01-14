using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="result"/> is <c>Error</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="result"/> is <c>Ok</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static Exception Error<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            return result.Match(
                error: Identity,
                ok: static value => throw FunctionalAssertException.ForMismatchedValues(
                    expected: "Error(...)",
                    actual: $"Ok({Format(value)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
