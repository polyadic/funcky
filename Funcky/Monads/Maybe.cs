using System;

namespace Funcky.Monads
{
    public static class Maybe
    {
        /// <summary>
        /// Factory Method to use type inference to create a new Maybe instance
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Maybe<TItem> Create<TItem>(TItem item)
        {
            return new Maybe<TItem>(item);
        }

        public static Maybe<TItem> Some<TItem>(TItem item)
        {
            return new Maybe<TItem>(item);
        }
    }

    public sealed class Maybe<TItem>
    {
        private readonly bool _hasItem;
        private readonly TItem _item;

        public Maybe() => _hasItem = false;
        public static Maybe<TItem> None() => new Maybe<TItem>();

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
                ? Maybe.Some(selector(_item))
                : Maybe<TResult>.None();
        }


        public Maybe<TResult> SelectMany<TMaybe, TResult>(Func<TItem, Maybe<TMaybe>> maybeSelector, Func<TItem, TMaybe, TResult> resultSelector)
        {
            if (maybeSelector == null)
            {
                throw new ArgumentNullException(nameof(maybeSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            if (_hasItem)
            {
                Maybe<TMaybe> selectedMaybe = maybeSelector(_item);
                if (selectedMaybe._hasItem)
                {
                    return Maybe.Some(resultSelector(_item, selectedMaybe._item));
                }

            }
            return Maybe<TResult>.None();
        }

        public TResult Match<TResult>(TResult none, Func<TItem, TResult> some)
        {
            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            return _hasItem
                ? some(_item)
                : none;
        }

        public override bool Equals(object obj)
        {
            return obj is Maybe<TItem> other && Equals(_item, other._item);
        }

        public override int GetHashCode()
        {
            return _hasItem
                ? _item.GetHashCode()
                : 0;
        }
    }
}
