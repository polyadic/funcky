using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Funcky.Analyzers.CodeFixResources;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SequenceReturnCollectionExpressionCodeFix))]
public sealed class SequenceReturnCollectionExpressionCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(SequenceReturnCollectionExpressionAnalyzer.UseCollectionExpression.Id);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is { } invocation
            && invocation.ArgumentList.Arguments is [{ } argument])
        {
            context.RegisterCodeFix(new UseCollectionExpressionCodeAction(context.Document, invocation, argument), diagnostic);
        }
    }

    private sealed class UseCollectionExpressionCodeAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocation;
        private readonly ArgumentSyntax _argument;

        public UseCollectionExpressionCodeAction(Document document, InvocationExpressionSyntax invocation, ArgumentSyntax argument)
        {
            _document = document;
            _invocation = invocation;
            _argument = argument;
        }

        public override string Title => SequenceReturnCollectionExpressionCodeFixTitle;

        public override string EquivalenceKey => nameof(UseCollectionExpressionCodeAction);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await _document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            return _document.WithSyntaxRoot(root!.ReplaceNode(_invocation, CreateCollectionExpression()));
        }

        // Comments inside the parentheses hang off the parentheses and the element, so they are carried over into the
        // brackets. A line comment keeps its line break, which stops it from swallowing the closing bracket.
        private SyntaxNode CreateCollectionExpression()
            => SingleElementCollectionExpression.Create(
                    _argument.Expression,
                    leadingTrivia: $"{_invocation.ArgumentList.OpenParenToken.TrailingTrivia}{_argument.Expression.GetLeadingTrivia()}",
                    trailingTrivia: $"{_argument.Expression.GetTrailingTrivia()}{_invocation.ArgumentList.CloseParenToken.LeadingTrivia}")
                .WithTriviaFrom(_invocation)
                .WithAdditionalAnnotations(Formatter.Annotation);
    }
}
