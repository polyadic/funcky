using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.GenericConstraints;

namespace Funcky.Monads
{
    public readonly struct Option<TItem> : IToString
        where TItem : notnull
    {
        private readonly bool _hasItem;
        private readonly TItem _item;

        internal Option(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _item = item;
            _hasItem = true;
        }

        public static bool operator ==(Option<TItem> lhs, Option<TItem> rhs) => lhs.Equals(rhs);

        public static bool operator !=(Option<TItem> lhs, Option<TItem> rhs) => !lhs.Equals(rhs);

        public static Option<TItem> None() => default;

        public Option<TResult> Select<TResult>(Func<TItem, TResult> selector)
            where TResult : notnull
            => _hasItem
                ? Option.Some(selector(_item))
                : Option<TResult>.None();

        public Option<TResult> SelectMany<TMaybe, TResult>(Func<TItem, Option<TMaybe>> maybeSelector, Func<TItem, TMaybe, TResult> resultSelector)
            where TResult : notnull
            where TMaybe : notnull
        {
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
            => _hasItem
                ? some(_item)
                : none;

        public TResult Match<TResult>(Func<TResult> none, Func<TItem, TResult> some)
            => _hasItem
                  ? some(_item)
                  : none();

        public void Match(Action none, Action<TItem> some)
        {
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

        public override bool Equals(object obj)
            => obj is Option<TItem> other
            && Equals(_item, other._item);

        public override int GetHashCode() =>
            Select(item => item.GetHashCode()).OrElse(0);

        public override string ToString()
            => Match(
                none: "None",
                some: value => $"Some({value})");
    }

    public static class Option
    {
        public static Option<TItem> Some<TItem>(TItem item)
            where TItem : notnull
            => new Option<TItem>(item);

        public static Option<TItem> Some<TItem>(Option<TItem> item)
            where TItem : notnull
            => item;

        /// <summary>
        /// Creates an <see cref="Option{T}"/> from a nullable value.
        /// </summary>
        public static Option<T> FromNullable<T>(T? item)
            where T : class
            => item is { } value ? Some(value) : Option<T>.None();

        /// <summary>
        /// Creates an <see cref="Option{T}"/> from a nullable value.
        /// </summary>
        public static Option<T> FromNullable<T>(T? item)
            where T : struct
            => item.HasValue ? Some(item.Value) : Option<T>.None();
    }
}
