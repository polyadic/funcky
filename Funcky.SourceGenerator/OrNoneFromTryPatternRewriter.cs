using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.SourceGenerator;

public class OrNoneFromTryPatternRewriter : CSharpSyntaxRewriter
{
    private static readonly SyntaxToken OutParameter = Identifier("result");
    private static readonly SimpleNameSyntax OptionNone = IdentifierName("None");

    private readonly string _methodName;
    private readonly TypeSyntax _typeSyntax;

    public OrNoneFromTryPatternRewriter(string methodName, TypeSyntax typeSyntax)
    {
        _methodName = methodName;
        _typeSyntax = typeSyntax;
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax methodDeclaration)
        => WithImplementation(methodDeclaration.WithAttributeLists(default));

    private MethodDeclarationSyntax WithImplementation(MethodDeclarationSyntax methodDeclaration)
        => methodDeclaration
            .WithExpressionBody(
                ArrowExpressionClause(
                    ConditionalExpression(
                        TryParseCondition(methodDeclaration),
                        IdentifierName(OutParameter),
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, methodDeclaration.ReturnType, OptionNone))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private InvocationExpressionSyntax TryParseCondition(MethodDeclarationSyntax methodDeclaration)
        => InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _typeSyntax,
                IdentifierName(_methodName)))
            .WithArgumentList(ForwardAllArguments(methodDeclaration));

    private static ArgumentListSyntax ForwardAllArguments(MethodDeclarationSyntax methodDeclaration)
        => ArgumentList(SeparatedList<ArgumentSyntax>(MethodGroupAndAllArguments(methodDeclaration)));

    private static SyntaxNodeOrToken[] MethodGroupAndAllArguments(MethodDeclarationSyntax methodDeclarationSyntax)
    {
        var result = new List<SyntaxNodeOrToken>();

        foreach (var parameter in methodDeclarationSyntax.ParameterList.Parameters)
        {
            result.Add(Argument(IdentifierName(parameter.Identifier.Text)));
            result.Add(Token(SyntaxKind.CommaToken));
        }

        result.Add(OutParameterArgument());

        return result.ToArray();
    }

    private static ArgumentSyntax OutParameterArgument()
        => Argument(DeclarationExpression(InlineVarDeclaration(), SingleVariableDesignation(OutParameter)))
            .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword));

    private static IdentifierNameSyntax InlineVarDeclaration()
        => IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList()));
}
