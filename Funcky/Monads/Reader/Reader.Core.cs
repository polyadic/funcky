namespace Funcky.Monads;

public delegate TResult Reader<in TEnvironment, out TResult>(TEnvironment environment);

public static class Reader<TEnvironment>
{
    [Pure]
    public static Reader<TEnvironment, TResult> Return<TResult>(TResult value)
        => _
            => value;

    [Pure]
    public static Reader<TEnvironment, TResult> FromFunc<TResult>(Func<TEnvironment, TResult> function)
        => function.Invoke;

    [Pure]
    public static Reader<TEnvironment, Unit> FromAction(Action<TEnvironment> action)
        => ActionToUnit(action).Invoke;
}
