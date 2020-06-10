using System;
using System.Collections.Generic;
using System.Linq;
using static Funcky.Functional;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        public Option<TItem> Where(Func<TItem, bool> predicate)
            => SelectMany(item => predicate(item) ? Option.Some(item) : None());

        public Option<TItem> OrElse(Option<TItem> elseOption)
            => Match(none: elseOption, some: Option.Some);

        public TItem OrElse(TItem elseOption)
            => Match(none: elseOption, some: Identity);

        public Option<TItem> OrElse(Func<Option<TItem>> elseOption)
            => Match(none: elseOption, some: Option.Some);

        public TItem OrElse(Func<TItem> elseOption)
            => Match(none: elseOption, some: Identity);

        public Option<TResult> AndThen<TResult>(Func<TItem, TResult> andThenFunction)
            where TResult : notnull
            => Select(andThenFunction);

        public Option<TResult> AndThen<TResult>(Func<TItem, Option<TResult>> andThenFunction)
            where TResult : notnull
            => SelectMany(andThenFunction);

        public void AndThen(Action<TItem> andThenFunction)
            => Match(none: NoOperation, some: andThenFunction);

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