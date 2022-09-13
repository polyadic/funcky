using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Monads;

public static partial class OptionAsyncExtensions
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static OptionTaskAwaiter GetAwaiter(this Option<Task> option)
        => new(option.GetOrElse(Task.CompletedTask).GetAwaiter());

    /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
    /// <param name="option">The option to await.</param>
    /// <param name="continueOnCapturedContext">
    /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
    /// <returns>An object used to await this task.</returns>
    public static ConfiguredOptionTaskAwaitable ConfigureAwait(this Option<Task> option, bool continueOnCapturedContext)
        => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static OptionTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<Task<TItem>> option)
        where TItem : notnull
        => new(option.Select(task => task.GetAwaiter()));

    /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
    /// <param name="option">The option to await.</param>
    /// <param name="continueOnCapturedContext">
    /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
    /// <returns>An object used to await this task.</returns>
    public static ConfiguredOptionTaskAwaitable<TItem> ConfigureAwait<TItem>(this Option<Task<TItem>> option, bool continueOnCapturedContext)
        where TItem : notnull
        => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "False positive.")]
    public static OptionValueTaskAwaiter GetAwaiter(this Option<ValueTask> option)
        => new(option.GetOrElse(default(ValueTask)).GetAwaiter());

    /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
    /// <param name="option">The option to await.</param>
    /// <param name="continueOnCapturedContext">
    /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
    /// <returns>An object used to await this task.</returns>
    public static ConfiguredOptionValueTaskAwaitable ConfigureAwait(this Option<ValueTask> option, bool continueOnCapturedContext)
        => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static OptionValueTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<ValueTask<TItem>> option)
        where TItem : notnull
        => new(option.Select(task => task.GetAwaiter()));

    /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
    /// <param name="option">The option to await.</param>
    /// <param name="continueOnCapturedContext">
    /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
    /// <returns>An object used to await this ValueTask.</returns>
    public static ConfiguredOptionValueTaskAwaitable<TItem> ConfigureAwait<TItem>(this Option<ValueTask<TItem>> option, bool continueOnCapturedContext)
        where TItem : notnull
        => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));
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

public readonly struct ConfiguredOptionTaskAwaitable
{
    private readonly Option<ConfiguredTaskAwaitable> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public void GetResult() => _awaiter.AndThen(awaiter => awaiter.GetResult());
    }
}

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

public readonly struct ConfiguredOptionTaskAwaitable<TItem>
    where TItem : notnull
{
    private readonly Option<ConfiguredTaskAwaitable<TItem>> _awaitable;

    internal ConfiguredOptionTaskAwaitable(Option<ConfiguredTaskAwaitable<TItem>> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> _awaiter;

        internal ConfiguredOptionTaskAwaiter(Option<ConfiguredTaskAwaitable<TItem>.ConfiguredTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public Option<TItem> GetResult() => _awaiter.Select(awaiter => awaiter.GetResult());
    }
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly struct OptionValueTaskAwaiter : INotifyCompletion
{
    private readonly ValueTaskAwaiter _awaiter;

    internal OptionValueTaskAwaiter(ValueTaskAwaiter awaiter) => _awaiter = awaiter;

    public bool IsCompleted => _awaiter.IsCompleted;

    public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

    public void GetResult() => _awaiter.GetResult();
}

public readonly struct ConfiguredOptionValueTaskAwaitable
{
    private readonly Option<ConfiguredValueTaskAwaitable> _awaitable;

    internal ConfiguredOptionValueTaskAwaitable(Option<ConfiguredValueTaskAwaitable> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionValueTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionValueTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter> _awaiter;

        internal ConfiguredOptionValueTaskAwaiter(Option<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public void GetResult() => _awaiter.AndThen(awaiter => awaiter.GetResult());
    }
}

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

public readonly struct ConfiguredOptionValueTaskAwaitable<TItem>
    where TItem : notnull
{
    private readonly Option<ConfiguredValueTaskAwaitable<TItem>> _awaitable;

    internal ConfiguredOptionValueTaskAwaitable(Option<ConfiguredValueTaskAwaitable<TItem>> awaitable) => _awaitable = awaitable;

    public ConfiguredOptionValueTaskAwaiter GetAwaiter() => new(_awaitable.Select(awaitable => awaitable.GetAwaiter()));

    public readonly struct ConfiguredOptionValueTaskAwaiter : INotifyCompletion
    {
        private readonly Option<ConfiguredValueTaskAwaitable<TItem>.ConfiguredValueTaskAwaiter> _awaiter;

        internal ConfiguredOptionValueTaskAwaiter(Option<ConfiguredValueTaskAwaitable<TItem>.ConfiguredValueTaskAwaiter> awaiter) => _awaiter = awaiter;

        public bool IsCompleted => _awaiter.Select(awaiter => awaiter.IsCompleted).GetOrElse(true);

        public void OnCompleted(Action continuation) => _awaiter.AndThen(awaiter => awaiter.OnCompleted(continuation));

        public Option<TItem> GetResult() => _awaiter.Select(task => task.GetResult());
    }
}
