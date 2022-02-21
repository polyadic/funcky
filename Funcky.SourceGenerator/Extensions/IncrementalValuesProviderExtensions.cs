using Microsoft.CodeAnalysis;

namespace Funcky.SourceGenerator.Extensions
{
    internal static class IncrementalValuesProviderExtensions
    {
        public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(this IncrementalValuesProvider<TSource?> source)
            => source.Where(x => x is not null)!;
    }
}
