using System.Collections.Generic;
using System.Collections.Immutable;

namespace Funcky.Extensions
{
    internal sealed class SlidingWindowQueue<TSource>
    {
        private readonly int _width;
        private int _currentWidth;
        private ImmutableQueue<TSource> _window = ImmutableQueue<TSource>.Empty;

        public SlidingWindowQueue(int width)
        {
            _width = width;
        }

        public int CurrentWidth => _currentWidth;

        public IEnumerable<TSource> Window => _window;

        public void Enqueue(TSource element)
        {
            EnqueueWithWidth(element);
            KeepWindowWidth();
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
            if (_currentWidth > _width)
            {
                DequeueWithWidth();
            }
        }
    }
}
