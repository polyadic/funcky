using System.Collections;

namespace Funcky.Internal
{
    internal class ListWithIndex<TSource> : IList<ValueWithIndex<TSource>>
    {
        private readonly IList<TSource> _source;

        public ListWithIndex(IList<TSource> source)
            => _source = source;

        public int Count
            => _source.Count;

        public bool IsReadOnly
            => true;

        public ValueWithIndex<TSource> this[int index]
        {
            get => new(_source[index], index);
            set => throw new InvalidOperationException();
        }

        public void Add(ValueWithIndex<TSource> item)
            => throw new InvalidOperationException();

        public void Clear()
            => throw new InvalidOperationException();

        public bool Contains(ValueWithIndex<TSource> item)
            => throw new InvalidOperationException();

        public void CopyTo(ValueWithIndex<TSource>[] array, int arrayIndex)
            => throw new InvalidOperationException();

        public IEnumerator<ValueWithIndex<TSource>> GetEnumerator()
            => _source
                .Select(ValueWithIndex<TSource>.Create)
                .GetEnumerator();

        public int IndexOf(ValueWithIndex<TSource> item)
            => throw new InvalidOperationException();

        public void Insert(int index, ValueWithIndex<TSource> item)
            => throw new InvalidOperationException();

        public bool Remove(ValueWithIndex<TSource> item)
            => throw new InvalidOperationException();

        public void RemoveAt(int index)
            => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
