using System.Collections;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    internal sealed class FailOnEnumerationList : IList<int>
    {
        private readonly int _length;

        public FailOnEnumerationList(int length)
            => _length = length;

        public int Count
            => _length;

        public bool IsReadOnly
            => true;

        public int this[int index]
        {
            get => index > 0 && index < _length ? index : throw new IndexOutOfRangeException();
            set => throw new InvalidOperationException();
        }

        public void Add(int item)
            => throw new InvalidOperationException();

        public void Clear()
            => throw new InvalidOperationException();

        public bool Contains(int item)
            => throw new InvalidOperationException();

        public void CopyTo(int[] array, int arrayIndex)
            => throw new InvalidOperationException();

        public IEnumerator<int> GetEnumerator()
            => throw new InvalidOperationException();

        public int IndexOf(int item)
            => throw new InvalidOperationException();

        public void Insert(int index, int item)
            => throw new InvalidOperationException();

        public bool Remove(int item)
            => throw new InvalidOperationException();

        public void RemoveAt(int index)
            => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator()
            => throw new InvalidOperationException();
    }
}
