using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct ConfiguredOptionTaskAwaitable<TItem>
    where TItem : notnull
{
    private readonly Option<ConfiguredTaskAwaitable<TItem>> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable<TItem>> awaitable) => _awaitable = awaitable;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ConfiguredOptionTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public Option<TItem> GetResult() => _awaiter.Select(awaiter => awaiter.GetResult());
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct ConfiguredOptionTaskAwaitable
{
    private readonly Option<ConfiguredTaskAwaitable> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable> awaitable) => _awaitable = awaitable;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ConfiguredOptionTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> awaiter) =>
            _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public void GetResult() => _awaiter.AndThen(awaiter => awaiter.GetResult());
    }
}
