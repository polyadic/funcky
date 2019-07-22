using System;

namespace Funcky.Monads
{
    public static class Option
    {
        public static Option<TItem> Some<TItem>(TItem item)
        {
            return new Option<TItem>(item);
        }
    }

    public struct Option<TItem> :
        IToString
    {
        private readonly bool _hasItem;
        private readonly TItem _item;

        public static Option<TItem> None() => new Option<TItem>();

        internal Option(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _item = item;
            _hasItem = true;

        }

        public Option<TResult> Select<TResult>(Func<TItem, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return _hasItem
                ? Option.Some(selector(_item))
                : Option<TResult>.None();
        }


        public Option<TResult> SelectMany<TMaybe, TResult>(Func<TItem, Option<TMaybe>> maybeSelector, Func<TItem, TMaybe, TResult> resultSelector)
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
                var selectedMaybe = maybeSelector(_item);
                if (selectedMaybe._hasItem)
                {
                    return Option.Some(resultSelector(_item, selectedMaybe._item));
                }

            }

            return Option<TResult>.None();
        }

        public TResult Match<TResult>(TResult none, Func<TItem, TResult> some)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            return _hasItem
                ? some(_item)
                : none;
        }

        public TResult Match<TResult>(Func<TResult> none, Func<TItem, TResult> some)
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
                : none();
        }

        public void Match(Action none, Action<TItem> some)
        {
            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (_hasItem)
            {
                some(_item);
            }
            else
            {
                none();
            }
        }

        public Option<TItem> OrElse(Option<TItem> elseOption)
        {
            return _hasItem
                ? this
                : elseOption;
        }

        public TItem OrElse(TItem elseOption)
        {
            return _hasItem
                ? _item
                : elseOption;
        }

        public Option<TResult> AndThen<TResult>(Func<TItem, TResult> andThenFunction)
        {
            return _hasItem
                ? Option.Some(andThenFunction(_item))
                : Option<TResult>.None();
        }

        public void AndThen(Action<TItem> andThenFunction)
        {
            if (_hasItem)
            {
                andThenFunction(_item);
            }
        }


        public override bool Equals(object obj)
        {
            return obj is Option<TItem> other
                   && Equals(_item, other._item);
        }

        public override int GetHashCode()
        {
            return _hasItem
                ? _item.GetHashCode()
                : 0;
        }

        public override string ToString()
        {
            return Match(
                none: "None",
                some: value => $"Some({value})"
                );
        }
    }
}
