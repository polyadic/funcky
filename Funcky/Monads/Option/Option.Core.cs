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
        public static Option<TItem> None => default;

        [Pure]
        public TResult Match<TResult>(TResult none, Func<TItem, TResult> some)
            => Match(none: () => none, some: some);

        /// <summary>
        /// <para>Calls either <paramref name="none"/> when the option has no value or <paramref name="some"/> when the option
        /// has a value. Serves the same purpose as a switch expression.</para>
        /// <para>Note that there are often better alternatives available:
        /// <list type="bullet">
        /// <item><description><see cref="Select{TResult}"/></description></item>
        /// <item><description><see cref="SelectMany{TResult}"/></description></item>
        /// <item><description><see cref="OrElse(Func{Option{TItem}})"/></description></item>
        /// <item><description><see cref="GetOrElse(TItem)"/></description></item>
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
        /// <item><description><see cref="AndThen(System.Action{TItem})"/></description></item>
        /// <item><description><see cref="Inspect(System.Action{TItem})"/></description></item>
        /// </list>
        /// </para>
        /// </summary>
        public void Switch(Action none, Action<TItem> some)
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

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <c>null</c>.</exception>
        [Pure]
        public static Option<TItem> Return<TItem>(TItem item)
            where TItem : notnull
            => new(item);
    }
}
