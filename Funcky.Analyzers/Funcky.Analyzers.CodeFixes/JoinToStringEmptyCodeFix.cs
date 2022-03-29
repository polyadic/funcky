using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.CodeFixResources;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JoinToStringEmptyCodeFix))]
public sealed class JoinToStringEmptyCodeFix : CodeFixProvider
{
    private const string ConcatToString = "ConcatToString";

    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(JoinToStringEmptyAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnosticSpan = context.Diagnostics.First().Location.SourceSpan;

        if (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First() is { } declaration)
        {
            context.RegisterCodeFix(CreateFix(context, declaration), GetDiagnostic(context));
        }
    }

    private static Diagnostic GetDiagnostic(CodeFixContext context)
        => context.Diagnostics.First();

    private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax declaration)
        => CodeAction.Create(
            JoinToStringEmptyCodeFixTitle,
            CreateConcatToStringAsync(context.Document, declaration),
            nameof(JoinToStringEmptyCodeFixTitle));

    private static Func<CancellationToken, Task<Document>> CreateConcatToStringAsync(Document document, InvocationExpressionSyntax declaration)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

                if (declaration.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    editor.ReplaceNode(declaration, CreateConcatToStringRoot(memberAccess.Expression));
                }

                return editor.GetChangedDocument();
            };

    private static SyntaxNode CreateConcatToStringRoot(ExpressionSyntax target)
        => SyntaxConcatToString(target)
            .NormalizeWhitespace();

    private static InvocationExpressionSyntax SyntaxConcatToString(ExpressionSyntax target)
        => InvocationExpression(
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, IdentifierName(ConcatToString))
                .WithAdditionalAnnotations(Simplifier.Annotation));
}
