using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferXunitAssertCodeFix))]
public sealed class PreferXunitAssertCodeFix : CodeFixProvider
{
    private const string AssertTypeName = "global::Xunit.Assert";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferXunitAssertAnalyzer.PreferAssert.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
                && invocation.Span == diagnostic.Location.SourceSpan
                && WithAssertReceiver(invocation) is { } replacement)
            {
                context.RegisterCodeFix(new UseAssertCodeFixAction(context.Document, invocation, replacement), diagnostic);
            }
        }
    }

    // `FunctionalAssert.Some(x)` and `Funcky.FunctionalAssert.Some(x)` keep their name (and type arguments); `Some(x)` through
    // a `using static` gets the Assert receiver in front.
    private static InvocationExpressionSyntax? WithAssertReceiver(InvocationExpressionSyntax invocation)
        => invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => invocation.WithExpression(memberAccess.WithExpression(Assert().WithTriviaFrom(memberAccess.Expression))),
            SimpleNameSyntax name => invocation.WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Assert().WithLeadingTrivia(name.GetLeadingTrivia()), name.WithoutLeadingTrivia())),
            _ => null,
        };

    private static ExpressionSyntax Assert()
        => ParseExpression(AssertTypeName).WithAdditionalAnnotations(Simplifier.Annotation);

    private sealed class UseAssertCodeFixAction(Document document, InvocationExpressionSyntax invocation, InvocationExpressionSyntax replacement) : CodeAction
    {
        public override string Title => "Replace with the assertion on Assert";

        public override string EquivalenceKey => nameof(PreferXunitAssertCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            return root is null || semanticModel is null
                ? document
                : FunckyExtensionsImport.ReplaceWithExtensionMemberCall(document, root, semanticModel, invocation, replacement);
        }
    }
}
