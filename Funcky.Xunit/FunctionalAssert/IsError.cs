using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        /// <summary>Asserts that the given <paramref name="result"/> is <c>Error</c>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="result"/> is <c>Ok</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
        #else
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        public static Exception IsError<TResult>(Result<TResult> result)
        {
            try
            {
                return result.Match(
                    error: Identity,
                    ok: static value => throw new AssertActualExpectedException(
                        expected: "Error(...)",
                        actual: $"Ok({value})",
                        userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsError)}() Failure",
                        expectedTitle: null, // The other constructor overload is missing in 2.4.2-pre.12. See https://github.com/xunit/xunit/issues/2449
                        actualTitle: null,
                        innerException: null));
            }
            catch (AssertActualExpectedException exception)
            {
                throw exception;
            }
        }
    }
}
