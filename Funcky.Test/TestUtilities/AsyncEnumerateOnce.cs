#if INTEGRATED_ASYNC
using Xunit.Sdk;

namespace Funcky.Async.Test.TestUtilities;

public class AsyncEnumerateOnce
{
    public static AsyncEnumerateOnce<T> Create<T>(IEnumerable<T> sequence)
        where T : notnull
        => new(new Queue<T>(sequence));
}

public class AsyncEnumerateOnce<T> : IAsyncEnumerable<T>
    where T : notnull
{
    private readonly Queue<T> _once;
    private bool _first = true;

    internal AsyncEnumerateOnce(Queue<T> queue)
        => _once = queue;

#pragma warning disable CS1998
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
#pragma warning restore CS1998
    {
        ValidateFirst();

        while (true)
        {
            if (_once.TryDequeue(out var value))
            {
                yield return value;
            }
            else
            {
                break;
            }
        }
    }

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
#endif
