using System;

namespace Funcky.Monads
{
    public static class Factory
    {
        public static Io<TResult> Io<TResult>(Func<TResult> function)
            => ()
                => function();

        public static Io<Unit> Io(Action action)
            => ()
                =>
                {
                    action();
                    return Unit.Value;
                };

        public static Func<TResult> Func<TResult>(Func<TResult> function)
            => ()
                => function();

        public static Func<Unit> Func(Action action)
            => ()
                =>
                {
                    action();
                    return Unit.Value;
                };
    }
}
