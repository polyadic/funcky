using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.OptionMatchAnalyzer;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionMatchToToNullableCodeFix))]
public sealed class OptionMatchToToNullableCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferToNullable.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (syntaxRoot?.FindInvocationExpression(context.Span) is { Expression: MemberAccessExpressionSyntax memberAccessExpression } invocation)
            {
                context.RegisterCodeFix(new ToNullableCodeFixAction(context.Document, invocation, memberAccessExpression), diagnostic);
            }
        }
    }

    private sealed class ToNullableCodeFixAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocationExpression;
        private readonly MemberAccessExpressionSyntax _memberAccessExpression;

        public ToNullableCodeFixAction(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            MemberAccessExpressionSyntax memberAccessExpression)
        {
            _document = document;
            _invocationExpression = invocationExpression;
            _memberAccessExpression = memberAccessExpression;
        }

        public override string Title => $"Replace {MatchMethodName} with {OptionToNullableMethodName}";

        public override string EquivalenceKey => nameof(ToNullableCodeFixAction);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);

            editor.ReplaceNode(
                _invocationExpression,
                _invocationExpression.WithExpression(_memberAccessExpression
                    .WithName(IdentifierName(OptionToNullableMethodName)))
                    .WithArgumentList(ArgumentList()));

            return editor.GetChangedDocument();
        }
    }
}
