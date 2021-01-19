using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funcky.Extensions;

namespace Funcky.Monads
{
    public static class ValueTaskOptionExtensions
    {
        [Pure]
        public static async ValueTask<Option<TResult>> Select<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, TResult> selector)
            where TItem : notnull
            where TResult : notnull
            => (await option).Select(selector);

        [Pure]
        public static async ValueTask<Option<TResult>> SelectAwait<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask<TResult>> selector)
            where TItem : notnull
            where TResult : notnull
            => await (await option).SelectAwait(selector);

        [Pure]
        public static async ValueTask<Option<TResult>> SelectMany<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, Option<TResult>> selector)
            where TItem : notnull
            where TResult : notnull
            => (await option).SelectMany(selector);

        [Pure]
        public static async ValueTask<Option<TResult>> SelectManyAwait<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask<Option<TResult>>> selector)
            where TItem : notnull
            where TResult : notnull
            => await (await option).SelectManyAwait(selector);

        [Pure]
        public static async ValueTask<Option<TResult>> SelectMany<TItem, TMaybe, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, Option<TMaybe>> maybeSelector, Func<TItem, TMaybe, TResult> resultSelector)
            where TItem : notnull
            where TResult : notnull
            where TMaybe : notnull
            => (await option).SelectMany(maybeSelector, resultSelector);

        [Pure]
        public static async ValueTask<TResult> Match<TItem, TResult>(this ValueTask<Option<TItem>> option, TResult none, Func<TItem, TResult> some)
            where TItem : notnull
            => (await option).Match(none, some);

        [Pure]
        public static async ValueTask<TResult> Match<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TResult> none, Func<TItem, TResult> some)
            where TItem : notnull
            => (await option).Match(none, some);

        public static async ValueTask Match<TItem>(this ValueTask<Option<TItem>> option, Action none, Action<TItem> some)
            where TItem : notnull
            => (await option).Match(none, some);

        [Pure]
        public static async ValueTask<Option<TItem>> WhereAwait<TItem>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask<bool>> predicate)
            where TItem : notnull
            => await (await option).WhereAwait(predicate);

        [Pure]
        public static async ValueTask<Option<TItem>> OrElse<TItem>(this ValueTask<Option<TItem>> option, Option<TItem> elseOption)
            where TItem : notnull
            => (await option).OrElse(elseOption);

        [Pure]
        public static async ValueTask<Option<TItem>> OrElse<TItem>(this ValueTask<Option<TItem>> option, Func<Option<TItem>> elseOption)
            where TItem : notnull
            => (await option).OrElse(elseOption);

        [Pure]
        public static async ValueTask<Option<TItem>> OrElseAwait<TItem>(this ValueTask<Option<TItem>> option, Func<ValueTask<Option<TItem>>> elseOption)
            where TItem : notnull
            => await (await option).OrElseAwait(elseOption);

        [Pure]
        public static async ValueTask<TItem> GetOrElse<TItem>(this ValueTask<Option<TItem>> option, TItem elseOption)
            where TItem : notnull
            => (await option).GetOrElse(elseOption);

        [Pure]
        public static async ValueTask<TItem> GetOrElse<TItem>(this ValueTask<Option<TItem>> option, Func<TItem> elseOption)
            where TItem : notnull
            => (await option).GetOrElse(elseOption);

        [Pure]
        public static async ValueTask<TItem> GetOrElseAwait<TItem>(this ValueTask<Option<TItem>> option, Func<ValueTask<TItem>> elseOption)
            where TItem : notnull
            => await (await option).GetOrElseAwait(elseOption);

        [Pure]
        public static async ValueTask<Option<TResult>> AndThen<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, TResult> andThenFunction)
            where TItem : notnull
            where TResult : notnull
            => (await option).AndThen(andThenFunction);

        [Pure]
        public static async ValueTask<Option<TResult>> AndThenAwait<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask<TResult>> andThenFunction)
            where TItem : notnull
            where TResult : notnull
            => await (await option).AndThenAwait(andThenFunction);

        [Pure]
        public static async ValueTask<Option<TResult>> AndThen<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, Option<TResult>> andThenFunction)
            where TItem : notnull
            where TResult : notnull
            => (await option).AndThen(andThenFunction);

        [Pure]
        public static async ValueTask<Option<TResult>> AndThenAwait<TItem, TResult>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask<Option<TResult>>> andThenFunction)
            where TItem : notnull
            where TResult : notnull
            => await (await option).AndThenAwait(andThenFunction);

        /// <summary>
        /// Performs a side effect when the option has a value.
        /// </summary>
        public static async ValueTask AndThen<TItem>(this ValueTask<Option<TItem>> option, Action<TItem> andThenFunction)
            where TItem : notnull
            => (await option).AndThen(andThenFunction);

        /// <summary>
        /// Performs a side effect when the option has a value and returns the option again.
        /// This is the <see cref="Option{T}"/> equivalent of <see cref="EnumerableExtensions.Inspect{T}(IEnumerable{T}, Action{T})"/>.
        /// </summary>
        public static async ValueTask<Option<TItem>> Inspect<TItem>(this ValueTask<Option<TItem>> option, Action<TItem> action)
            where TItem : notnull
            => (await option).Inspect(action);

        public static async ValueTask<Option<TItem>> InspectAwait<TItem>(this ValueTask<Option<TItem>> option, Func<TItem, ValueTask> action)
            where TItem : notnull
            => await (await option).InspectAwait(action);

        /// <summary>
        /// Returns an <see cref="IAsyncEnumerable{T}"/> that yields exactly one value when the option
        /// has an item and nothing when the option is empty.
        /// </summary>
        [Pure]
        public static async IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(this ValueTask<Option<TItem>> option)
            where TItem : notnull
        {
            if ((await option).TryGetValue(out var item))
            {
                yield return item;
            }
        }
    }
}
