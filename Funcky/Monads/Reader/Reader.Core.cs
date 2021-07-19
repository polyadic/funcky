using System.Diagnostics.Contracts;
using static Funcky.Functional;

namespace Funcky.Monads
{
    public delegate TResult Reader<in TEnvironment, out TResult>(TEnvironment environment);

    public static class Reader<TEnvironment>
    {
        [Pure]
        public static Reader<TEnvironment, TSource> Return<TSource>(TSource value)
            => _
                => value;

        [Pure]
        public static Reader<TEnvironment, TResult> FromFunc<TResult>(Func<TEnvironment, TResult> function)
            => function.Invoke;

        [Pure]
        public static Reader<TEnvironment, Unit> FromAction(Action<TEnvironment> action)
            => ActionToUnit(action).Invoke;
    }
}
