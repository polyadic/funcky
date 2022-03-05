using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Funcky.BuiltinAnalyzers;

internal static class MethodSymbolExtensions
{
    public static bool IsIteratorMethod(this ISymbol symbol)
        => symbol is IMethodSymbol methodSymbol && methodSymbol.IsIterator();

    // Source: https://github.com/dotnet/roslyn/blob/1a20545501d7a3749c2b93c8dced687617df50ab/src/Features/CSharp/Portable/MakeMethodAsynchronous/CSharpMakeMethodAsynchronousCodeFixProvider.cs#L136
    public static bool IsIterator(this IMethodSymbol methodSymbol)
    {
        return methodSymbol.Locations.Any(l => ContainsYield(GetSourceTreeOrThrow(l).GetRoot().FindNode(l.SourceSpan)));

        bool ContainsYield(SyntaxNode node)
            => node.DescendantNodes(descendIntoChildren: n => n == node || !IsReturnableConstruct(n)).Any(IsYield);

        static bool IsYield(SyntaxNode node)
            => node.IsKind(SyntaxKind.YieldBreakStatement) || node.IsKind(SyntaxKind.YieldReturnStatement);
    }

    private static SyntaxTree GetSourceTreeOrThrow(Location location)
        => location.SourceTree ?? throw new InvalidOperationException($"Location {location} unexpectedly had no source tree");

    private static bool IsReturnableConstruct(SyntaxNode node)
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
}
