using System;

namespace Funcky.Extensions
{
    internal static class ActionExtensions
    {
        public static Func<Unit> ToUnitFunc(this Action action)
            => ()
                =>
                {
                    action();
                    return Unit.Value;
                };

        public static Func<T, Unit> ToUnitFunc<T>(this Action<T> action)
            => (v)
                =>
                {
                    action(v);
                    return Unit.Value;
                };
    }
}
