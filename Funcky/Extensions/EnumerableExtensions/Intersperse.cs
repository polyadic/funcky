namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public IEnumerable<TSource> Intersperse(TSource element)
            => source.WithFirst()
                .SelectMany(s => s.IsFirst
                    ? Enumerable.Return(s.Value)
                    : Enumerable.Return(element, s.Value));
    }
}
