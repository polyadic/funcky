using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Funcky.Analyzers.Fallback;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TrivialLazyFallbackCodeFix))]
public sealed class TrivialLazyFallbackCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(TrivialLazyFallbackAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LambdaExpressionSyntax lambda
                && ReturnedExpression(lambda) is { } returnedExpression)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Pass the value directly",
                        cancellationToken => UnwrapLambdaAsync(context.Document, lambda, returnedExpression, cancellationToken),
                        nameof(TrivialLazyFallbackCodeFix)),
                    diagnostic);
            }
        }
    }

    private static ExpressionSyntax? ReturnedExpression(LambdaExpressionSyntax lambda)
        => lambda switch
        {
            { ExpressionBody: { } expressionBody } => expressionBody,
            { Block.Statements: [ReturnStatementSyntax { Expression: { } expression }] } => expression,
            _ => null,
        };

    private static async Task<Document> UnwrapLambdaAsync(Document document, LambdaExpressionSyntax lambda, ExpressionSyntax returnedExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(lambda, returnedExpression.WithoutTrivia().WithTriviaFrom(lambda));
        return editor.GetChangedDocument();
    }
}
