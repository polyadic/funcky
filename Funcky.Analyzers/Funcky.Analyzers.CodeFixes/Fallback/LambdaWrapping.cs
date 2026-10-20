using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.Fallback;

internal static class LambdaWrapping
{
    private const string DiscardName = "_";

    /// <summary>Whether the expression may move into a synchronous lambda: no <c>await</c>, no reference to a <c>ref</c>,
    /// <c>in</c> or <c>out</c> parameter, a <c>ref</c> local or a <c>ref struct</c> value, and no <c>this</c> inside a struct, because a
    /// lambda cannot capture any of those. A variable the expression declares (<c>out var x</c>, <c>is T x</c>) would be scoped
    /// to the lambda and vanish for the code after the call. Nested functions are not inspected, what happens inside them is deferred already.</summary>
    public static bool CanWrapInLambda(SemanticModel semanticModel, ExpressionSyntax expression, CancellationToken cancellationToken)
    {
        var enclosingType = semanticModel.GetEnclosingSymbol(expression.SpanStart, cancellationToken)?.ContainingType;

        return expression
            .DescendantNodesAndSelf(descendIntoChildren: node => node is not AnonymousFunctionExpressionSyntax and not LocalFunctionStatementSyntax)
            .All(node => node switch
            {
                AwaitExpressionSyntax => false,
                SingleVariableDesignationSyntax => false,
                ThisExpressionSyntax or BaseExpressionSyntax => enclosingType is not { IsValueType: true },
                IdentifierNameSyntax identifier => IsCapturable(semanticModel.GetSymbolInfo(identifier, cancellationToken).Symbol, identifier, enclosingType),
                _ => true,
            });
    }

    /// <summary>Replaces an eagerly evaluated argument expression with a lambda producing it, so that the lazy overload is
    /// picked: <c>() => expression</c> for <c>Option</c>, <c>_ => expression</c> for <c>Result</c> and <c>Either</c>.</summary>
    public static async Task<Document> WrapInLambdaAsync(Document document, ExpressionSyntax expression, int parameterCount, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var parameterName = parameterCount == 1 ? await UnusedParameterNameAsync(document, expression, cancellationToken).ConfigureAwait(false) : DiscardName;
        editor.ReplaceNode(expression, Lambda(expression.WithoutTrivia(), parameterCount, parameterName).WithTriviaFrom(expression));
        return editor.GetChangedDocument();
    }

    private static bool IsCapturable(ISymbol? symbol, IdentifierNameSyntax identifier, INamedTypeSymbol? enclosingType)
        => symbol switch
        {
            IParameterSymbol parameter => parameter.RefKind == RefKind.None && !parameter.Type.IsRefLikeType,
            ILocalSymbol local => !local.IsRef && !local.Type.IsRefLikeType,
            IFieldSymbol or IPropertySymbol or IEventSymbol or IMethodSymbol => symbol.IsStatic || HasExplicitReceiver(identifier) || enclosingType is not { IsValueType: true },
            _ => true,
        };

    // `x.Member` names its receiver, a bare `Member` is an implicit `this`, which a lambda in a struct cannot capture.
    private static bool HasExplicitReceiver(IdentifierNameSyntax identifier)
        => identifier.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == identifier;

    // A single `_` parameter is a regular parameter, not a discard: it would shadow a `_` in scope, clash with a local of
    // that name or turn an `out _` discard inside the expression into a reference to the parameter. The name is extended
    // until nothing of that name is visible at the argument or written inside the expression.
    private static async Task<string> UnusedParameterNameAsync(Document document, ExpressionSyntax expression, CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var identifiersInExpression = expression.DescendantTokens().Where(token => token.IsKind(SyntaxKind.IdentifierToken)).Select(token => token.ValueText).ToImmutableHashSet();
        var name = DiscardName;

        while (identifiersInExpression.Contains(name) || (semanticModel is not null && semanticModel.LookupSymbols(expression.SpanStart, name: name).Length > 0))
        {
            name += DiscardName;
        }

        return name;
    }

    private static LambdaExpressionSyntax Lambda(ExpressionSyntax body, int parameterCount, string parameterName)
        => parameterCount == 1
            ? SimpleLambdaExpression(Parameter(Identifier(parameterName)), body)
            : ParenthesizedLambdaExpression(
                ParameterList(SeparatedList(Enumerable.Repeat(Parameter(Identifier(parameterName)), parameterCount))),
                body);
}
