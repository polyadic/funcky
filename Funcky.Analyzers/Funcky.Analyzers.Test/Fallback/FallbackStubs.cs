namespace Funcky.Analyzers.Test.Fallback;

internal static class FallbackStubs
{
    // language=csharp
    public const string MonadStubCode =
        """

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public static Option<TItem> None => default;

                public static implicit operator Option<TItem>(TItem item) => default;

                public TResult Match<TResult>(TResult none, System.Func<TItem, TResult> some) => default!;

                public TResult Match<TResult>(System.Func<TResult> none, System.Func<TItem, TResult> some) => default!;

                public TItem GetOrElse(TItem fallback) => default!;

                public TItem GetOrElse(System.Func<TItem> fallback) => default!;

                public Option<TItem> OrElse(Option<TItem> fallback) => default!;

                public Option<TItem> OrElse(System.Func<Option<TItem>> fallback) => default!;

                public Option<TResult> AndThen<TResult>(System.Func<TItem, TResult> selector) where TResult : notnull => default;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;
            }

            public readonly struct Either<TLeft, TRight>
                where TLeft : notnull
                where TRight : notnull
            {
                public static Either<TLeft, TRight> Left(TLeft left) => default;

                public TRight GetOrElse(TRight fallback) => default!;

                public TRight GetOrElse(System.Func<TLeft, TRight> fallback) => default!;

                public Either<TLeft, TRight> OrElse(Either<TLeft, TRight> fallback) => default;

                public Either<TLeft, TRight> OrElse(System.Func<TLeft, Either<TLeft, TRight>> fallback) => default;

                public TMatchResult Match<TMatchResult>(System.Func<TLeft, TMatchResult> left, System.Func<TRight, TMatchResult> right) => default!;
            }

            public readonly struct Result<TValidResult>
                where TValidResult : notnull
            {
                public static Result<TValidResult> Error(System.Exception exception) => default;

                public Result<TValidResult> OrElse(Result<TValidResult> fallback) => default;

                public Result<TValidResult> OrElse(System.Func<System.Exception, Result<TValidResult>> fallback) => default;

                public TValidResult GetOrElse(TValidResult fallback) => default!;

                public TValidResult GetOrElse(System.Func<System.Exception, TValidResult> fallback) => default!;

                public TMatchResult Match<TMatchResult>(System.Func<System.Exception, TMatchResult> error, System.Func<TValidResult, TMatchResult> ok) => default!;
            }
        }
        """;
}
