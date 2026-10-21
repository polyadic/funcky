using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.Flatten;

/// <summary>Replaces <c>SelectMany(x => x)</c> with <c>Flatten()</c>, adding the using for the extension's namespace when needed.
/// The fix is only offered when the rewritten call binds to Funcky's <c>Flatten</c>: a <c>Flatten</c> member of the receiver's own
/// type wins over any extension method and would silently redirect the call.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferFlattenCodeFix))]
public sealed class PreferFlattenCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferFlattenAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is not { } root
            || await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is not { } semanticModel)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess } invocation
                && diagnostic.Properties.TryGetValue(PreferFlattenAnalyzer.FlattenNamespaceProperty, out var flattenNamespace)
                && flattenNamespace is not null
                && await ReplaceWithFlattenAsync(context.Document, root, semanticModel, invocation, memberAccess, flattenNamespace, context.CancellationToken).ConfigureAwait(false) is { } fixedDocument)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Replace with '{PreferFlattenAnalyzer.FlattenMethodName}()'",
                        _ => Task.FromResult(fixedDocument),
                        nameof(PreferFlattenCodeFix)),
                    diagnostic);
            }
        }
    }

    /// <summary>The call is tried as is and, when that does not reach Funcky's <c>Flatten</c>, once more with the using added. The
    /// using alone does not guarantee the binding (an instance member named <c>Flatten</c> still wins), so the result is checked in the
    /// rewritten document; without a successful binding there is no fix.</summary>
    private static async Task<Document?> ReplaceWithFlattenAsync(Document document, SyntaxNode root, SemanticModel semanticModel, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess, string flattenNamespace, CancellationToken cancellationToken)
    {
        var annotation = new SyntaxAnnotation();
        var flatten = invocation
            .WithExpression(memberAccess.WithName(FlattenName(memberAccess.Name)))
            .WithArgumentList(ArgumentList().WithTriviaFrom(invocation.ArgumentList))
            .WithAdditionalAnnotations(annotation);

        if (IsFunckyFlatten(semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, flatten, SpeculativeBindingOption.BindAsExpression).Symbol, flattenNamespace))
        {
            return document.WithSyntaxRoot(root.ReplaceNode(invocation, flatten));
        }

        var rootWithReplacement = root.ReplaceNode(invocation, flatten);
        var fixedDocument = document.WithSyntaxRoot(UsingDirectives.AddUsing(rootWithReplacement, rootWithReplacement.GetAnnotatedNodes(annotation).Single(), flattenNamespace));

        if (await fixedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is { } fixedRoot
            && await fixedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is { } fixedSemanticModel
            && IsFunckyFlatten(fixedSemanticModel.GetSymbolInfo(fixedRoot.GetAnnotatedNodes(annotation).Single(), cancellationToken).Symbol, flattenNamespace))
        {
            return fixedDocument;
        }

        return null;
    }

    /// <summary>Explicit type arguments select the result's element type (<c>SelectMany&lt;IEnumerable&lt;string&gt;, object&gt;(x => x)</c>
    /// yields <c>IEnumerable&lt;object&gt;</c>), so the last one is carried over: <c>Flatten&lt;object&gt;()</c>. Where <c>Flatten</c> has
    /// another arity the call does not bind and there is no fix.</summary>
    private static SimpleNameSyntax FlattenName(SimpleNameSyntax selectManyName)
        => selectManyName is GenericNameSyntax { TypeArgumentList.Arguments: [.., var resultType] }
            ? GenericName(Identifier(PreferFlattenAnalyzer.FlattenMethodName), TypeArgumentList(SingletonSeparatedList(resultType.WithoutTrivia()))).WithTriviaFrom(selectManyName)
            : IdentifierName(PreferFlattenAnalyzer.FlattenMethodName).WithTriviaFrom(selectManyName);

    private static bool IsFunckyFlatten(ISymbol? symbol, string flattenNamespace)
        => symbol is IMethodSymbol { Name: PreferFlattenAnalyzer.FlattenMethodName, ReducedFrom: not null, ContainingNamespace: var containingNamespace }
           && containingNamespace.ToDisplayString() == flattenNamespace;
}
