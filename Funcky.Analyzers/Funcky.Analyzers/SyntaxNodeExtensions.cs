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
            && node.AncestorsAndSelf().Any(current => IsExpressionTree(current, expressionTypeOpt, semanticModel, cancellationToken));

        static bool IsExpressionTree(
            SyntaxNode current,
            INamedTypeSymbol expressionType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return current switch {
                _ when current.IsAnyLambda() => LambdaIsExpressionTree(),
                SelectOrGroupClauseSyntax or OrderingSyntax => QueryExpressionIsExpressionTree(),
                QueryClauseSyntax queryClause => QueryClauseIsExpressionTree(queryClause),
                _ => false,
            };

            bool LambdaIsExpressionTree()
            {
                var typeInfo = semanticModel.GetTypeInfo(current, cancellationToken);
                return SymbolEqualityComparer.Default.Equals(expressionType, typeInfo.ConvertedType?.OriginalDefinition);
            }

            bool QueryExpressionIsExpressionTree()
            {
                var info = semanticModel.GetSymbolInfo(current, cancellationToken);
                return TakesExpressionTree(info, expressionType);
            }

            bool QueryClauseIsExpressionTree(QueryClauseSyntax queryClause)
            {
                var info = semanticModel.GetQueryClauseInfo(queryClause, cancellationToken);
                return TakesExpressionTree(info.CastInfo, expressionType)
                    || TakesExpressionTree(info.OperationInfo, expressionType);
            }
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
}
