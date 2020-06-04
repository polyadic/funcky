using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Constraints;
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
            where TResult : notnull
            where TMaybe : notnull
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

        public Option<TItem> OrElse(Func<Option<TItem>> elseOption)
        {
            return _hasItem
                ? this
                : elseOption.Invoke();
        }

        public TItem OrElse(Func<TItem> elseOption)
        {
            return _hasItem
                ? _item
                : elseOption.Invoke();
        }

        public Option<TResult> AndThen<TResult>(Func<TItem, TResult> andThenFunction)
            where TResult : notnull
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

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that yields exactly one value when the option
        /// has an item and nothing when the option is empty.
        /// </summary>
        public IEnumerable<TItem> ToEnumerable()
            => Match(
                   none: Enumerable.Empty<TItem>(),
                   some: value => Enumerable.Repeat(value, 1));

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
                some: value => $"Some({value})");
        }
    }

    public static class Option
    {
        public static Option<TItem> Some<TItem>(TItem item)
            where TItem : notnull
        {
            return new Option<TItem>(item);
        }

        public static Option<TItem> Some<TItem>(Option<TItem> item)
            where TItem : notnull
        {
            return item;
        }

        /// <summary>
        /// Creates an <see cref="Option{T}"/> from a nullable value.
        /// </summary>
        public static Option<T> From<T>(T? item, RequireClass<T>? ω = null)
            where T : class
            => item is { } value ? Some(value) : Option<T>.None();

        /// <inheritdoc cref="From{T}(T, RequireClass{T})"/>
        public static Option<T> From<T>(T item, RequireStruct<T>? ω = null)
            where T : struct
            => Some(item);

        /// <inheritdoc cref="From{T}(T, RequireClass{T})"/>
        public static Option<T> From<T>(T? item)
            where T : struct
            => item.HasValue ? Some(item.Value) : Option<T>.None();
    }
}
