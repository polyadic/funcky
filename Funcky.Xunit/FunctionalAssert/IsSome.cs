using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky
{
    public static partial class FunctionalAssert
    {
        /// <summary>Asserts that the given <paramref name="option"/> is <c>Some</c> and contains the given <paramref name="expectedValue"/>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when the option is <c>None</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
        #else
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public static void IsSome<TItem>(TItem expectedValue, Option<TItem> option)
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
                    userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsSome)}() Failure",
                    expectedTitle: null, // The other constructor overload is missing in 2.4.2-pre.12. See https://github.com/xunit/xunit/issues/2449
                    actualTitle: null,
                    innerException: null);
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
        public static TItem IsSome<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                return option.Match(
                    some: Identity,
                    none: static () => throw new AssertActualExpectedException(
                        expected: "Some(...)",
                        actual: "None",
                        userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsSome)}() Failure",
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
