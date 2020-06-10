using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.GenericConstraints;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem> : IToString
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

    public static partial class Option
    {
        public static Option<TItem> Some<TItem>(TItem item)
            where TItem : notnull
            => new Option<TItem>(item);

        public static Option<TItem> Some<TItem>(Option<TItem> item)
            where TItem : notnull
            => item;
    }
}
