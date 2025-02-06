using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static partial class SyntaxNodeExtensions
{
    // Adapted from Roslyn's source code as this API is not public:
    // https://github.com/dotnet/roslyn/blob/232f7afa4966411958759c880de3a1765bdb28a0/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/SyntaxNodeExtensions.cs#L925
    public static bool IsInExpressionTree(
        [NotNullWhen(returnValue: true)] this SyntaxNode? node,
        SemanticModel semanticModel,
        [NotNullWhen(returnValue: true)] INamedTypeSymbol? expressionType,
        CancellationToken cancellationToken)
        => expressionType is not null
            && node is not null
            && node
                .AncestorsAndSelf()
                .Any(current => IsExpressionTree(new(current, expressionType, semanticModel, cancellationToken)));

    private static bool IsExpressionTree(IsExpressionTreeContext context)
        => context.Syntax switch
        {
            var node when node.IsAnyLambda() => LambdaIsExpressionTree(context),
            SelectOrGroupClauseSyntax or OrderingSyntax => QueryExpressionIsExpressionTree(context),
            QueryClauseSyntax queryClause => QueryClauseIsExpressionTree(context, queryClause),
            _ => false,
        };

    private static bool LambdaIsExpressionTree(IsExpressionTreeContext context)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(context.Syntax, context.CancellationToken);
        return SymbolEquals(context.ExpressionType, typeInfo.ConvertedType?.OriginalDefinition);
    }

    private static bool QueryExpressionIsExpressionTree(IsExpressionTreeContext context)
    {
        var info = context.SemanticModel.GetSymbolInfo(context.Syntax, context.CancellationToken);
        return TakesExpressionTree(info, context.ExpressionType);
    }

    private static bool QueryClauseIsExpressionTree(IsExpressionTreeContext context, QueryClauseSyntax queryClause)
    {
        var info = context.SemanticModel.GetQueryClauseInfo(queryClause, context.CancellationToken);
        return TakesExpressionTree(info.CastInfo, context.ExpressionType)
            || TakesExpressionTree(info.OperationInfo, context.ExpressionType);
    }

    private static bool TakesExpressionTree(SymbolInfo info, INamedTypeSymbol expressionType)
        => GetAllSymbols(info).Any(symbol => TakesExpressionTreeAsFirstArgument(symbol, expressionType));

    private static bool TakesExpressionTreeAsFirstArgument(ISymbol symbol, INamedTypeSymbol expressionType)
        => symbol is IMethodSymbol { Parameters: [var firstParameter, ..] }
            && SymbolEquals(expressionType, firstParameter.Type.OriginalDefinition);

    private sealed record IsExpressionTreeContext(
        SyntaxNode Syntax,
        INamedTypeSymbol ExpressionType,
        SemanticModel SemanticModel,
        CancellationToken CancellationToken);
}
