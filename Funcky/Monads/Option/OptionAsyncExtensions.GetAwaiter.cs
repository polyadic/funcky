#if ASYNC_SUPPORTED
using System.ComponentModel;

namespace Funcky.Monads;

public static partial class OptionAsyncExtensions
{
    /// <param name="option">The option to await.</param>
    extension(Option<Task> option)
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public OptionTaskAwaiter GetAwaiter()
            => new(option.GetOrElse(Task.CompletedTask).GetAwaiter());

        /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
        /// <param name="continueOnCapturedContext">
        /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns>An object used to await this task.</returns>
        public ConfiguredOptionTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
            => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));
    }

    /// <param name="option">The option to await.</param>
    extension<TItem>(Option<Task<TItem>> option)
        where TItem : notnull
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public OptionTaskAwaiter<TItem> GetAwaiter()
            => new(option.Select(task => task.GetAwaiter()));

        /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
        /// <param name="continueOnCapturedContext">
        /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns>An object used to await this task.</returns>
        public ConfiguredOptionTaskAwaitable<TItem> ConfigureAwait(bool continueOnCapturedContext)
            => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));
    }

    /// <param name="option">The option to await.</param>
    extension(Option<ValueTask> option)
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "False positive.")]
        public OptionValueTaskAwaiter GetAwaiter()
            => new(option.GetOrElse(default(ValueTask)).GetAwaiter());

        /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
        /// <param name="continueOnCapturedContext">
        /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns>An object used to await this task.</returns>
        public ConfiguredOptionValueTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
            => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));
    }

    /// <param name="option">The option to await.</param>
    extension<TItem>(Option<ValueTask<TItem>> option)
        where TItem : notnull
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public OptionValueTaskAwaiter<TItem> GetAwaiter()
            => new(option.Select(task => task.GetAwaiter()));

        /// <summary>Configures an awaiter used to await this <see cref="Option{TItem}"/>.</summary>
        /// <param name="continueOnCapturedContext">
        /// <see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns>An object used to await this ValueTask.</returns>
        public ConfiguredOptionValueTaskAwaitable<TItem> ConfigureAwait(bool continueOnCapturedContext)
            => new(option.Select(task => task.ConfigureAwait(continueOnCapturedContext)));
    }
}
#endif
