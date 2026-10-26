using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.CodeAnalysisExtensions;

/// <summary>Replaces an expression with one that calls a static extension member of a class in <c>Funcky.Extensions</c>, and
/// adds the <c>using</c> for that namespace when the replacement does not bind to such a member without it.</summary>
internal static class FunckyExtensionsImport
{
    private static readonly SyntaxAnnotation ReplacementAnnotation = new();

    public static Document ReplaceWithExtensionMemberCall(Document document, SyntaxNode root, SemanticModel semanticModel, ExpressionSyntax original, ExpressionSyntax replacement)
    {
        var annotatedReplacement = replacement.WithAdditionalAnnotations(ReplacementAnnotation);
        var rootWithReplacement = root.ReplaceNode(original, annotatedReplacement);

        return ExtensionMemberSupport.IsDeclaredInFunckyExtensions(semanticModel.GetSpeculativeSymbolInfo(original.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol)
            ? document.WithSyntaxRoot(rootWithReplacement)
            : document.WithSyntaxRoot(UsingDirectives.AddUsing(rootWithReplacement, rootWithReplacement.GetAnnotatedNodes(ReplacementAnnotation).Single(), ExtensionMemberSupport.ExtensionsNamespace));
    }
}
