using System;

namespace Funcky.Monads
{
    public static class Func
    {
        public static Func<TResult> Return<TResult>(Func<TResult> function)
            => function;

        public static Func<Unit> Return(Action action)
            => ()
                =>
                {
                    action();
                    return Unit.Value;
                };
    }
}
