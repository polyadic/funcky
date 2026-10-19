using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.Fallback;

internal static class LambdaWrapping
{
    /// <summary>Replaces an eagerly evaluated argument expression with a lambda producing it, so that the lazy overload is
    /// picked: <c>() => expression</c> for <c>Option</c>, <c>_ => expression</c> for <c>Result</c> and <c>Either</c>.</summary>
    public static async Task<Document> WrapInLambdaAsync(Document document, ExpressionSyntax expression, int parameterCount, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(expression, Lambda(expression.WithoutTrivia(), parameterCount).WithTriviaFrom(expression));
        return editor.GetChangedDocument();
    }

    private static LambdaExpressionSyntax Lambda(ExpressionSyntax body, int parameterCount)
        => parameterCount == 1
            ? SimpleLambdaExpression(Parameter(Identifier("_")), body)
            : ParenthesizedLambdaExpression(
                ParameterList(SeparatedList(Enumerable.Repeat(Parameter(Identifier("_")), parameterCount))),
                body);
}
