using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Funcky.Async.Monads
{
    /// <summary>
    /// This type and its members are intended for compiler use only
    /// and are therefore exempt from typical semver guarantees.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct OptionTaskAwaiter<TItem> : INotifyCompletion
        where TItem : notnull
    {
        private readonly Option<TaskAwaiter<TItem>> _awaiter;

        internal OptionTaskAwaiter(Option<TaskAwaiter<TItem>> awaiter)
        {
            _awaiter = awaiter;
        }

        public bool IsCompleted => _awaiter
            .Select(a => a.IsCompleted)
            .GetOrElse(true);

        public void OnCompleted(Action continuation)
            => _awaiter.AndThen(a => a.OnCompleted(continuation));

        public Option<TItem> GetResult()
            => _awaiter.Select(a => a.GetResult());
    }
}
