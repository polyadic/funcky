using System;

namespace Funcky.Monads
{
    public static class Lazy
    {
        public static Lazy<TResult> Return<TResult>(Func<TResult> function)
            => new Lazy<TResult>(function);

        public static Lazy<Unit> Return(Action action)
            => new Lazy<Unit>
                =>
            {
                action();
                return Unit.Value;
            };
}
}
