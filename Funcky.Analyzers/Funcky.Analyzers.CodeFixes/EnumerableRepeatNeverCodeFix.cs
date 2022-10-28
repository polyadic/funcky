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
using static Funcky.Analyzers.EnumerableRepeatNeverAnalyzer;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumerableRepeatNeverCodeFix))]
public sealed class EnumerableRepeatNeverCodeFix : CodeFixProvider
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
            && diagnostic.Properties.TryGetValue(ValueParameterIndexProperty, out var valueParameterIndexProperty)
            && int.TryParse(valueParameterIndexProperty, out var valueParameterIndex))
        {
            context.RegisterCodeFix(new ToEnumerableEmptyCodeAction(context.Document, declaration, valueParameterIndex), diagnostic);
        }
    }

    private static Diagnostic GetDiagnostic(CodeFixContext context)
        => context.Diagnostics.First();

    private sealed class ToEnumerableEmptyCodeAction : CodeAction
    {
        private readonly Document _document;
        private readonly InvocationExpressionSyntax _invocationExpression;
        private readonly int _valueParameterIndex;

        public ToEnumerableEmptyCodeAction(Document document, InvocationExpressionSyntax invocationExpression, int valueParameterIndex)
        {
            _document = document;
            _invocationExpression = invocationExpression;
            _valueParameterIndex = valueParameterIndex;
        }

        public override string Title => EnumerableRepeatNeverCodeFixTitle;

        public override string EquivalenceKey => nameof(ToEnumerableEmptyCodeAction);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);
            var valueParameter = _invocationExpression.ArgumentList.Arguments[_valueParameterIndex];
            editor.ReplaceNode(_invocationExpression, CreateEnumerableReturnRoot(valueParameter, editor.SemanticModel, editor.Generator));
            return editor.GetChangedDocument();
        }

        private static SyntaxNode CreateEnumerableReturnRoot(ArgumentSyntax firstArgument, SemanticModel model, SyntaxGenerator generator)
            => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    (ExpressionSyntax)generator.TypeExpressionForStaticMemberAccess(model.Compilation.GetEnumerableType()!),
                    GenericName(nameof(Enumerable.Empty))
                        .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(CreateTypeFromArgumentType(firstArgument, model)))))
                    .WithAdditionalAnnotations(Simplifier.Annotation));

        private static TypeSyntax CreateTypeFromArgumentType(ArgumentSyntax firstArgument, SemanticModel model)
            => ParseTypeName(model.GetTypeInfo(firstArgument.Expression).Type?.ToMinimalDisplayString(model, firstArgument.SpanStart) ?? string.Empty);
    }
}
