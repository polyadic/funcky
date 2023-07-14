namespace Funcky.Analyzers.Test;

public sealed partial class AlternativeMonadAnalyzerTest
{
    // language=csharp
    private const string OptionStubCode =
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

                public Option<TResult> SelectMany<TResult>(System.Func<TItem, Option<TResult>> selector) where TResult : notnull => default;
            }

            public static class OptionExtensions
            {
                public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireStruct<TItem>? ω = null) where TItem : struct => default;

                public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireClass<TItem>? ω = null) where TItem : class => default;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;

                public static Option<TItem> Return<TItem>(TItem value) where TItem : notnull => default;
            }

            public readonly struct Either<TLeft, TRight>
                where TLeft : notnull
                where TRight : notnull
            {
                public static Either<TLeft, TRight> Left(TLeft left) => default;

                public static Either<TLeft, TRight> Right(TRight right) => default;

                public TRight GetOrElse(TRight fallback) => default!;

                public TRight GetOrElse(System.Func<TLeft, TRight> fallback) => default!;

                public Either<TLeft, TRight> OrElse(Either<TLeft, TRight> fallback) => default;

                public Either<TLeft, TRight> OrElse(System.Func<TLeft, Either<TLeft, TRight>> fallback) => default;

                public Either<TLeft, TResult> SelectMany<TResult>(System.Func<TRight, Either<TLeft, TResult>> selector) where TResult : notnull => default;

                public TMatchResult Match<TMatchResult>(System.Func<TLeft, TMatchResult> left, System.Func<TRight, TMatchResult> right) => default!;
            }

            public static class Either<TLeft>
                where TLeft : notnull
            {
                public static Either<TLeft, TRight> Return<TRight>(TRight right)
                    where TRight : notnull
                    => default;
            }

            public readonly struct Result<TValidResult>
                where TValidResult : notnull
            {
                public static Result<TValidResult> Error(System.Exception exception) => default;

                public Result<TValidResult> OrElse(Result<TValidResult> fallback) => default;

                public Result<TValidResult> OrElse(System.Func<System.Exception, Result<TValidResult>> fallback) => default;

                public TValidResult GetOrElse(TValidResult fallback) => default!;

                public TValidResult GetOrElse(System.Func<System.Exception, TValidResult> fallback) => default!;

                public Result<TResult> SelectMany<TResult>(System.Func<TValidResult, Result<TResult>> selector) where TResult : notnull => default;

                public TMatchResult Match<TMatchResult>(System.Func<TValidResult, TMatchResult> ok, System.Func<System.Exception, TMatchResult> error) => default!;
            }

            public static class Result
            {
                public static Result<TValidResult> Ok<TValidResult>(TValidResult result) where TValidResult : notnull => default;

                public static Result<TValidResult> Return<TValidResult>(TValidResult result) where TValidResult : notnull => default;
            }

            public sealed class RequireStruct<T> where T : struct { }

            public sealed class RequireClass<T> where T : class { }
        }

        namespace Funcky
        {
            public static class Functional
            {
                public static T Identity<T>(T x) => x;
            }
        }
        """;
}
