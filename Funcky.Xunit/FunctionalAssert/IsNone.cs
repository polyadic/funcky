using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        /// <summary>Asserts that the given <paramref name="option"/> is <c>None</c>.</summary>
        /// <exception cref="AssertActualExpectedException">Thrown when <paramref name="option"/> is <c>Some</c>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
