using Microsoft.CodeAnalysis;

namespace Funcky.SourceGenerator.Extensions;

internal static class EnumerableExtensions
{
    public static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> nodes)
        where T : SyntaxNode
        => default(SyntaxList<T>).AddRange(nodes);
}
