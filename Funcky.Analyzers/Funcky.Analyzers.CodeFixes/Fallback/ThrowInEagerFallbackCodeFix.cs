using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.Fallback;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThrowInEagerFallbackCodeFix))]
public sealed class ThrowInEagerFallbackCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ThrowInEagerFallbackAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            // The diagnostic stays either way; the fix is only offered when the expression can legally move into a lambda.
            if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is ExpressionSyntax expression
                && diagnostic.TryGetIntProperty(ThrowInEagerFallbackAnalyzer.LambdaParameterCountProperty, out var parameterCount)
                && LambdaWrapping.CanWrapInLambda(semanticModel, expression, context.CancellationToken))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Pass a function instead",
                        cancellationToken => LambdaWrapping.WrapInLambdaAsync(context.Document, expression, parameterCount, cancellationToken),
                        nameof(ThrowInEagerFallbackCodeFix)),
                    diagnostic);
            }
        }
    }
}
