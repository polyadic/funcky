using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static partial class SyntaxNodeExtensions
{
    public static TNode ReplaceParameterReferences<TNode>(this TNode node, SemanticModel semanticModel, string parameterName, ExpressionSyntax replacement)
        where TNode : SyntaxNode
        => (TNode)new ReplaceParameterReferenceRewriter(semanticModel, parameterName, replacement).Visit(node);

    private sealed class ReplaceParameterReferenceRewriter(
        SemanticModel semanticModel,
        string parameterName,
        ExpressionSyntax replacement)
        : CSharpSyntaxRewriter(visitIntoStructuredTrivia: false)
    {
        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            => semanticModel.GetOperation(node) is IParameterReferenceOperation { Parameter.Name: var name } && name == parameterName
                ? replacement.WithTriviaFrom(node)
                : node;
    }
}
