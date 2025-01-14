using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="either"/> is <c>Left</c> and contains the given <paramref name="expectedLeft"/>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Right</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Left<TLeft, TRight>(TLeft expectedLeft, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            either.Switch(
                right: right => throw FunctionalAssertException.ForMismatchedValues(
                    expected: $"Left({Format(expectedLeft)})",
                    actual: $"Right({Format(right)})"),
                left: left => EqualOrThrow(
                    expected: expectedLeft,
                    actual: left,
                    () => throw FunctionalAssertException.ForMismatchedValues(
                        expected: $"Left({Format(expectedLeft)})",
                        actual: $"Left({Format(left)})")));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <summary>Asserts that the given <paramref name="either"/> is <c>Left</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Right</c>.</exception>
    /// <returns>Returns the value in <paramref name="either"/> if it was <c>Left</c>.</returns>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TLeft Left<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            return either.Match(
                left: Identity,
                right: static right => throw FunctionalAssertException.ForMismatchedValues(
                    expected: "Left(...)",
                    actual: $"Right({Format(right)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
