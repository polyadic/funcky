using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferStaticParseOrNoneCodeFix))]
public sealed class PreferStaticParseOrNoneCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferStaticParseOrNoneAnalyzer.PreferStaticParseOrNone.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
                && invocation.Span == diagnostic.Location.SourceSpan
                && diagnostic.Properties.TryGetValue(PreferStaticParseOrNoneAnalyzer.TargetTypeProperty, out var targetType)
                && diagnostic.Properties.TryGetValue(PreferStaticParseOrNoneAnalyzer.StaticMemberNameProperty, out var staticMemberName)
                && targetType is not null
                && staticMemberName is not null
                && semanticModel?.GetOperation(invocation, context.CancellationToken) is IInvocationOperation operation
                && PreferStaticParseOrNoneAnalyzer.StaticInvocation(operation, targetType, staticMemberName) is { } replacement)
            {
                context.RegisterCodeFix(new UseStaticMemberCodeFixAction(context.Document, invocation, replacement, targetType, staticMemberName), diagnostic);
            }
        }
    }

    private sealed class UseStaticMemberCodeFixAction(Document document, InvocationExpressionSyntax invocation, InvocationExpressionSyntax replacement, string targetType, string staticMemberName) : CodeAction
    {
        public override string Title => $"Replace with {targetType.Replace("global::", string.Empty)}.{staticMemberName}";

        public override string EquivalenceKey => nameof(PreferStaticParseOrNoneCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            return root is null || semanticModel is null
                ? document
                : FunckyExtensionsImport.ReplaceWithExtensionMemberCall(
                    document,
                    root,
                    semanticModel,
                    invocation,
                    replacement.WithExpression(replacement.Expression.WithAdditionalAnnotations(Simplifier.Annotation)).WithTriviaFrom(invocation));
        }
    }
}
