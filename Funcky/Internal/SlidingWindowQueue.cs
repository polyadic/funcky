namespace Funcky.Internal;

/// <summary>A ring buffer of the last <paramref name="width"/> elements; <see cref="Window"/> snapshots them in insertion order.</summary>
internal sealed class SlidingWindowQueue<TSource>(int width)
{
    private readonly TSource[] _buffer = new TSource[width];
    private int _start;
    private int _count;

    public IReadOnlyList<TSource> Window
    {
        get
        {
            var window = new TSource[_count];

            for (var index = 0; index < _count; index++)
            {
                window[index] = _buffer[(_start + index) % width];
            }

            return Array.AsReadOnly(window);
        }
    }

    public bool IsFull
        => _count == width;

    public SlidingWindowQueue<TSource> Enqueue(TSource element)
    {
        if (IsFull)
        {
            _buffer[_start] = element;
            _start = (_start + 1) % width;
        }
        else
        {
            _buffer[(_start + _count) % width] = element;
            _count += 1;
        }

        return this;
    }
}
