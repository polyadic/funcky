namespace Funcky.Monads;

public static partial class ResultExtensions
{
    public static Result<T> Flatten<T>(this Result<Result<T>> result)
        where T : notnull
        => result.SelectMany(Identity);
}
