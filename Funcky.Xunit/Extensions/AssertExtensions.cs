using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;
using static Xunit.Sdk.ArgumentFormatter;

namespace Funcky.Extensions;

/// <summary>Assertions for Funcky's monads as static extension members of xunit's <see cref="Assert"/>,
/// so they are found where the other assertions live: <c>Assert.Some(option)</c>.</summary>
public static class AssertExtensions
{
    extension(Assert)
    {
        /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c> and contains the given <paramref name="expectedValue"/>.</summary>
        /// <exception cref="XunitException">Thrown when the option is <c>None</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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
        [AssertMethodHasOverloadWithExpectedValue]
        [Pure]
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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

        /// <summary>Asserts that the given <paramref name="option"/> is <c>None</c>.</summary>
        /// <exception cref="XunitException">Thrown when <paramref name="option"/> is <c>Some</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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

        /// <summary>Asserts that the given <paramref name="result"/> is <c>Ok</c> and contains the given <paramref name="expectedResult"/>.</summary>
        /// <exception cref="XunitException">Thrown when <paramref name="result"/> is <c>Error</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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
        [AssertMethodHasOverloadWithExpectedValue]
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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

        /// <summary>Asserts that the given <paramref name="either"/> is <c>Left</c> and contains the given <paramref name="expectedLeft"/>.</summary>
        /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Right</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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
        [AssertMethodHasOverloadWithExpectedValue]
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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

        /// <summary>Asserts that the given <paramref name="either"/> is <c>Right</c> and contains the given <paramref name="expectedRight"/>.</summary>
        /// <exception cref="XunitException">Thrown when <paramref name="either"/> is <c>Left</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
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

    private static void EqualOrThrow<T>(T expected, T actual, Action @throw)
    {
        try
        {
            Assert.Equal(expected, actual);
        }
        catch (XunitException)
        {
            @throw();
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
