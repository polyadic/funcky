using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Funcky.GenericConstraints;

namespace Funcky.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ObjectExtensions
    {
        /// <summary>
        /// Wraps this object instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Obsolete("Use " + nameof(Sequence) + "." + nameof(Sequence.FromNullable) + " instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Pure]
        public static IEnumerable<T> ToEnumerable<T>(this T? item, RequireClass<T>? ω = null)
            where T : class
            => Sequence.FromNullable(item);

        /// <inheritdoc cref="ToEnumerable{T}(T, RequireClass{T})"/>
        [Pure]
        [Obsolete("Use " + nameof(Sequence) + "." + nameof(Sequence.Return) + " instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> ToEnumerable<T>(this T item, RequireStruct<T>? ω = null)
            where T : struct
            => Sequence.Return(item);

        /// <inheritdoc cref="ToEnumerable{T}(T, RequireClass{T})"/>
        [Pure]
        [Obsolete("Use " + nameof(Sequence) + "." + nameof(Sequence.FromNullable) + " instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> ToEnumerable<T>(this T? item)
            where T : struct
            => Sequence.FromNullable(item);
    }
}
