using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly struct OptionTaskAwaiter<TItem> : INotifyCompletion
    where TItem : notnull
{
    private readonly Option<TaskAwaiter<TItem>> _awaiter;

    internal OptionTaskAwaiter(Option<TaskAwaiter<TItem>> awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter
        .Select(awaiter => awaiter.IsCompleted)
        .GetOrElse(true);

    public void OnCompleted(Action continuation)
        => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

    public Option<TItem> GetResult()
        => _awaiter.Select(awaiter => awaiter.GetResult());
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly struct OptionTaskAwaiter : INotifyCompletion
{
    private readonly TaskAwaiter _awaiter;

    internal OptionTaskAwaiter(TaskAwaiter awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter.IsCompleted;

    public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

    public void GetResult() => _awaiter.GetResult();
}
