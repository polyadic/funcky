namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
#if TRY_GET_NON_ENUMERATED_COUNT
    public static Option<int> GetNonEnumeratedCountOrNone<TSource>(this IEnumerable<TSource> source)
        => source.TryGetNonEnumeratedCount(out var count)
            ? count
            : Option.None<int>();
#endif
}
