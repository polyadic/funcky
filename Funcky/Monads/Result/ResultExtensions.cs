namespace Funcky.Monads;

public static partial class ResultExtensions
{
    public static Result<T> Flatten<T>(this Result<Result<T>> result)
        where T : notnull
#pragma warning disable λ1018 // This is the implementation of Flatten.
        => result.SelectMany(Identity);
#pragma warning restore λ1018
}
