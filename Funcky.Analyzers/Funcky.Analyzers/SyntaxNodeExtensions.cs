using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers;

internal static class SyntaxNodeExtensions
{
    // Copied from Roslyn's source code as this API is not public:
    // https://github.com/dotnet/roslyn/blob/232f7afa4966411958759c880de3a1765bdb28a0/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/SyntaxNodeExtensions.cs#L925
    public static bool IsInExpressionTree(
        [NotNullWhen(returnValue: true)] this SyntaxNode? node,
        SemanticModel semanticModel,
        [NotNullWhen(returnValue: true)] INamedTypeSymbol? expressionTypeOpt,
        CancellationToken cancellationToken)
    {
        if (expressionTypeOpt != null)
        {
            for (var current = node; current != null; current = current.Parent)
            {
                if (current.IsAnyLambda())
                {
                    var typeInfo = semanticModel.GetTypeInfo(current, cancellationToken);
                    if (SymbolEqualityComparer.Default.Equals(expressionTypeOpt, typeInfo.ConvertedType?.OriginalDefinition))
                    {
                        return true;
                    }
                }
                else if (current is SelectOrGroupClauseSyntax or OrderingSyntax)
                {
                    var info = semanticModel.GetSymbolInfo(current, cancellationToken);
                    if (TakesExpressionTree(info, expressionTypeOpt))
                    {
                        return true;
                    }
                }
                else if (current is QueryClauseSyntax queryClause)
                {
                    var info = semanticModel.GetQueryClauseInfo(queryClause, cancellationToken);
                    if (TakesExpressionTree(info.CastInfo, expressionTypeOpt) ||
                        TakesExpressionTree(info.OperationInfo, expressionTypeOpt))
                    {
                        return true;
                    }
                }
            }
        }

        return false;

        static bool TakesExpressionTree(SymbolInfo info, INamedTypeSymbol expressionType)
        {
            foreach (var symbol in GetAllSymbols(info))
            {
                if (symbol is IMethodSymbol method &&
                    method.Parameters.Length > 0 &&
                    SymbolEqualityComparer.Default.Equals(expressionType, method.Parameters[0].Type?.OriginalDefinition))
                {
                    return true;
                }
            }

            return false;
        }
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
