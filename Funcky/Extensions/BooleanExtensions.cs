namespace Funcky.Extensions;

public static class BooleanExtensions
{
    extension(bool condition)
    {
        /// <summary>Returns <c>Some(value)</c> when the condition is <see langword="true"/>, <c>None</c> otherwise.</summary>
        /// <typeparam name="TItem">Type of the value.</typeparam>
        /// <param name="value">The value to return when the condition is <see langword="true"/>.</param>
        /// <example><code>
        /// Option&lt;int&gt; even = (number % 2 == 0).Then(number);
        /// </code></example>
        [Pure]
        public Option<TItem> Then<TItem>(TItem value)
            where TItem : notnull
            => condition
                ? Option.Some(value)
                : Option<TItem>.None;

        /// <summary>Returns <c>Some(selector())</c> when the condition is <see langword="true"/>, <c>None</c> otherwise.
        /// The selector is only called when the condition is <see langword="true"/>.</summary>
        /// <typeparam name="TItem">Type of the value.</typeparam>
        /// <param name="selector">Produces the value when the condition is <see langword="true"/>.</param>
        /// <example><code>
        /// Option&lt;string&gt; name = user.HasName.Then(() => user.Name);
        /// </code></example>
        [Pure]
        public Option<TItem> Then<TItem>(Func<TItem> selector)
            where TItem : notnull
            => condition
                ? Option.Some(selector())
                : Option<TItem>.None;

        /// <summary>Returns the result of the selector when the condition is <see langword="true"/>, <c>None</c> otherwise.
        /// The selector is only called when the condition is <see langword="true"/>.</summary>
        /// <typeparam name="TItem">Type of the value.</typeparam>
        /// <param name="selector">Produces the option when the condition is <see langword="true"/>.</param>
        /// <example><code>
        /// Option&lt;IFormFile&gt; file = request.HasFormContentType.Then(() => request.Form.Files.FirstOrNone());
        /// </code></example>
        [Pure]
        public Option<TItem> Then<TItem>(Func<Option<TItem>> selector)
            where TItem : notnull
            => condition
                ? selector()
                : Option<TItem>.None;
    }
}
