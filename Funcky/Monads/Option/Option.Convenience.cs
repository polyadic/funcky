using System;
using System.Collections.Generic;
using System.Linq;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        public Option<TItem> Where(Func<TItem, bool> predicate)
            => AndThen(item => predicate(item) ? Option.Some(item) : None());

        public Option<TItem> OrElse(Option<TItem> elseOption)
            => _hasItem
                ? this
                : elseOption;

        public TItem OrElse(TItem elseOption)
            => _hasItem
                ? _item
                : elseOption;

        public Option<TItem> OrElse(Func<Option<TItem>> elseOption)
            => _hasItem
                ? this
                : elseOption.Invoke();

        public TItem OrElse(Func<TItem> elseOption)
            => _hasItem
                ? _item
                : elseOption.Invoke();

        public Option<TResult> AndThen<TResult>(Func<TItem, TResult> andThenFunction)
            where TResult : notnull
            => _hasItem
                ? Option.Some(andThenFunction(_item))
                : Option<TResult>.None();

        public Option<TResult> AndThen<TResult>(Func<TItem, Option<TResult>> andThenFunction)
            where TResult : notnull
            => _hasItem
                ? andThenFunction(_item)
                : Option<TResult>.None();

        public void AndThen(Action<TItem> andThenFunction)
        {
            if (_hasItem)
            {
                andThenFunction(_item);
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that yields exactly one value when the option
        /// has an item and nothing when the option is empty.
        /// </summary>
        public IEnumerable<TItem> ToEnumerable()
            => Match(
                none: Enumerable.Empty<TItem>(),
                some: value => Enumerable.Repeat(value, 1));
    }
}