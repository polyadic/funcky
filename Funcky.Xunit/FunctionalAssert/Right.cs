using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="either"/> is <c>Right</c> and contains the given <paramref name="expectedRight"/>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="either"/> is <c>Left</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public static void Right<TLeft, TRight>(TRight expectedRight, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            Assert.Equal(Either<TLeft, TRight>.Right(expectedRight), either);
        }
        catch (EqualException exception)
        {
            throw new AssertActualExpectedException(
                expected: exception.Expected,
                actual: exception.Actual,
                userMessage: $"{nameof(FunctionalAssert)}.{nameof(Right)}() Failure");
        }
    }

    /// <summary>Asserts that the given <paramref name="either"/> is <c>Right</c>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="either"/> is <c>Left</c>.</exception>
    /// <returns>Returns the value in <paramref name="either"/> if it was <c>Right</c>.</returns>
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
            return either.Match(
                right: Identity,
                left: static left => throw new AssertActualExpectedException(
                    expected: "Right(...)",
                    actual: $"Left({left})",
                    userMessage: $"{nameof(FunctionalAssert)}.{nameof(Right)}() Failure"));
        }
        catch (AssertActualExpectedException exception)
        {
            throw exception;
        }
    }
}
