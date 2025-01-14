using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="option"/> is <c>None</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="option"/> is <c>Some</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void None<TItem>(Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            option.AndThen(
                static value => throw FunctionalAssertException.ForMismatchedValues(
                    expected: "None",
                    actual: $"Some({Format(value)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
