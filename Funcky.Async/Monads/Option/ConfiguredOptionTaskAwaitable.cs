using System.Runtime.CompilerServices;

namespace Funcky.Monads;

public readonly struct ConfiguredOptionTaskAwaitable<TItem>
    where TItem : notnull
{
    private readonly Option<ConfiguredTaskAwaitable<TItem>> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable<TItem>> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.Switch(none: continuation, some: awaiter => awaiter.OnCompleted(continuation));

        public void UnsafeOnCompleted(Action continuation) => _awaiter.Switch(none: continuation, some: awaiter => awaiter.UnsafeOnCompleted(continuation));

        public Option<TItem> GetResult() => _awaiter.Select(awaiter => awaiter.GetResult());
    }
}

public readonly struct ConfiguredOptionTaskAwaitable
{
    private readonly Option<ConfiguredTaskAwaitable> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> awaiter) =>
            _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.Switch(none: continuation, some: awaiter => awaiter.OnCompleted(continuation));

        public void UnsafeOnCompleted(Action continuation) => _awaiter.Switch(none: continuation, some: awaiter => awaiter.UnsafeOnCompleted(continuation));

        public void GetResult() => _awaiter.AndThen(awaiter => awaiter.GetResult());
    }
}
