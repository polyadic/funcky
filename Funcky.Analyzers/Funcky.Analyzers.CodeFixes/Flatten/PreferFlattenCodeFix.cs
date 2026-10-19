using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.Flatten;

/// <summary>Replaces <c>SelectMany(x => x)</c> with <c>Flatten()</c>, adding the using for the extension's namespace when needed.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferFlattenCodeFix))]
public sealed class PreferFlattenCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferFlattenAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess } invocation
                && diagnostic.Properties.TryGetValue(PreferFlattenAnalyzer.FlattenNamespaceProperty, out var flattenNamespace)
                && flattenNamespace is not null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Replace with '{PreferFlattenAnalyzer.FlattenMethodName}()'",
                        cancellationToken => ReplaceWithFlattenAsync(context.Document, invocation, memberAccess, flattenNamespace, cancellationToken),
                        nameof(PreferFlattenCodeFix)),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> ReplaceWithFlattenAsync(Document document, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess, string flattenNamespace, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root
            || await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } semanticModel)
        {
            return document;
        }

        var flatten = invocation
            .WithExpression(memberAccess.WithName(IdentifierName(PreferFlattenAnalyzer.FlattenMethodName).WithTriviaFrom(memberAccess.Name)))
            .WithArgumentList(ArgumentList().WithTriviaFrom(invocation.ArgumentList));
        var newRoot = root.ReplaceNode(invocation, flatten);

        return document.WithSyntaxRoot(IsImported(semanticModel, invocation, flatten) ? newRoot : UsingDirectives.AddUsing(newRoot, newRoot.FindNode(invocation.Span), flattenNamespace));
    }

    private static bool IsImported(SemanticModel semanticModel, InvocationExpressionSyntax original, InvocationExpressionSyntax flatten)
        => semanticModel.GetSpeculativeSymbolInfo(original.SpanStart, flatten, SpeculativeBindingOption.BindAsExpression) is { Symbol: IMethodSymbol } or { CandidateSymbols.IsEmpty: false };
}
