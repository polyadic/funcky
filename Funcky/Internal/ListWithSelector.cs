using System.Collections;

namespace Funcky.Internal;

internal class ListWithSelector
{
    public static ListWithSelector<TSource, TResult> Create<TSource, TResult>(IList<TSource> source, Func<IList<TSource>, Func<TSource, int, TResult>> selector)
        => new(source, selector);
}

internal class ListWithSelector<TSource, TResult>(IList<TSource> source, Func<IList<TSource>, Func<TSource, int, TResult>> selector) : IList<TResult>
{
    private readonly Func<TSource, int, TResult> _selector = selector(source);

    public int Count
        => source.Count;

    public bool IsReadOnly
        => true;

    public TResult this[int index]
    {
        get => _selector(source[index], index);
        set => throw new NotSupportedException();
    }

    public void Add(TResult item)
        => throw new NotSupportedException();

    public void Clear()
        => throw new NotSupportedException();

    public bool Contains(TResult item)
        => source.Select(_selector).Contains(item);

    public void CopyTo(TResult[] array, int arrayIndex)
    {
        var index = arrayIndex;
        foreach (var element in source.Skip(arrayIndex).Select(_selector))
        {
            array[index++] = element;
        }
    }

    public IEnumerator<TResult> GetEnumerator()
        => source
            .Select(_selector)
            .GetEnumerator();

    public int IndexOf(TResult item)
        => throw new NotSupportedException();

    public void Insert(int index, TResult item)
        => throw new NotSupportedException();

    public bool Remove(TResult item)
        => throw new NotSupportedException();

    public void RemoveAt(int index)
        => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
