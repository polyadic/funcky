using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Funcky.Analyzers.AlternativeMonadAnalyzer;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.AlternativeMonad;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MatchToOrElseCodeFix))]
public sealed class MatchToOrElseCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferGetOrElse.Id, PreferOrElse.Id, PreferSelectMany.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (syntaxRoot?.FindInvocationExpression(context.Span) is { Expression: MemberAccessExpressionSyntax memberAccessExpression } invocation
                && diagnostic.TryGetIntProperty(PreservedArgumentIndexProperty, out var noneArgumentIndex))
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

    private sealed class GetOrElseCodeFixAction(
        Document document,
        InvocationExpressionSyntax invocationExpression,
        MemberAccessExpressionSyntax memberAccessExpression,
        int errorStateArgumentIndex,
        IdentifierNameSyntax methodName) : CodeAction
    {
        public override string Title => $"Replace {MatchMethodName} with {methodName.Identifier}";

        public override string? EquivalenceKey => nameof(MatchToOrElseCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            editor.ReplaceNode(
                invocationExpression,
                invocationExpression.WithExpression(memberAccessExpression
                    .WithName(methodName))
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(invocationExpression.ArgumentList.Arguments[errorStateArgumentIndex].Expression)))));

            return editor.GetChangedDocument();
        }
    }
}
