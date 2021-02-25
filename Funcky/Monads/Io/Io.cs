using System;

namespace Funcky.Monads
{
    public delegate T Io<out T>();

    public static class Io
    {
        public static Io<TResult> Return<TResult>(Func<TResult> function)
            => ()
                => function();

        public static Io<Unit> Return(Action action)
            => ()
                =>
            {
                action();
                return Unit.Value;
            };
    }
}
