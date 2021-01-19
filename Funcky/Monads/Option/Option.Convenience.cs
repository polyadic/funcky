using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Extensions;
using static Funcky.Functional;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        public static implicit operator Option<TItem>(TItem item)
            => Option.Some(item);

        [Pure]
        public Option<TItem> Where(Func<TItem, bool> predicate)
            => SelectMany(item => predicate(item) ? item : None());

        [Pure]
        public ValueTask<Option<TItem>> WhereAwait(Func<TItem, ValueTask<bool>> predicate)
            => SelectManyAwait(async item => await predicate(item) ? item : None());

        [Pure]
        public Option<TItem> OrElse(Option<TItem> elseOption)
            => Match(none: elseOption, some: Option.Some);

        [Pure]
        public Option<TItem> OrElse(Func<Option<TItem>> elseOption)
            => Match(none: elseOption, some: Option.Some);

        [Pure]
        public async ValueTask<Option<TItem>> OrElseAwait(Func<ValueTask<Option<TItem>>> elseOption)
            => await Match(none: elseOption, some: item => new ValueTask<Option<TItem>>(item));

        [Pure]
        public TItem GetOrElse(TItem elseOption)
            => Match(none: elseOption, some: Identity);

        [Pure]
        public TItem GetOrElse(Func<TItem> elseOption)
            => Match(none: elseOption, some: Identity);

        [Pure]
        public async ValueTask<TItem> GetOrElseAwait(Func<ValueTask<TItem>> elseOption)
            => await Match<ValueTask<TItem>>(none: elseOption, some: item => new ValueTask<TItem>(item));

        [Pure]
        public Option<TResult> AndThen<TResult>(Func<TItem, TResult> andThenFunction)
            where TResult : notnull
            => Select(andThenFunction);

        [Pure]
        public async ValueTask<Option<TResult>> AndThenAwait<TResult>(Func<TItem, ValueTask<TResult>> andThenFunction)
            where TResult : notnull
            => await SelectAwait(andThenFunction);

        [Pure]
        public Option<TResult> AndThen<TResult>(Func<TItem, Option<TResult>> andThenFunction)
            where TResult : notnull
            => SelectMany(andThenFunction);

        [Pure]
        public async ValueTask<Option<TResult>> AndThenAwait<TResult>(Func<TItem, ValueTask<Option<TResult>>> andThenFunction)
            where TResult : notnull
            => await SelectManyAwait(andThenFunction);

        /// <summary>
        /// Performs a side effect when the option has a value.
        /// </summary>
        public void AndThen(Action<TItem> andThenFunction)
            => Match(none: NoOperation, some: andThenFunction);

        /// <summary>
        /// Performs a side effect when the option has a value and returns the option again.
        /// This is the <see cref="Option{T}"/> equivalent of <see cref="EnumerableExtensions.Inspect{T}(IEnumerable{T}, Action{T})"/>.
        /// </summary>
        public Option<TItem> Inspect(Action<TItem> action)
        {
            AndThen(action);
            return this;
        }

        /// <inheritdoc cref="Inspect(System.Action{TItem})"/>
        public async ValueTask<Option<TItem>> InspectAwait(Func<TItem, ValueTask> action)
        {
            await AndThen(action);
            return this;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that yields exactly one value when the option
        /// has an item and nothing when the option is empty.
        /// </summary>
        [Pure]
        public IEnumerable<TItem> ToEnumerable()
            => Match(
                none: Enumerable.Empty<TItem>,
                some: Sequence.Return);
    }
}
