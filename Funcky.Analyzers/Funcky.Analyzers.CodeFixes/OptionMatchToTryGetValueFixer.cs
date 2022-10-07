using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionMatchToGetOrElseCodeFix))]
public sealed class OptionMatchToGetOrElseCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(OptionMatchAnalyzer.PreferGetOrElse.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (syntaxRoot?.FindNode(context.Span).FirstAncestorOrSelf<InvocationExpressionSyntax>() is { } invocation
                && invocation.Expression is MemberAccessExpressionSyntax memberAccessExpression
                && diagnostic.Properties.TryGetValue(OptionMatchAnalyzer.NoneArgumentIndexProperty, out var noneArgumentIndexString)
                && int.TryParse(noneArgumentIndexString, out var noneArgumentIndex))
            {
                context.RegisterCodeFix(new GetOrElseCodeFixAction(context.Document, invocation, memberAccessExpression, noneArgumentIndex), diagnostic);
            }
        }
    }

    private sealed class GetOrElseCodeFixAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocationExpression;
        private readonly MemberAccessExpressionSyntax _memberAccessExpression;
        private readonly int _noneArgumentIndex;

        public GetOrElseCodeFixAction(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            MemberAccessExpressionSyntax memberAccessExpression,
            int noneArgumentIndex)
        {
            _document = document;
            _invocationExpression = invocationExpression;
            _memberAccessExpression = memberAccessExpression;
            _noneArgumentIndex = noneArgumentIndex;
        }

        public override string Title => "Replace Match with GetOrElse";

        public override string? EquivalenceKey => nameof(OptionMatchToGetOrElseCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);

            editor.ReplaceNode(
                _invocationExpression,
                _invocationExpression.WithExpression(_memberAccessExpression
                    .WithName(IdentifierName("GetOrElse")))
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(_invocationExpression.ArgumentList.Arguments[_noneArgumentIndex].Expression)))));

            return editor.GetChangedDocument();
        }
    }
}
