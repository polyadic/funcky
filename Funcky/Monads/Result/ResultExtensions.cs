namespace Funcky.Monads;

public static partial class ResultExtensions
{
    extension<T>(Result<Result<T>> result)
        where T : notnull
    {
        public Result<T> Flatten()
#pragma warning disable λ1018 // This is the implementation of Flatten.
            => result.SelectMany(Identity);
    }
#pragma warning restore λ1018
}
