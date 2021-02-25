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
            => function;

        public static Func<Unit> Func(Action action)
            => ()
                =>
                {
                    action();
                    return Unit.Value;
                };

        public static State<TState, TState> GetState<TState>() =>
            oldState => (oldState, oldState);

        public static State<TState, Unit> SetState<TState>(TState newState) =>
            _ => (default, newState);
    }
}
