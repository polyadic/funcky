using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Funcky.Monads;
using Xunit;
using Xunit.Sdk;
using static Funcky.Functional;

namespace Funcky.Xunit
{
    public static class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                option.Match(
                    none: NoOperation,
                    some: value => throw new IsNoneException(value));
            }
            catch (IsNoneException exception)
            {
                throw exception;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsSome<TItem>(TItem expectedValue, Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                Assert.Equal(Option.Some(expectedValue), option);
            }
            catch (EqualException)
            {
                throw new IsSomeWithExpectedValueException(expectedValue, option);
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItem IsSome<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                return option.Match(
                    some: Identity,
                    none: () => throw new IsSomeException());
            }
            catch (IsSomeException exception)
            {
                throw exception;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception IsError<TResult>(Result<TResult> result)
        {
            try
            {
                return result.Match(
                    error: Identity,
                    ok: value => throw new IsErrorException(value));
            }
            catch (IsErrorException exception)
            {
                throw exception;
            }
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Unmatched()
            => throw new UnmatchedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Unmatched(string unmatchedCase)
            => throw new UnmatchedException(unmatchedCase);
    }
}
