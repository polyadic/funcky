using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers;

internal static class SyntaxNodeExtensions
{
    // Adapted from Roslyn's source code as this API is not public:
    // https://github.com/dotnet/roslyn/blob/232f7afa4966411958759c880de3a1765bdb28a0/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/SyntaxNodeExtensions.cs#L925
    public static bool IsInExpressionTree(
        [NotNullWhen(returnValue: true)] this SyntaxNode? node,
        SemanticModel semanticModel,
        [NotNullWhen(returnValue: true)] INamedTypeSymbol? expressionTypeOpt,
        CancellationToken cancellationToken)
    {
        return expressionTypeOpt is not null
            && node is not null
            && node.AncestorsAndSelf().Any(current => IsExpressionTree(new IsExpressionTreeContext(current, expressionTypeOpt, semanticModel, cancellationToken)));

        static bool IsExpressionTree(IsExpressionTreeContext context)
        {
            return context.Syntax switch {
                var node when node.IsAnyLambda() => LambdaIsExpressionTree(context),
                SelectOrGroupClauseSyntax or OrderingSyntax => QueryExpressionIsExpressionTree(context),
                QueryClauseSyntax queryClause => QueryClauseIsExpressionTree(context, queryClause),
                _ => false,
            };
        }

        static bool LambdaIsExpressionTree(IsExpressionTreeContext context)
        {
            var typeInfo = context.SemanticModel.GetTypeInfo(context.Syntax, context.CancellationToken);
            return SymbolEqualityComparer.Default.Equals(context.ExpressionType, typeInfo.ConvertedType?.OriginalDefinition);
        }

        static bool QueryExpressionIsExpressionTree(IsExpressionTreeContext context)
        {
            var info = context.SemanticModel.GetSymbolInfo(context.Syntax, context.CancellationToken);
            return TakesExpressionTree(info, context.ExpressionType);
        }

        static bool QueryClauseIsExpressionTree(IsExpressionTreeContext context, QueryClauseSyntax queryClause)
        {
            var info = context.SemanticModel.GetQueryClauseInfo(queryClause, context.CancellationToken);
            return TakesExpressionTree(info.CastInfo, context.ExpressionType)
                || TakesExpressionTree(info.OperationInfo, context.ExpressionType);
        }

        static bool TakesExpressionTree(SymbolInfo info, INamedTypeSymbol expressionType)
            => GetAllSymbols(info).Any(symbol => TakesExpressionTreeAsFirstArgument(symbol, expressionType));

        static bool TakesExpressionTreeAsFirstArgument(ISymbol symbol, INamedTypeSymbol expressionType)
            => symbol is IMethodSymbol method
                && method.Parameters.Length > 0
                && SymbolEqualityComparer.Default.Equals(expressionType, method.Parameters[0].Type?.OriginalDefinition);
    }

    internal static ImmutableArray<ISymbol> GetAllSymbols(SymbolInfo info)
        => info.Symbol == null
            ? info.CandidateSymbols
            : ImmutableArray.Create(info.Symbol);

    // Copied from Roslyn's source code as this API is not public:
    // https://github.com/dotnet/roslyn/blob/232f7afa4966411958759c880de3a1765bdb28a0/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/SyntaxNodeExtensions.cs#L925
    internal static bool IsAnyLambda([NotNullWhen(returnValue: true)] this SyntaxNode? node)
        => node?.Kind() is SyntaxKind.ParenthesizedLambdaExpression or SyntaxKind.SimpleLambdaExpression;

    private sealed record IsExpressionTreeContext(
        SyntaxNode Syntax,
        INamedTypeSymbol ExpressionType,
        SemanticModel SemanticModel,
        CancellationToken CancellationToken);
}
