#if INTEGRATED_ASYNC
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct OptionValueTaskAwaiter<TItem> : ICriticalNotifyCompletion
    where TItem : notnull
{
    private readonly Option<ValueTaskAwaiter<TItem>> _awaiter;

    internal OptionValueTaskAwaiter(Option<ValueTaskAwaiter<TItem>> awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter
        .Select(awaiter => awaiter.IsCompleted)
        .GetOrElse(true);

    public void OnCompleted(Action continuation)
        => _awaiter.Switch(none: continuation, some: awaiter => awaiter.OnCompleted(continuation));

    public void UnsafeOnCompleted(Action continuation)
        => _awaiter.Switch(none: continuation, some: awaiter => awaiter.UnsafeOnCompleted(continuation));

    public Option<TItem> GetResult()
        => _awaiter.Select(awaiter => awaiter.GetResult());
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly struct OptionValueTaskAwaiter : ICriticalNotifyCompletion
{
    private readonly ValueTaskAwaiter _awaiter;

    internal OptionValueTaskAwaiter(ValueTaskAwaiter awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter.IsCompleted;

    public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

    public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

    public void GetResult() => _awaiter.GetResult();
}
#endif
