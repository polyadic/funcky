using System.Collections.Immutable;

namespace Funcky.Internal;

internal static class Materializations
{
    public static IReadOnlyCollection<TItem> DefaultMaterialization<TItem>(IEnumerable<TItem> source)
        => source.ToImmutableList();
}
