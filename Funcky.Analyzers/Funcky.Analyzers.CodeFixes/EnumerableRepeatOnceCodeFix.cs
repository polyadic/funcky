using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.CodeFixResources;
using static Funcky.Analyzers.EnumerableRepeatOnceAnalyzer;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumerableRepeatOnceCodeFix))]
public sealed class EnumerableRepeatOnceCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = GetDiagnostic(context);
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First() is { } declaration
            && diagnostic.TryGetIntProperty(ValueParameterIndexProperty, out var valueParameterIndex))
        {
            context.RegisterCodeFix(new ToEnumerableReturnCodeAction(context.Document, declaration, valueParameterIndex), diagnostic);
        }
    }

    private static Diagnostic GetDiagnostic(CodeFixContext context)
        => context.Diagnostics.First();

    private sealed class ToEnumerableReturnCodeAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocationExpression;
        private readonly int _valueParameterIndex;

        public ToEnumerableReturnCodeAction(Document document, InvocationExpressionSyntax invocationExpression, int valueParameterIndex)
        {
            _document = document;
            _invocationExpression = invocationExpression;
            _valueParameterIndex = valueParameterIndex;
        }

        public override string Title => EnumerableRepeatOnceCodeFixTitle;

        public override string EquivalenceKey => nameof(ToEnumerableReturnCodeAction);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);
            var valueArgument = _invocationExpression.ArgumentList.Arguments[_valueParameterIndex];
            editor.ReplaceNode(_invocationExpression, CreateEnumerableReturnRoot(valueArgument, editor.SemanticModel, editor.Generator));
            return editor.GetChangedDocument();
        }

        private static SyntaxNode CreateEnumerableReturnRoot(ArgumentSyntax firstArgument, SemanticModel model, SyntaxGenerator generator)
            => SyntaxEnumerableReturn(model, generator)
                .WithArgumentList(ArgumentList(SingletonSeparatedList(firstArgument.WithNameColon(null)))
                    .WithCloseParenToken(Token(SyntaxKind.CloseParenToken)))
                .NormalizeWhitespace();

        // Enumerable.Return is Funcky's static extension member on System.Linq.Enumerable, so the receiver is the BCL type.
        private static InvocationExpressionSyntax SyntaxEnumerableReturn(SemanticModel model, SyntaxGenerator generator)
            => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    (ExpressionSyntax)generator.TypeExpressionForStaticMemberAccess(model.Compilation.GetEnumerableType()!),
                    IdentifierName(MonadReturnMethodName))
                    .WithAdditionalAnnotations(Simplifier.Annotation));
    }
}
