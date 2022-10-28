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
            context.RegisterCodeFix(CreateFix(context, declaration, valueParameterIndex), diagnostic);
        }
    }

    private static Diagnostic GetDiagnostic(CodeFixContext context)
        => context.Diagnostics.First();

    private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax declaration, int valueParameterIndex)
        => CodeAction.Create(
            EnumerableRepeatNeverCodeFixTitle,
            CreateSequenceReturnAsync(context.Document, declaration, valueParameterIndex),
            nameof(EnumerableRepeatNeverCodeFixTitle));

    private static Func<CancellationToken, Task<Document>> CreateSequenceReturnAsync(Document document, InvocationExpressionSyntax declaration, int valueParameterIndex)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                editor.ReplaceNode(declaration, CreateEnumerableReturnRoot(declaration.ArgumentList.Arguments[valueParameterIndex], editor.SemanticModel, editor.Generator));
                return editor.GetChangedDocument();
            };

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
