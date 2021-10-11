using System.ComponentModel;
using System.Runtime.CompilerServices;
using Funcky.Monads;

namespace Funcky.Async.Monads
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AwaitableOptionExtensions
    {
        /// <summary>
        /// This method is intended for compiler use only
        /// and are therefore exempt from typical semver guarantees.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TaskAwaiter GetAwaiter(this Option<Task> option)
            => option.GetOrElse(Task.CompletedTask).GetAwaiter();

        /// <summary>
        /// This method is intended for compiler use only
        /// and are therefore exempt from typical semver guarantees.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static OptionTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<Task<TItem>> option)
            where TItem : notnull
            => new(option.Select(t => t.GetAwaiter()));

        /// <summary>
        /// This method is intended for compiler use only
        /// and are therefore exempt from typical semver guarantees.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "Intended")]
        public static ValueTaskAwaiter GetAwaiter(this Option<ValueTask> option)
            => option.GetOrElse(default(ValueTask)).GetAwaiter();

        /// <summary>
        /// This method is intended for compiler use only
        /// and are therefore exempt from typical semver guarantees.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static OptionValueTaskAwaiter<TItem> GetAwaiter<TItem>(this Option<ValueTask<TItem>> option)
            where TItem : notnull
            => new(option.Select(t => t.GetAwaiter()));
    }
}
