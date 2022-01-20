using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.SourceGenerator;

public class OrNoneFromTryPatternRewriter : CSharpSyntaxRewriter
{
    private const string FailToOption = "FailToOption";
    private const string FromTryPattern = "FromTryPattern";
    private readonly string _methodName;
    private readonly TypeSyntax _typeSyntax;

    public OrNoneFromTryPatternRewriter(string methodName, TypeSyntax typeSyntax)
    {
        _methodName = methodName;
        _typeSyntax = typeSyntax;
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax methodDeclaration)
        => WithImplementation(methodDeclaration.WithAttributeLists(default));

    private SyntaxList<AttributeListSyntax> RemoveOrNoneFromTryPatternAttribute(MethodDeclarationSyntax methodDeclaration)
    {
        var nodeToRemove = methodDeclaration.GetAttributeByUsedName("OrNoneFromTryPattern");

        return methodDeclaration.AttributeLists.Aggregate(default(SyntaxList<AttributeListSyntax>), (current, attributeList) => AttributeListSyntaxes(current, attributeList, nodeToRemove));
    }

    private SyntaxList<AttributeListSyntax> AttributeListSyntaxes(SyntaxList<AttributeListSyntax> newAttributes, AttributeListSyntax attributeList, AttributeSyntax nodeToRemove)
    {
        return attributeList.RemoveNode(nodeToRemove, SyntaxRemoveOptions.KeepNoTrivia) is { Attributes.Count: > 0 } cleanedAttributeList
               && VisitAttributeList(cleanedAttributeList) is AttributeListSyntax newAttribute
            ? newAttributes.Add(newAttribute)
            : newAttributes;
    }

    private MethodDeclarationSyntax WithImplementation(MethodDeclarationSyntax methodDeclaration)
        => methodDeclaration
            .WithExpressionBody(ArrowExpressionClause(CallFromTryPattern().WithArgumentList(ForwardAllArguments(methodDeclaration))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private InvocationExpressionSyntax CallFromTryPattern()
        => InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                GenericName(Identifier(FailToOption)).AddTypeArgumentListArguments(_typeSyntax),
                IdentifierName(FromTryPattern)));

    private ArgumentListSyntax ForwardAllArguments(MethodDeclarationSyntax methodDeclaration)
        => ArgumentList(SeparatedList<ArgumentSyntax>(MethodGroupAndAllArguments(methodDeclaration)));

    private SyntaxNodeOrToken[] MethodGroupAndAllArguments(MethodDeclarationSyntax methodDeclarationSyntax)
    {
        var result = new List<SyntaxNodeOrToken>
        {
            MethodGroupArgument(),
        };

        foreach (var parameter in methodDeclarationSyntax.ParameterList.Parameters)
        {
            result.Add(Token(SyntaxKind.CommaToken));
            result.Add(Argument(IdentifierName(parameter.Identifier.Text)));
        }

        return result.ToArray();
    }

    private ArgumentSyntax MethodGroupArgument()
        => Argument(MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            _typeSyntax,
            IdentifierName(_methodName)));
}
