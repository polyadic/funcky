using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        /// <summary>Asserts that the given <paramref name="result"/> is <c>Ok</c> and contains the given <paramref name="expectedResult"/>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="result"/> is <c>Error</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
        #else
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public static void IsOk<TResult>(TResult expectedResult, Result<TResult> result)
        {
            try
            {
                Assert.Equal(Result.Ok(expectedResult), result);
            }
            catch (EqualException exception)
            {
                throw new AssertActualExpectedException(
                    expected: exception.Expected,
                    actual: exception.Actual,
                    userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsOk)}() Failure",
                    expectedTitle: null, // The other constructor overload is missing in 2.4.2-pre.12. See https://github.com/xunit/xunit/issues/2449
                    actualTitle: null,
                    innerException: null);
            }
        }

        /// <summary>Asserts that the given <paramref name="result"/> is <c>Ok</c>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="result"/> is <c>Error</c>.</exception>
        /// <returns>Returns the value in <paramref name="result"/> if it was <c>Ok</c>.</returns>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
        #else
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
        public static TResult IsOk<TResult>(Result<TResult> result)
        {
            try
            {
                return result.Match(
                    ok: Identity,
                    error: static exception => throw new AssertActualExpectedException(
                        expected: "Ok(...)",
                        actual: $"Error({FormatException(exception)})",
                        userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsOk)}() Failure",
                        expectedTitle: null, // The other constructor overload is missing in 2.4.2-pre.12. See https://github.com/xunit/xunit/issues/2449
                        actualTitle: null,
                        innerException: null));
            }
            catch (AssertActualExpectedException exception)
            {
                throw exception;
            }
        }

        private static string FormatException(Exception exception)
            => $"{exception.GetType().FullName}: {exception.Message}";
    }
}
