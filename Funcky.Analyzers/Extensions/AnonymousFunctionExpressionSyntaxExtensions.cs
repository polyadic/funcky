using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.Extensions
{
    internal static class AnonymousFunctionExpressionSyntaxExtensions
    {
        public static ITypeSymbol? GetReturnType(
            this AnonymousFunctionExpressionSyntax expression,
            SyntaxNodeAnalysisContext context)
            => context.SemanticModel.GetOperation(expression) is IAnonymousFunctionOperation functionOperation
                ? functionOperation.Symbol.ReturnType
                : null;

        public static bool HasBooleanAsReturnType(
            this AnonymousFunctionExpressionSyntax expression,
            SyntaxNodeAnalysisContext context)
            => SymbolEqualityComparer.IncludeNullability.Equals(expression.GetReturnType(context), GetBooleanType(context));

        private static ITypeSymbol? GetBooleanType(SyntaxNodeAnalysisContext context)
            => context.Compilation?.GetSpecialType(SpecialType.System_Boolean);
    }
}
