using System;

namespace Funcky
{
    public sealed class Maybe<TItem>
    {
        private readonly bool _hasItem;
        private readonly TItem _item;

        public Maybe() => _hasItem = false;

        public Maybe(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _item = item;
            _hasItem = true;

        }

        public Maybe<TResult> Select<TResult>(Func<TItem, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return _hasItem
                ? new Maybe<TResult>(selector(_item))
                : new Maybe<TResult>();
        }

        public Maybe<TResult> SelectMany<TResult>(Func<TItem, Maybe<TResult>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return _hasItem
                ? selector(_item)
                : new Maybe<TResult>();
        }

        public TResult Match<TResult>(TResult nothing, Func<TItem, TResult> just)
        {
            if (nothing == null)
            {
                throw new ArgumentNullException(nameof(nothing));
            }
            if (just == null)
            {
                throw new ArgumentNullException(nameof(just));
            }

            return _hasItem
                ? just(_item)
                : nothing;
        }
    }
}
