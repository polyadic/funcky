using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

public class EnumerateOnce
{
    public static EnumerateOnce<T> Create<T>(IEnumerable<T> sequence)
        where T : notnull
        => new(new Queue<T>(sequence));
}

public class EnumerateOnce<T> : IEnumerable<T>
    where T : notnull
{
    private readonly Queue<T> _once;
    private bool _first = true;

    internal EnumerateOnce(IEnumerable<T> sequence)
        => _once = new Queue<T>(sequence);

    public IEnumerator<T> GetEnumerator()
    {
        ValidateFirst();

        while (_once.DequeueOrNone().TryGetValue(out var value))
        {
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void ValidateFirst()
    {
        if (_first)
        {
            _first = false;
        }
        else
        {
            throw new XunitException("Sequence was unexpectedly enumerated a second time.");
        }
    }
}
