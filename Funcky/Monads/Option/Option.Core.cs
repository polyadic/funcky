using System;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace Funcky.Monads
{
    [JsonConverter(typeof(JsonOptionConverterFactory))]
    public readonly partial struct Option<TItem>
        where TItem : notnull
    {
        private readonly bool _hasItem;
        private readonly TItem _item;

        internal Option(TItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _item = item;
            _hasItem = true;
        }

        [Pure]
        public static Option<TItem> None() => default;

        [Pure]
        public Option<TResult> Select<TResult>(Func<TItem, TResult> selector)
            where TResult : notnull
            => Match(
                 none: Option<TResult>.None,
                 item => selector(item));

        [Pure]
        public Option<TResult> SelectMany<TResult>(Func<TItem, Option<TResult>> selector)
            where TResult : notnull
            => Match(
                 none: Option<TResult>.None,
                 some: selector);

        [Pure]
        public Option<TResult> SelectMany<TMaybe, TResult>(Func<TItem, Option<TMaybe>> maybeSelector, Func<TItem, TMaybe, TResult> resultSelector)
            where TResult : notnull
            where TMaybe : notnull
            => SelectMany(
                 item => maybeSelector(item).Select(
                     maybe => resultSelector(item, maybe)));

        [Pure]
        public TResult Match<TResult>(TResult none, Func<TItem, TResult> some)
            => Match(() => none, some);

        /// <summary>
        /// <para>Calls either <paramref name="none"/> when the option has no value or <paramref name="some"/> when the option
        /// has a value. Serves the same purpose as a switch expression.</para>
        /// <para>Note that there are often better alternatives available:
        /// <list type="bullet">
        /// <item><description><see cref="Select{TResult}"/></description></item>
        /// <item><description><see cref="SelectMany{TResult}"/></description></item>
        /// <item><description><see cref="OrElse(Func{Option{TItem}})"/></description></item>
        /// <item><description><seealso cref="GetOrElse(TItem)"/></description></item>
        /// </list></para>
        /// </summary>
        [Pure]
        public TResult Match<TResult>(Func<TResult> none, Func<TItem, TResult> some)
            => _hasItem
                  ? some(_item)
                  : none();

        /// <summary>
        /// <para>Calls either <paramref name="none"/> when the option has no value or <paramref name="some"/> when the option
        /// has a value. Serves the same purpose as a switch statement.</para>
        /// <para>Note that there are often better alternatives available, such as:
        /// <list type="bullet">
        /// <item><description><seealso cref="AndThen(System.Action{TItem})"/></description></item>
        /// <item><description><seealso cref="Inspect(System.Action{TItem})"/></description></item>
        /// </list>
        /// </para>
        /// </summary>
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

        [Pure]
        public override string ToString()
            => Match(
                 none: "None",
                 some: value => $"Some({value})");
    }

    public static partial class Option
    {
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <c>null</c>.</exception>
        [Pure]
        public static Option<TItem> Some<TItem>(TItem item)
            where TItem : notnull
            => new(item);
    }
}
