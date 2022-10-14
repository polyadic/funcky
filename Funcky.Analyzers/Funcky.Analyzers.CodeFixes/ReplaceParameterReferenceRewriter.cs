using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal sealed class ReplaceParameterReferenceRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly string _parameterName;
    private readonly ExpressionSyntax _replacement;

    public ReplaceParameterReferenceRewriter(SemanticModel semanticModel, string parameterName, ExpressionSyntax replacement)
        : base(visitIntoStructuredTrivia: false)
    {
        _semanticModel = semanticModel;
        _parameterName = parameterName;
        _replacement = replacement;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (_semanticModel.GetOperation(node) is IParameterReferenceOperation { Parameter.Name: var name } && name == _parameterName)
        {
            return _replacement.WithTriviaFrom(node);
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
