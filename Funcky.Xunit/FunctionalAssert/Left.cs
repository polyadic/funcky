using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="either"/> is <c>Left</c> and contains the given <paramref name="expectedLeft"/>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="either"/> is <c>Right</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public static void Left<TLeft, TRight>(TLeft expectedLeft, Either<TLeft, TRight> either)
    {
        try
        {
            Assert.Equal(Either<TLeft, TRight>.Left(expectedLeft), either);
        }
        catch (EqualException exception)
        {
            throw new AssertActualExpectedException(
                expected: exception.Expected,
                actual: exception.Actual,
                userMessage: $"{nameof(FunctionalAssert)}.{nameof(Left)}() Failure");
        }
    }

    /// <summary>Asserts that the given <paramref name="either"/> is <c>Left</c>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="either"/> is <c>Right</c>.</exception>
    /// <returns>Returns the value in <paramref name="either"/> if it was <c>Left</c>.</returns>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TLeft Left<TLeft, TRight>(Either<TLeft, TRight> either)
    {
        try
        {
            return either.Match(
                left: Identity,
                right: static right => throw new AssertActualExpectedException(
                    expected: "Left(...)",
                    actual: $"Right({right})",
                    userMessage: $"{nameof(FunctionalAssert)}.{nameof(Left)}() Failure"));
        }
        catch (AssertActualExpectedException exception)
        {
            throw exception;
        }
    }
}
