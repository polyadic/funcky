using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        public static TResult IsOk<TResult>(Result<TResult> result)
        {
            try
            {
                return result.Match(
                    ok: Identity,
                    error: exception => throw new IsOkException(exception));
            }
            catch (IsOkException exception)
            {
                throw exception;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsOk<TResult>(TResult expectedResult, Result<TResult> result)
        {
            try
            {
                Assert.Equal(Result.Ok(expectedResult), result);
            }
            catch (EqualException)
            {
                throw new IsOkWithExpectedValueException(expectedResult, result.Select(value => (object?)value));
            }
        }
    }
}
