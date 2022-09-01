namespace Funcky.Monads;

public delegate TResult Reader<in TEnvironment, out TResult>(TEnvironment environment)
    where TEnvironment : notnull
    where TResult : notnull;

public static class Reader<TEnvironment>
    where TEnvironment : notnull
{
    [Pure]
    public static Reader<TEnvironment, TResult> Return<TResult>(TResult value)
        where TResult : notnull
        => _
            => value;

    [Pure]
    public static Reader<TEnvironment, TResult> FromFunc<TResult>(Func<TEnvironment, TResult> function)
        where TResult : notnull
        => function.Invoke;

    [Pure]
    public static Reader<TEnvironment, Unit> FromAction(Action<TEnvironment> action)
        => ActionToUnit(action).Invoke;
}
