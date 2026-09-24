using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class SyntaxGeneratorExtensions
{
    // Source: https://github.com/dotnet/roslyn-analyzers/blob/f24a5b42c85be6ee572f3a93bef223767fbefd75/src/Utilities/Workspaces/SyntaxGeneratorExtensions.cs#L68-L74
    // This is a workaround for https://github.com/dotnet/roslyn/issues/43950.
    public static SyntaxNode TypeExpressionForStaticMemberAccess(this SyntaxGenerator generator, INamedTypeSymbol typeSymbol)
    {
        var qualifiedNameSyntaxKind = generator.QualifiedName(generator.IdentifierName("ignored"), generator.IdentifierName("ignored")).RawKind;

        var typeExpression = generator.TypeExpression(typeSymbol);
        return QualifiedNameToMemberAccess(qualifiedNameSyntaxKind, typeExpression, generator);

        static SyntaxNode QualifiedNameToMemberAccess(int qualifiedNameSyntaxKind, SyntaxNode expression, SyntaxGenerator generator)
        {
            if (expression.RawKind == qualifiedNameSyntaxKind)
            {
                var left = QualifiedNameToMemberAccess(qualifiedNameSyntaxKind, expression.ChildNodes().First(), generator);
                var right = expression.ChildNodes().Last();
                return generator.MemberAccessExpression(left, right);
            }

            return expression;
        }
    }
}
