using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c> and contains the given <paramref name="expectedValue"/>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when the option is <c>None</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public static void Some<TItem>(TItem expectedValue, Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            Assert.Equal(Option.Some(expectedValue), option);
        }
        catch (EqualException exception)
        {
            throw new AssertActualExpectedException(
                expected: exception.Expected,
                actual: exception.Actual,
                userMessage: $"{nameof(FunctionalAssert)}.{nameof(Some)}() Failure");
        }
    }

    /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c>.</summary>
    /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="option"/> is <c>None</c>.</exception>
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
            return option.GetOrElse(static () => throw new AssertActualExpectedException(
                expected: "Some(...)",
                actual: "None",
                userMessage: $"{nameof(FunctionalAssert)}.{nameof(Some)}() Failure"));
        }
        catch (AssertActualExpectedException exception)
        {
            throw exception;
        }
    }
}
