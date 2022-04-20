using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.BuiltinAnalyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionNoneCodeFix))]
public sealed class OptionNoneCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(OptionNoneMethodGroupAnalyzer.Descriptor.Id);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } root
            && await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel
            && semanticModel.Compilation.GetOptionType() is { } optionType)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ExpressionSyntax>(node => semanticModel.GetOperation(node) is IMethodReferenceOperation) is { } syntax
                    && semanticModel.GetOperation(syntax) is IMethodReferenceOperation methodReference)
                {
                    context.RegisterCodeFix(CreateFix(context, methodReference, optionType), diagnostic);
                }
            }
        }
    }

    private static CodeAction CreateFix(CodeFixContext context, IMethodReferenceOperation methodReference, INamedTypeSymbol optionType)
        => CodeAction.Create(
            "Use Option.None<T>",
            AddArgumentLabelAsync(context.Document, methodReference, optionType),
            nameof(OptionNoneCodeFix));

    private static Func<CancellationToken, Task<Document>> AddArgumentLabelAsync(Document document, IMethodReferenceOperation methodReference, INamedTypeSymbol optionType)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
                editor.ReplaceNode(methodReference.Syntax, GenerateOptionNoneMemberAccess(editor.Generator, methodReference, optionType));
                return editor.GetChangedDocument();
            };

    private static SyntaxNode GenerateOptionNoneMemberAccess(SyntaxGenerator generator, IMethodReferenceOperation methodReference, INamedTypeSymbol optionType)
    {
        var itemType = methodReference.Method.ContainingType.TypeArguments.Single();
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            (TypeSyntax)generator.TypeExpression(optionType),
            GenericName(
                Identifier(WellKnownMemberNames.Option.None),
                TypeArgumentList(SingletonSeparatedList((TypeSyntax)generator.TypeExpression(itemType)))))
            .WithAdditionalAnnotations(Simplifier.Annotation);
    }
}
