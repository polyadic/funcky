using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

public sealed class DoNotEnumerateTwice<T> : IEnumerable<T>
    where T : notnull
{
    private readonly Queue<T> _source;
    private bool _first;

    public DoNotEnumerateTwice(IEnumerable<T> source)
    {
        _source = new Queue<T>(source);
        _first = true;
    }

    public int EnumerationIndex { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        if (!_first)
        {
            throw new XunitException($"Multiple enumeration in {nameof(DoNotEnumerateTwice<T>)}");
        }

        _first = false;

        while (_source.Count > 0)
        {
            EnumerationIndex++;
            yield return _source.Dequeue();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
