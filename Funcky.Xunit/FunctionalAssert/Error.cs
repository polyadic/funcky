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

    /// <summary>Asserts that the given <paramref name="result"/> is <c>Error</c> and that the error is exactly of type <typeparamref name="TException"/>, like <c>Assert.Throws</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="result"/> is <c>Ok</c> or when the error is of another type.</exception>
    /// <returns>Returns the error in <paramref name="result"/> typed as <typeparamref name="TException"/>.</returns>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TException Error<TException, TValidResult>(Result<TValidResult> result)
        where TException : Exception
        where TValidResult : notnull
    {
        try
        {
            return result.Match(
                error: static exception => exception.GetType() == typeof(TException)
                    ? (TException)exception
                    : throw FunctionalAssertException.ForMismatchedValues(
                        expected: $"Error({typeof(TException).Name})",
                        actual: $"Error({exception.GetType().Name})"),
                ok: static value => throw FunctionalAssertException.ForMismatchedValues(
                    expected: $"Error({typeof(TException).Name})",
                    actual: $"Ok({Format(value)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
