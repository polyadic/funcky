using System;
using Funcky.Extensions;

namespace Funcky.Monads
{
    public static class Lazy
    {
        public static Lazy<TResult> Return<TResult>(Func<TResult> function)
            => new(function);

        public static Lazy<Unit> Return(Action action)
            => new(action.ToUnitFunc());
    }
}
