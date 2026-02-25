namespace Funcky.Test.TestUtilities;

internal sealed record FailOnEnumerateList<T>(int Count) : FailOnEnumerateCollection<T>(Count), IList<T>
{
    public T this[int index]
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public int IndexOf(T item) => throw new NotSupportedException();

    public void Insert(int index, T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();
}
