using System.Collections.Immutable;
using System.Globalization;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

/// <summary>Replaces expressions with ones that call a static extension member of a class in <c>Funcky.Extensions</c>, and
/// adds the <c>using</c> for that namespace when a replacement does not bind to such a member without it.</summary>
internal static class FunckyExtensionsImport
{
    private const string ReplacementAnnotationKind = "Funcky.Analyzers.FunckyExtensionsImport.Replacement";

    public static Document ReplaceWithExtensionMemberCall(Document document, SyntaxNode root, SemanticModel semanticModel, ExpressionSyntax original, ExpressionSyntax replacement)
        => ReplaceWithExtensionMemberCalls(document, root, semanticModel, ImmutableArray.Create(new ExtensionMemberRewrite(original, replacement)), ExtensionMemberSupport.IsDeclaredInFunckyExtensions);

    /// <summary>Applies all rewrites to the document at once. The <c>using</c> is added once per group of usings (the enclosing
    /// namespace declaration or the file) that has a replacement in need of it, so that any number of rewrites work in one pass.
    /// A rewrite nested in another one is skipped, the outer replacement is built from the original and would discard it anyway;
    /// its diagnostic stays and is fixed by a subsequent pass. <paramref name="bindsToExtensionMember"/> tells whether the symbol a
    /// replacement binds to without the <c>using</c> is the intended extension member.</summary>
    public static Document ReplaceWithExtensionMemberCalls(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, Func<ISymbol?, bool> bindsToExtensionMember)
        => document.WithSyntaxRoot(WithoutReplacementAnnotations(ReplaceWithAnnotatedExtensionMemberCalls(root, semanticModel, rewrites, bindsToExtensionMember)));

    /// <summary>Like <see cref="ReplaceWithExtensionMemberCalls"/>, but the replacements in the returned root still carry the annotations
    /// that <see cref="Replacements"/> finds, so that a caller can bind them in the changed document before it strips them with
    /// <see cref="WithoutReplacementAnnotations"/>. A document handed to the host must not keep them: a later fix on the same document
    /// would take them for its own.</summary>
    public static SyntaxNode ReplaceWithAnnotatedExtensionMemberCalls(SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, Func<ISymbol?, bool> bindsToExtensionMember)
    {
        var originals = rewrites.Select(rewrite => rewrite.Original).ToImmutableHashSet();
        var outermostRewrites = rewrites
            .Select((rewrite, index) => (Rewrite: rewrite, Index: index))
            .Where(pair => !pair.Rewrite.Original.Ancestors().Any(originals.Contains))
            .GroupBy(pair => pair.Rewrite.Original)
            .ToDictionary(group => group.Key, group => group.First());
        var rootWithReplacements = root.ReplaceNodes(outermostRewrites.Keys, (original, _) => outermostRewrites[original].Rewrite.Replacement.WithAdditionalAnnotations(Annotation(outermostRewrites[original].Index)));
        var replacementsInNeedOfImport = outermostRewrites.Values
            .Where(pair => !bindsToExtensionMember(semanticModel.GetSpeculativeSymbolInfo(pair.Rewrite.Original.SpanStart, pair.Rewrite.Replacement, SpeculativeBindingOption.BindAsExpression).Symbol))
            .Select(pair => pair.Index)
            .ToImmutableArray();

        return AddUsings(rootWithReplacements, replacementsInNeedOfImport);
    }

    /// <summary>The replacements in a root produced by <see cref="ReplaceWithAnnotatedExtensionMemberCalls"/>, each with the index of the rewrite it stems from.</summary>
    public static IEnumerable<(int Index, SyntaxNode Replacement)> Replacements(SyntaxNode root)
        => root.GetAnnotatedNodes(ReplacementAnnotationKind).Select(node => (Index(node), node));

    public static SyntaxNode WithoutReplacementAnnotations(SyntaxNode root)
        => root.ReplaceNodes(root.GetAnnotatedNodes(ReplacementAnnotationKind), (_, replacement) => replacement.WithoutAnnotations(ReplacementAnnotationKind));

    private static SyntaxNode AddUsings(SyntaxNode root, ImmutableArray<int> replacementsInNeedOfImport)
    {
        var anchors = Replacements(root)
            .Where(replacement => replacementsInNeedOfImport.Contains(replacement.Index))
            .GroupBy(replacement => UsingContainer(replacement.Replacement)?.Span ?? default)
            .Select(group => group.First().Index)
            .ToImmutableArray();

        return anchors.Aggregate(root, (current, anchor) => UsingDirectives.AddUsing(current, Replacements(current).Single(replacement => replacement.Index == anchor).Replacement, ExtensionMemberSupport.ExtensionsNamespace));
    }

    // Where UsingDirectives.AddUsing puts the directive: the enclosing namespace declaration with usings, or the file.
    private static BaseNamespaceDeclarationSyntax? UsingContainer(SyntaxNode node)
        => node.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(namespaceDeclaration => namespaceDeclaration.Usings.Count > 0);

    private static SyntaxAnnotation Annotation(int index)
        => new(ReplacementAnnotationKind, index.ToString(CultureInfo.InvariantCulture));

    private static int Index(SyntaxNode replacement)
        => int.Parse(replacement.GetAnnotations(ReplacementAnnotationKind).Single().Data!, CultureInfo.InvariantCulture);
}
