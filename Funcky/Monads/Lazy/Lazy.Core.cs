using System;
using static Funcky.Functional;

namespace Funcky.Monads
{
    public static class Lazy
    {
        public static Lazy<TResult> Return<TResult>(Func<TResult> function)
            => new(function);

        public static Lazy<Unit> Return(Action action)
            => new(ActionToUnit(action));
    }
}
