using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

public static partial class OptionAsyncExtensions
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TaskAwaiter GetAwaiter(this Option<Task> option)
        => option.GetOrElse(Task.CompletedTask).GetAwaiter();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static OptionTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<Task<TItem>> option)
        where TItem : notnull
        => new(option.Select(t => t.GetAwaiter()));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "False positive.")]
    public static ValueTaskAwaiter GetAwaiter(this Option<ValueTask> option)
        => option.GetOrElse(default(ValueTask)).GetAwaiter();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static OptionValueTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<ValueTask<TItem>> option)
        where TItem : notnull
        => new(option.Select(t => t.GetAwaiter()));
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly struct OptionTaskAwaiter<TItem> : INotifyCompletion
    where TItem : notnull
{
    private readonly Option<TaskAwaiter<TItem>> _awaiter;

    internal OptionTaskAwaiter(Option<TaskAwaiter<TItem>> awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter
        .Select(a => a.IsCompleted)
        .GetOrElse(true);

    public void OnCompleted(Action continuation)
        => _awaiter.AndThen(a => a.OnCompleted(continuation));

    public Option<TItem> GetResult()
        => _awaiter.Select(a => a.GetResult());
}

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct OptionValueTaskAwaiter<TItem> : INotifyCompletion
    where TItem : notnull
{
    private readonly Option<ValueTaskAwaiter<TItem>> _awaiter;

    internal OptionValueTaskAwaiter(Option<ValueTaskAwaiter<TItem>> awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter
        .Select(a => a.IsCompleted)
        .GetOrElse(true);

    public void OnCompleted(Action continuation)
        => _awaiter.AndThen(a => a.OnCompleted(continuation));

    public Option<TItem> GetResult()
        => _awaiter.Select(a => a.GetResult());
}
