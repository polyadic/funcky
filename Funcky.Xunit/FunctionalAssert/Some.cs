using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c> and contains the given <paramref name="expectedValue"/>.</summary>
    /// <exception cref="XunitException">Thrown when the option is <c>None</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Some<TItem>(TItem expectedValue, Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            option.Switch(
                none: () => throw FunctionalAssertException.ForMismatchedValues(
                    expected: $"Some({Format(expectedValue)})",
                    actual: "None"),
                some: value => EqualOrThrow(
                    expected: expectedValue,
                    actual: value,
                    () => throw FunctionalAssertException.ForMismatchedValues(
                        expected: $"Some({Format(expectedValue)})",
                        actual: $"Some({Format(value)})")));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="option"/> is <c>None</c>.</exception>
    /// <returns>Returns the value in <paramref name="option"/> if it was <c>Some</c>.</returns>
    [Pure]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TItem Some<TItem>(Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            return option.GetOrElse(static () => throw FunctionalAssertException.ForMismatchedValues(
                expected: "Some(...)",
                actual: "None"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
