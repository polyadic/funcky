using System;
using System.Threading;
using System.Threading.Tasks;

namespace Funcky.Linq.Async
{
    internal static class PredicateExtensions
    {
        public static Func<TSource, CancellationToken, ValueTask<bool>> ToAsyncPredicateWithCancellationToken<TSource>(Func<TSource, ValueTask<bool>> predicate)
            => (item, _) => predicate(item);

        public static Func<TSource, CancellationToken, ValueTask<bool>> ToAsyncPredicateWithCancellationToken<TSource>(Func<TSource, bool> predicate)
            => (item, _) => new ValueTask<bool>(predicate(item));
    }
}
