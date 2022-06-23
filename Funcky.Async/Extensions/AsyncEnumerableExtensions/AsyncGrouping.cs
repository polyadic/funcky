using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    internal class AsyncGrouping<TKey, TElement> : IAsyncGrouping<TKey, TElement>
    {
        private readonly IImmutableList<TElement> _elements;

        internal AsyncGrouping(TKey key, IImmutableList<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public TKey Key { get; }

#pragma warning disable CS1998
        public async IAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default)
#pragma warning restore CS1998
        {
            foreach (var element in _elements)
            {
                yield return element;
            }
        }
    }
}
