using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.GenericConstraints;

namespace Funcky
{
    public static partial class Sequence
    {
        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Pure]
        public static IEnumerable<T> FromNullable<T>(T? item, RequireClass<T>? ω = null)
            where T : class
            => item is null ? Enumerable.Empty<T>() : Return(item);

        /// <inheritdoc cref="FromNullable{T}(T, Funcky.GenericConstraints.RequireClass{T})"/>
        [Pure]
        public static IEnumerable<T> FromNullable<T>(T? item, RequireStruct<T>? ω = null)
            where T : struct
            => item.HasValue ? Return(item.Value) : Enumerable.Empty<T>();
    }
}
