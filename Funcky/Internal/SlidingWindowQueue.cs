using System.Collections.Immutable;

namespace Funcky.Internal;

internal sealed class SlidingWindowQueue<TSource>(int width)
{
    private int _currentWidth;
    private ImmutableQueue<TSource> _window = ImmutableQueue<TSource>.Empty;

    public IReadOnlyList<TSource> Window
        => _window.ToImmutableList();

    public bool IsFull
        => width == _currentWidth;

    public SlidingWindowQueue<TSource> Enqueue(TSource element)
    {
        EnqueueWithWidth(element);
        KeepWindowWidth();

        return this;
    }

    private void EnqueueWithWidth(TSource element)
    {
        _window = _window.Enqueue(element);
        _currentWidth += 1;
    }

    private void DequeueWithWidth()
    {
        _window = _window.Dequeue();
        _currentWidth -= 1;
    }

    private void KeepWindowWidth()
    {
        if (_currentWidth > width)
        {
            DequeueWithWidth();
        }
    }
}
