namespace Funcky.Extensions;

public static class EnumeratorExtensions
{
    extension<T>(IEnumerator<T> enumerator)
        where T : notnull
    {
        /// <summary>Advances the enumerator and returns <c>Some</c> with new element of enumeration
        /// or <see cref="Option{TItem}.None"/> if no more elements are available.</summary>
        public Option<T> MoveNextOrNone()
            => enumerator.MoveNext()
                ? enumerator.Current
                : Option<T>.None;
    }
}
