using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

public class AsyncEnumerateOnce<T> : IAsyncEnumerable<T>
    where T : notnull
{
    private readonly Queue<T> _once;
    private bool _first = true;

    private AsyncEnumerateOnce(Queue<T> queue)
        => _once = queue;

    public static async Task<AsyncEnumerateOnce<T>> Create(IAsyncEnumerable<T> sequence)
        => new(new Queue<T>(await sequence.ToListAsync()));

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
