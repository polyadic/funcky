using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

internal static class UsingDirectives
{
    /// <summary>Adds <c>using &lt;namespace&gt;;</c> next to the existing usings: into the namespace declaration enclosing
    /// <paramref name="anchor"/> when the usings are there, otherwise at the top of the file. The <paramref name="anchor"/>
    /// must be a node of <paramref name="root"/>.</summary>
    public static SyntaxNode AddUsing(SyntaxNode root, SyntaxNode anchor, string namespaceName)
        => anchor.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(namespaceDeclaration => namespaceDeclaration.Usings.Count > 0) is { } namespaceDeclaration
            ? root.ReplaceNode(namespaceDeclaration, namespaceDeclaration.AddUsings(UsingDirective(root, namespaceDeclaration.Usings, namespaceName)))
            : ((CompilationUnitSyntax)root).AddUsings(UsingDirective(root, ((CompilationUnitSyntax)root).Usings, namespaceName));

    /// <summary>The directive is parsed rather than built from tokens so that it carries no elastic trivia, which the formatter would otherwise
    /// normalize together with the surrounding line endings. It takes its indentation and line ending from the last existing using.</summary>
    private static UsingDirectiveSyntax UsingDirective(SyntaxNode root, SyntaxList<UsingDirectiveSyntax> usings, string namespaceName)
    {
        var usingDirective = ParseCompilationUnit($"using {namespaceName};").Usings.Single();
        return usings.LastOrDefault() is { } lastUsing
            ? usingDirective.WithLeadingTrivia(lastUsing.GetLeadingTrivia()).WithTrailingTrivia(lastUsing.GetTrailingTrivia())
            : usingDirective.WithTrailingTrivia(EndOfLine(root), EndOfLine(root));
    }

    /// <summary>The line ending the file uses, so that the added directive does not introduce a mixed one.</summary>
    private static SyntaxTrivia EndOfLine(SyntaxNode root)
        => root.DescendantTrivia().FirstOrDefault(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia)) is { RawKind: not 0 } endOfLine
            ? endOfLine
            : CarriageReturnLineFeed;
}
