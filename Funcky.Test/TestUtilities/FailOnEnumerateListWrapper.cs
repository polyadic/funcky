namespace Funcky.Test.TestUtilities;

internal sealed class FailOnEnumerateListWrapper<T>(IList<T> list) : FailOnEnumerateCollectionWrapper<T>(list), IList<T>
{
    public T this[int index]
    {
        get => list[index];
        set => throw new NotSupportedException();
    }

    public int IndexOf(T item) => list.IndexOf(item);

    public void Insert(int index, T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();
}
