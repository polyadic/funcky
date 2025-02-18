using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct ConfiguredOptionValueTaskAwaitable<TItem>
    where TItem : notnull
{
    private readonly Option<ConfiguredValueTaskAwaitable<TItem>> _awaitable;

    internal ConfiguredOptionValueTaskAwaitable(Option<ConfiguredValueTaskAwaitable<TItem>> awaitable) => _awaitable = awaitable;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConfiguredOptionValueTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ConfiguredOptionValueTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredValueTaskAwaitable<TItem>.ConfiguredValueTaskAwaiter> _awaiter;

        internal ConfiguredOptionValueTaskAwaiter(Option<ConfiguredValueTaskAwaitable<TItem>.ConfiguredValueTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public Option<TItem> GetResult() => _awaiter.Select(task => task.GetResult());
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct ConfiguredOptionValueTaskAwaitable
{
    private readonly Option<ConfiguredValueTaskAwaitable> _awaitable;

    internal ConfiguredOptionValueTaskAwaitable(Option<ConfiguredValueTaskAwaitable> awaitable) =>
        _awaitable = awaitable;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConfiguredOptionValueTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ConfiguredOptionValueTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter> _awaiter;

        internal ConfiguredOptionValueTaskAwaiter(
            Option<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public void GetResult() => _awaiter.AndThen(awaiter => awaiter.GetResult());
    }
}
