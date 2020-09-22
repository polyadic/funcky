using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.Extensions
{
    internal static class CSharpSyntaxNodeExtension
    {
        public static bool IsTrueLiteral(this CSharpSyntaxNode node)
            => node is LiteralExpressionSyntax literal && literal.Kind() == SyntaxKind.TrueLiteralExpression;

        public static bool IsFalseLiteral(this CSharpSyntaxNode node)
            => node is LiteralExpressionSyntax literal && literal.Kind() == SyntaxKind.FalseLiteralExpression;
    }
}
