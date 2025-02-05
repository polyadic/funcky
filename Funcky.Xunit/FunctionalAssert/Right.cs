using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="either"/> is <c>Right</c> and contains the given <paramref name="expectedRight"/>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Left</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Right<TLeft, TRight>(TRight expectedRight, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            either.Switch(
                left: left => throw FunctionalAssertException.ForMismatchedValues(
                    expected: $"Right({Format(expectedRight)})",
                    actual: $"Left({Format(left)})"),
                right: right => EqualOrThrow(
                    expected: expectedRight,
                    actual: right,
                    () => throw FunctionalAssertException.ForMismatchedValues(
                        expected: $"Right({Format(expectedRight)})",
                        actual: $"Right({Format(right)})")));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <summary>Asserts that the given <paramref name="either"/> is <c>Right</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Left</c>.</exception>
    /// <returns>Returns the value in <paramref name="either"/> if it was <c>Right</c>.</returns>
    [AssertMethodHasOverloadWithExpectedValue]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TRight Right<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            return either.GetOrElse(
                static left => throw FunctionalAssertException.ForMismatchedValues(
                    expected: "Right(...)",
                    actual: $"Left({Format(left)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
