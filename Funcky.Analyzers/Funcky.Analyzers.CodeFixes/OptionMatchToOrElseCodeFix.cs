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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionMatchToOrElseCodeFix))]
public sealed class OptionMatchToOrElseCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferGetOrElse.Id, PreferOrElse.Id, PreferSelectMany.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (syntaxRoot?.FindInvocationExpression(context.Span) is { Expression: MemberAccessExpressionSyntax memberAccessExpression } invocation
                && diagnostic.Properties.TryGetValue(PreservedArgumentIndexProperty, out var noneArgumentIndexString)
                && int.TryParse(noneArgumentIndexString, out var noneArgumentIndex))
            {
                context.RegisterCodeFix(new GetOrElseCodeFixAction(context.Document, invocation, memberAccessExpression, noneArgumentIndex, DiagnosticIdToMethodName(diagnostic.Id)), diagnostic);
            }
        }
    }

    private static IdentifierNameSyntax DiagnosticIdToMethodName(string diagnosticId)
        => diagnosticId switch
        {
            _ when diagnosticId == PreferGetOrElse.Id => IdentifierName(GetOrElseMethodName),
            _ when diagnosticId == PreferOrElse.Id => IdentifierName(OrElseMethodName),
            _ when diagnosticId == PreferSelectMany.Id => IdentifierName(SelectManyMethodName),
            _ => throw new NotSupportedException("Internal error: This branch should be unreachable"),
        };

    private sealed class GetOrElseCodeFixAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocationExpression;
        private readonly MemberAccessExpressionSyntax _memberAccessExpression;
        private readonly int _noneArgumentIndex;
        private readonly IdentifierNameSyntax _methodName;

        public GetOrElseCodeFixAction(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            MemberAccessExpressionSyntax memberAccessExpression,
            int noneArgumentIndex,
            IdentifierNameSyntax methodName)
        {
            _document = document;
            _invocationExpression = invocationExpression;
            _memberAccessExpression = memberAccessExpression;
            _noneArgumentIndex = noneArgumentIndex;
            _methodName = methodName;
        }

        public override string Title => $"Replace {MatchMethodName} with {_methodName.Identifier}";

        public override string? EquivalenceKey => nameof(OptionMatchToOrElseCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);

            editor.ReplaceNode(
                _invocationExpression,
                _invocationExpression.WithExpression(_memberAccessExpression
                    .WithName(_methodName))
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(_invocationExpression.ArgumentList.Arguments[_noneArgumentIndex].Expression)))));

            return editor.GetChangedDocument();
        }
    }
}
