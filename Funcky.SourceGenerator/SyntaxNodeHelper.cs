using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.SourceGenerator;

internal static class SyntaxNodeHelper
{
    public static TSyntaxNode? TryGetParentSyntax<TSyntaxNode>(this SyntaxNode syntaxNode)
        where TSyntaxNode : SyntaxNode
        => syntaxNode.Parent switch
        {
            null => null,
            TSyntaxNode parent => parent,
            var parent => parent.TryGetParentSyntax<TSyntaxNode>(),
        };

    public static AttributeSyntax GetAttributeByUsedName(this MemberDeclarationSyntax methodDeclaration, string usedName)
        => methodDeclaration
            .AttributeLists
            .SelectMany(AllAttributes)
            .First(AttributeHasName(usedName));

    private static IEnumerable<AttributeSyntax> AllAttributes(AttributeListSyntax list)
        => list.Attributes;

    private static Func<AttributeSyntax, bool> AttributeHasName(string usedName)
        => attribute => attribute.Name is IdentifierNameSyntax id && id.Identifier.Text == usedName;
}
