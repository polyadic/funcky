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
            ? root.ReplaceNode(namespaceDeclaration, Insert(namespaceDeclaration, ParseUsingDirective($"using {namespaceName};")))
            : Insert((CompilationUnitSyntax)root, ParseUsingDirective($"using {namespaceName};"));

    /// <summary>Inserts the directive after the last using that is not inside a <c>#if</c> region, so that it is not swallowed by the
    /// condition; when every using is conditional it goes in front of them all. It takes the indentation and line ending of its
    /// neighbour, but none of its comments or directives: copying a leading <c>#if</c> would open a second region without an <c>#endif</c>.</summary>
    public static TContainer Insert<TContainer>(TContainer container, UsingDirectiveSyntax usingDirective)
        where TContainer : SyntaxNode
    {
        var usings = container switch
        {
            CompilationUnitSyntax compilationUnit => compilationUnit.Usings,
            BaseNamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Usings,
            _ => default,
        };
        var endOfLine = EndOfLine(container);

        return (usings.LastOrDefault(existing => !IsInsideConditionalRegion(existing)), usings.FirstOrDefault()) switch
        {
            ({ } anchor, _) => container.InsertNodesAfter(anchor, [usingDirective.WithLeadingTrivia(FormattingOnly(anchor.GetLeadingTrivia())).WithTrailingTrivia(FormattingOnly(anchor.GetTrailingTrivia()))]),
            (null, { } first) => container.InsertNodesBefore(first, [usingDirective.WithLeadingTrivia(FormattingOnly(first.GetLeadingTrivia())).WithTrailingTrivia(endOfLine)]),
            (null, null) => container switch
            {
                CompilationUnitSyntax compilationUnit => (TContainer)(SyntaxNode)WithFirstUsing(compilationUnit, usingDirective.WithTrailingTrivia(endOfLine, endOfLine)),
                BaseNamespaceDeclarationSyntax namespaceDeclaration => (TContainer)(SyntaxNode)namespaceDeclaration.AddUsings(usingDirective.WithTrailingTrivia(endOfLine, endOfLine)),
                _ => container,
            },
        };
    }

    /// <summary>The directive is parsed rather than built from tokens so that it carries no elastic trivia, which the formatter would otherwise
    /// normalize together with the surrounding line endings.</summary>
    public static UsingDirectiveSyntax ParseUsingDirective(string directive)
        => ParseCompilationUnit(directive).Usings.Single();

    // `#define` and `#undef` must precede every token, the using included. In a file without usings they lead the first
    // member, so they (and whatever stands before them) move in front of the new using; an `#if` or a comment stays where it is.
    private static CompilationUnitSyntax WithFirstUsing(CompilationUnitSyntax compilationUnit, UsingDirectiveSyntax usingDirective)
    {
        var firstToken = compilationUnit.GetFirstToken(includeZeroWidth: true);
        var leadingTrivia = firstToken.LeadingTrivia;
        var headerLength = leadingTrivia.Select((trivia, index) => (Trivia: trivia, Index: index)).Where(pair => pair.Trivia.IsKind(SyntaxKind.DefineDirectiveTrivia) || pair.Trivia.IsKind(SyntaxKind.UndefDirectiveTrivia)).Select(pair => pair.Index + 1).LastOrDefault();

        return headerLength == 0
            ? compilationUnit.AddUsings(usingDirective)
            : compilationUnit
                .ReplaceToken(firstToken, firstToken.WithLeadingTrivia(leadingTrivia.Skip(headerLength)))
                .AddUsings(usingDirective.WithLeadingTrivia(leadingTrivia.Take(headerLength)));
    }

    // Counting the directives before the using: more `#if` than `#endif` means it sits inside a region.
    private static bool IsInsideConditionalRegion(UsingDirectiveSyntax usingDirective)
        => (usingDirective.Ancestors().LastOrDefault() ?? usingDirective)
            .DescendantNodes(descendIntoTrivia: true)
            .OfType<DirectiveTriviaSyntax>()
            .Where(directive => directive.SpanStart < usingDirective.SpanStart)
            .Sum(directive => directive switch
            {
                IfDirectiveTriviaSyntax => 1,
                EndIfDirectiveTriviaSyntax => -1,
                _ => 0,
            }) > 0;

    private static SyntaxTriviaList FormattingOnly(SyntaxTriviaList trivia)
        => TriviaList(trivia.Where(t => t.IsKind(SyntaxKind.WhitespaceTrivia) || t.IsKind(SyntaxKind.EndOfLineTrivia)));

    /// <summary>The line ending the file uses, so that the added directive does not introduce a mixed one.</summary>
    private static SyntaxTrivia EndOfLine(SyntaxNode node)
        => (node.Ancestors().LastOrDefault() ?? node).DescendantTrivia().FirstOrDefault(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia)) is { RawKind: not 0 } endOfLine
            ? endOfLine
            : CarriageReturnLineFeed;
}
