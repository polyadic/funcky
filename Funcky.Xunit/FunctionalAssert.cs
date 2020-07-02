using System;
using System.Diagnostics.Contracts;
using Funcky.Monads;
using Xunit;
using Xunit.Sdk;
using static Funcky.Functional;

namespace Funcky.Xunit
{
    public static class FunctionalAssert
    {
        public static void Unmatched()
            => throw new UnmatchedException();

        public static void Unmatched(string unmatchedCase)
            => throw new UnmatchedException(unmatchedCase);

        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(Option<TItem>.None(), option);

        public static void IsSome<TItem>(TItem expectedValue, Option<TItem> option)
            where TItem : notnull
            => Assert.Equal(Option.Some(expectedValue), option);

        [Pure]
        public static TItem IsSome<TItem>(Option<TItem> option)
            where TItem : notnull
            => option.Match(
                   some: Identity,
                   none: () => throw new XunitException("Expected option to have a value, but was None instead"));

        public static Exception IsError<TResult>(Result<TResult> result)
            => result.Match(
                   error: Identity,
                   ok: value => throw new XunitException($"Expected result to be an Error, but got a '{value}' instead"));

        public static void IsError<TResult>(Exception expectedException, Result<TResult> result)
            => Assert.True(result.Match(ok: False, error: exception => expectedException == exception));

        public static TResult IsOk<TResult>(Result<TResult> result)
            => result.Match(
                   ok: Identity,
                   error: _ => throw new XunitException("Expected result to be an Ok, but got an Error instead"));

        public static void IsOk<TResult>(TResult expectedResult, Result<TResult> result)
            => Assert.Equal(Result.Ok(expectedResult), result);
    }
}
