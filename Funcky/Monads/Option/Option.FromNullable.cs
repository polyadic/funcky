namespace Funcky.Monads
{
    public static partial class Option
    {
        /// <summary>
        /// Creates an <see cref="Option{T}"/> from a nullable value.
        /// </summary>
        [Pure]
        public static Option<T> FromNullable<T>(T? item)
            where T : class
            => item is { } value ? Some(value) : Option<T>.None();

        /// <summary>
        /// Creates an <see cref="Option{T}"/> from a nullable value.
        /// </summary>
        [Pure]
        public static Option<T> FromNullable<T>(T? item)
            where T : struct
            => item.HasValue ? Some(item.Value) : Option<T>.None();
    }
}
