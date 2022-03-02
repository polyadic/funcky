using System.Collections;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

public class EnumerateOnce<T> : IEnumerable<T>
    where T : notnull
{
    private readonly Queue<T> _once;
    private bool _first = true;

    public EnumerateOnce(IEnumerable<T> sequence)
        => _once = new Queue<T>(sequence);

    public IEnumerator<T> GetEnumerator()
    {
        ValidateFirst();

        while (true)
        {
            var maybeValue = _once.DequeueOrNone();

            if (maybeValue.Match(none: false, some: True))
            {
                yield return maybeValue.GetOrElse(() => throw new Exception("cannot happen!"));
            }
            else
            {
                break;
            }
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
