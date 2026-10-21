namespace Funcky.Extensions;

public static partial class FuncExtensions
{
    extension<T>(Func<T, bool> predicate)
    {
        /// <summary>
        /// Returns a new predicate that inverts this predicate.
        /// </summary>
        /// <remarks>
        /// The extension form of <see cref="Functional.Not{T}"/>; the type argument is inferred from the receiver.
        /// </remarks>
        [Pure]
        public Func<T, bool> Not()
            => value => !predicate(value);
    }

    /// <summary>
    /// Returns a new predicate that is true when this predicate and all given predicates are true.
    /// </summary>
    /// <remarks>
    /// The extension form of <see cref="Functional.All{T}"/>; the type argument is inferred from the receiver.
    /// </remarks>
    [Pure]
    public static Func<T, bool> And<T>(this Func<T, bool> predicate, params Func<T, bool>[] predicates)
        => value => predicate(value) && predicates.All(other => other(value));

    /// <summary>
    /// Returns a new predicate that is true when this predicate or any of the given predicates is true.
    /// </summary>
    /// <remarks>
    /// The extension form of <see cref="Functional.Any{T}"/>; the type argument is inferred from the receiver.
    /// </remarks>
    [Pure]
    public static Func<T, bool> Or<T>(this Func<T, bool> predicate, params Func<T, bool>[] predicates)
        => value => predicate(value) || predicates.Any(other => other(value));
}
