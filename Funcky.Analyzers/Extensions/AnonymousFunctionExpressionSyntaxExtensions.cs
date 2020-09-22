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
    }
}
