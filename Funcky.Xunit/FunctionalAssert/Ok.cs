using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <summary>Asserts that the given <paramref name="result"/> is <c>Ok</c> and contains the given <paramref name="expectedResult"/>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="result"/> is <c>Error</c>.</exception>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Ok<TValidResult>(TValidResult expectedResult, Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            result.Switch(
                error: exception => throw FunctionalAssertException.ForMismatchedValues(
                    expected: $"Ok({Format(expectedResult)})",
                    actual: $"Error({FormatException(exception)})"),
                ok: value => EqualOrThrow(
                    expected: expectedResult,
                    actual: value,
                    () => throw FunctionalAssertException.ForMismatchedValues(
                        expected: $"Ok({Format(expectedResult)})",
                        actual: $"Ok({Format(value)})")));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <summary>Asserts that the given <paramref name="result"/> is <c>Ok</c>.</summary>
    /// <exception cref="XunitException">Thrown when <paramref name="result"/> is <c>Error</c>.</exception>
    /// <returns>Returns the value in <paramref name="result"/> if it was <c>Ok</c>.</returns>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #else
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TValidResult Ok<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            return result.GetOrElse(
                static exception => throw FunctionalAssertException.ForMismatchedValues(
                    expected: "Ok(...)",
                    actual: $"Error({FormatException(exception)})"));
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    private static string FormatException(Exception exception)
        => $"{FormatTypeName(exception.GetType())}: {Format(exception.Message)}";

    private static string FormatTypeName(Type type)
    {
        // Xunit's `Format` takes care of niceties like C# type names,
        // array types, nullable, etc. but adds typeof(...) around the name
        // which is unsuitable for our case.
        const string prefix = "typeof(";
        const string suffix = ")";
        var formatted = Format(type);
        return formatted.StartsWith(prefix) && formatted.EndsWith(suffix)
            ? formatted[prefix.Length..^suffix.Length]
            : formatted;
    }
}
