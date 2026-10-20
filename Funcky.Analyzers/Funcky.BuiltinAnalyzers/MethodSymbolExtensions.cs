using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Funcky.BuiltinAnalyzers;

internal static class MethodSymbolExtensions
{
    public static bool IsWithinIterator(this SyntaxNode node)
        => node.Ancestors().FirstOrDefault(IsReturnableConstruct) is { } returnableConstruct
            && returnableConstruct.IsIterator();

    // Source: https://github.com/dotnet/roslyn/blob/1a20545501d7a3749c2b93c8dced687617df50ab/src/Features/CSharp/Portable/MakeMethodAsynchronous/CSharpMakeMethodAsynchronousCodeFixProvider.cs#L136
    public static bool ContainsYield(this SyntaxNode node)
    {
        static bool IsYield(SyntaxNode node)
            => node.IsKind(SyntaxKind.YieldBreakStatement) || node.IsKind(SyntaxKind.YieldReturnStatement);

        return node.DescendantNodes(descendIntoChildren: n => n == node || !IsReturnableConstruct(n)).Any(IsYield);
    }

    /// <summary>Returns the node and its ancestors up to, but excluding, the enclosing method, lambda, local function or accessor.</summary>
    public static IEnumerable<SyntaxNode> AncestorsAndSelfWithinReturnableConstruct(this SyntaxNode node)
        => node.AncestorsAndSelf().TakeWhile(n => !IsReturnableConstruct(n));

    public static bool IsReturnableConstruct(this SyntaxNode node)
        => node.Kind()
            is SyntaxKind.AnonymousMethodExpression
            or SyntaxKind.SimpleLambdaExpression
            or SyntaxKind.ParenthesizedLambdaExpression
            or SyntaxKind.LocalFunctionStatement
            or SyntaxKind.MethodDeclaration
            or SyntaxKind.ConstructorDeclaration
            or SyntaxKind.DestructorDeclaration
            or SyntaxKind.GetAccessorDeclaration
            or SyntaxKind.SetAccessorDeclaration
            or SyntaxKind.InitAccessorDeclaration
            or SyntaxKind.OperatorDeclaration
            or SyntaxKind.ConversionOperatorDeclaration
            or SyntaxKind.AddAccessorDeclaration
            or SyntaxKind.RemoveAccessorDeclaration;

    private static bool IsIterator(this SyntaxNode node) => ContainsYield(node);
}
