#if INTEGRATED_ASYNC
using System.ComponentModel;

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
#endif
