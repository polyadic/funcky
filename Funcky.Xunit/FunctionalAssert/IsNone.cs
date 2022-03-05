using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace Funcky
{
    public static partial class FunctionalAssert
    {
        /// <summary>Asserts that the given <paramref name="option"/> is <c>None</c>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="option"/> is <c>Some</c>.</exception>
        #if STACK_TRACE_HIDDEN_SUPPORTED
        [System.Diagnostics.StackTraceHidden]
        #else
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                option.Match(
                    none: NoOperation,
                    some: static value => throw new AssertActualExpectedException(
                        expected: "None",
                        actual: $"Some({value})",
                        userMessage: $"{nameof(FunctionalAssert)}.{nameof(IsNone)}() Failure"));
            }
            catch (AssertActualExpectedException exception)
            {
                throw exception;
            }
        }
    }
}
