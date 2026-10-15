namespace Funcky;

public static partial class Sequence
{
    /// <summary>Creates a sequence that contains exactly one element.</summary>
    /// <remarks>This is the monadic <c>Return</c> for <see cref="IEnumerable{T}"/>, the counterpart of <c>Option.Return</c> and <c>Result.Return</c>.</remarks>
    /// <typeparam name="TResult">Type of the element.</typeparam>
    /// <param name="element">The single element of the sequence.</param>
    /// <returns>A materialized sequence with exactly one element.</returns>
    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(TResult element)
        => Return(elements: element);

    /// <summary>Creates a sequence that contains exactly the given elements, in order.</summary>
    /// <remarks>
    /// <para>The result is a read-only view over <paramref name="elements"/>: the array is not copied, so later changes to an
    /// array passed in show through, but the view cannot be cast back to a mutable array or list.</para>
    /// <para>Passing an array binds to this overload, so <c>Sequence.Return(array)</c> is a sequence of the array's elements,
    /// not a sequence containing the array. Use <c>Sequence.Return&lt;T[]&gt;(array)</c> for the latter.</para>
    /// </remarks>
    /// <typeparam name="TResult">Type of the elements.</typeparam>
    /// <param name="elements">The elements of the sequence.</param>
    /// <returns>A materialized, read-only sequence of the given elements.</returns>
    [Pure]
    public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
        => Array.AsReadOnly(elements);
}
