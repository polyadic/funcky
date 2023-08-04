namespace Funcky.Extensions;

public static class EnumeratorExtensions
{
    /// <summary>Advances the enumerator and returns <c>Some</c> with new element of enumeration
    /// or <see cref="Option{TItem}.None"/> if no more elements are available.</summary>
    public static Option<T> MoveNextOrNone<T>(this IEnumerator<T> enumerator)
        where T : notnull
        => enumerator.MoveNext()
            ? enumerator.Current
            : Option<T>.None;
}
