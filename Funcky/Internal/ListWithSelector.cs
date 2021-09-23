using System.Collections;

namespace Funcky.Internal
{
    internal class ListWithSelector<TSource, TResult> : IList<TResult>
    {
        private readonly IList<TSource> _source;
        private readonly Func<TSource, int, TResult> _selector;

        public ListWithSelector(IList<TSource> source, Func<IList<TSource>, Func<TSource, int, TResult>> selector)
            => (_source, _selector) = (source, selector(source));

        public int Count
            => _source.Count;

        public bool IsReadOnly
            => true;

        public TResult this[int index]
        {
            get => _selector(_source[index], index);
            set => throw new InvalidOperationException();
        }

        public void Add(TResult item)
            => throw new InvalidOperationException();

        public void Clear()
            => throw new InvalidOperationException();

        public bool Contains(TResult item)
            => throw new InvalidOperationException();

        public void CopyTo(TResult[] array, int arrayIndex)
            => throw new InvalidOperationException();

        public IEnumerator<TResult> GetEnumerator()
            => _source
                .Select(_selector)
                .GetEnumerator();

        public int IndexOf(TResult item)
            => throw new InvalidOperationException();

        public void Insert(int index, TResult item)
            => throw new InvalidOperationException();

        public bool Remove(TResult item)
            => throw new InvalidOperationException();

        public void RemoveAt(int index)
            => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
