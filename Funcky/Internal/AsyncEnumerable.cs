namespace Funcky.Internal
{
    internal static class AsyncEnumerable
    {
        public static IAsyncEnumerable<T> Create<T>(Func<CancellationToken, IAsyncEnumerator<T>> createEnumerator)
            => new AnonymousAsyncEnumerable<T>(createEnumerator);

        private sealed class AnonymousAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly Func<CancellationToken, IAsyncEnumerator<T>> _createEnumerator;

            public AnonymousAsyncEnumerable(Func<CancellationToken, IAsyncEnumerator<T>> createEnumerator)
                => _createEnumerator = createEnumerator;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => _createEnumerator(cancellationToken);
        }
    }
}
