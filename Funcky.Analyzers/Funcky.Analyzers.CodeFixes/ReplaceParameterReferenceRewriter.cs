using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal sealed class ReplaceParameterReferenceRewriter(
    SemanticModel semanticModel,
    string parameterName,
    ExpressionSyntax replacement)
    : CSharpSyntaxRewriter(visitIntoStructuredTrivia: false)
{
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (semanticModel.GetOperation(node) is IParameterReferenceOperation { Parameter.Name: var name } && name == parameterName)
        {
            return replacement.WithTriviaFrom(node);
        }

        return node;
    }
}

internal static partial class SyntaxNodeExtensions
{
    public static TNode ReplaceParameterReferences<TNode>(this TNode node, SemanticModel semanticModel, string parameterName, ExpressionSyntax replacement)
        where TNode : SyntaxNode
        => (TNode)new ReplaceParameterReferenceRewriter(semanticModel, parameterName, replacement).Visit(node);
}
