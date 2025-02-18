using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct OptionValueTaskAwaiter<TItem> : INotifyCompletion
    where TItem : notnull
{
    private readonly Option<ValueTaskAwaiter<TItem>> _awaiter;

    internal OptionValueTaskAwaiter(Option<ValueTaskAwaiter<TItem>> awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter
        .Select(awaiter => awaiter.IsCompleted)
        .GetOrElse(true);

    public void OnCompleted(Action continuation)
        => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

    public Option<TItem> GetResult()
        => _awaiter.Select(awaiter => awaiter.GetResult());
}

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct OptionValueTaskAwaiter : INotifyCompletion
{
    private readonly ValueTaskAwaiter _awaiter;

    internal OptionValueTaskAwaiter(ValueTaskAwaiter awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter.IsCompleted;

    public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

    public void GetResult() => _awaiter.GetResult();
}
