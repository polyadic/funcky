using Funcky.GenericConstraints;

namespace Funcky.Monads
{
    public static partial class Option
    {
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
