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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferEnumerableSequenceConstructorsCodeFix))]
public sealed class PreferEnumerableSequenceConstructorsCodeFix : CodeFixProvider
{
    private const string EnumerableTypeName = "global::System.Linq.Enumerable";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferEnumerableSequenceConstructorsAnalyzer.PreferEnumerable.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
                && invocation.Span == diagnostic.Location.SourceSpan
                && WithEnumerableReceiver(invocation.Expression) is { } replacement)
            {
                context.RegisterCodeFix(new UseEnumerableCodeFixAction(context.Document, invocation, replacement), diagnostic);
            }
        }
    }

    // `Sequence.Return(x)` and `Funcky.Sequence.Return(x)` keep their name (and type arguments); `Return(x)` through a
    // `using static` gets the Enumerable receiver in front.
    private static InvocationExpressionSyntax? WithEnumerableReceiver(ExpressionSyntax invoked)
        => invoked switch
        {
            MemberAccessExpressionSyntax memberAccess => ((InvocationExpressionSyntax)invoked.Parent!).WithExpression(memberAccess.WithExpression(Enumerable().WithTriviaFrom(memberAccess.Expression))),
            SimpleNameSyntax name => ((InvocationExpressionSyntax)invoked.Parent!).WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Enumerable().WithLeadingTrivia(name.GetLeadingTrivia()), name.WithoutLeadingTrivia())),
            _ => null,
        };

    private static ExpressionSyntax Enumerable()
        => ParseExpression(EnumerableTypeName).WithAdditionalAnnotations(Simplifier.Annotation);

    private sealed class UseEnumerableCodeFixAction(Document document, InvocationExpressionSyntax invocation, InvocationExpressionSyntax replacement) : CodeAction
    {
        public override string Title => "Replace with the Enumerable form";

        public override string EquivalenceKey => nameof(PreferEnumerableSequenceConstructorsCodeFix);

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
