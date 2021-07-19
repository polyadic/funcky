namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        private static Func<TSource, CancellationToken, ValueTask<bool>> ToAsyncPredicateWithCancellationToken<TSource>(Func<TSource, ValueTask<bool>> predicate)
            => (item, _) => predicate(item);

        private static Func<TSource, CancellationToken, ValueTask<bool>> ToAsyncPredicateWithCancellationToken<TSource>(Func<TSource, bool> predicate)
            => (item, _) => new ValueTask<bool>(predicate(item));
    }
}
