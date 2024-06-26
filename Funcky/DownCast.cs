namespace Funcky;

public static class DownCast<TResult>
    where TResult : class
{
    public static Option<TResult> From<TItem>(Option<TItem> option)
        where TItem : class
        => option.SelectMany(OptionDownCast);

    public static Result<TResult> From<TItem>(Result<TItem> result)
        where TItem : class
        => result.SelectMany(ResultDownCast);

    public static Either<TLeft, TResult> From<TLeft, TRight>(Either<TLeft, TRight> either, Func<TLeft> failedCast)
        where TRight : class
        where TLeft : notnull
        => either.SelectMany(EitherDownCast<TLeft, TRight>(failedCast));

    private static Option<TResult> OptionDownCast<TItem>(TItem item)
        where TItem : class
        => Option.FromNullable(item as TResult);

    private static Result<TResult> ResultDownCast<TValidResult>(TValidResult result)
        where TValidResult : class
        => result as TResult
           ?? Result<TResult>.Error(new InvalidCastException());

    private static Func<TRight, Either<TLeft, TResult>> EitherDownCast<TLeft, TRight>(Func<TLeft> failedCast)
        where TRight : class
        where TLeft : notnull
        => right
            => right as TResult
               ?? Either<TLeft, TResult>.Left(failedCast());
}
