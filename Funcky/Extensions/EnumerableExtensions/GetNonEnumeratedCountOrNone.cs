namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>Returns the number of elements in <paramref name="source"/> if it can be determined without enumerating the sequence.</summary>
        /// <remarks>On .NET 6 and later this uses <c>Enumerable.TryGetNonEnumeratedCount</c>; on older frameworks only collections with a <c>Count</c> property are recognized.</remarks>
        [Pure]
        public Option<int> GetNonEnumeratedCountOrNone()
            => source.TryGetNonEnumeratedCount(out var count)
                ? count
                : Option<int>.None;
    }
}
