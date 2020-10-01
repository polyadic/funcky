using System.Runtime.CompilerServices;
using Funcky.Monads;
using Xunit;
using Xunit.Sdk;
using static Funcky.Functional;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
