using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Funcky.BuiltinAnalyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionNoneInvocationCodeFix))]
public sealed class OptionNoneInvocationCodeFix : CodeFixProvider
{
    private const string CS1955 = nameof(CS1955); // error CS1955: Non-invocable member 'Member' cannot be used like a method.

    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(CS1955);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } root
            && await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel
            && semanticModel.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<InvocationExpressionSyntax>() is { } syntax
                    && IsInvocationOfOptionNoneProperty(semanticModel, syntax, optionOfTType))
                {
                    context.RegisterCodeFix(CreateFix(context, syntax), diagnostic);
                }
            }
        }
    }

    private static bool IsInvocationOfOptionNoneProperty(SemanticModel semanticModel, InvocationExpressionSyntax syntax, INamedTypeSymbol optionOfTType)
        => semanticModel.GetSymbolInfo(syntax.Expression) is { CandidateReason: CandidateReason.NotInvocable, CandidateSymbols: var candidates }
           && candidates.Any(IsOptionNoneProperty(optionOfTType));

    private static Func<ISymbol, bool> IsOptionNoneProperty(INamedTypeSymbol optionOfTType)
        => candidate
            => candidate is IPropertySymbol { Name: WellKnownMemberNames.OptionOfT.None, ContainingType: var type }
                && SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, optionOfTType);

    private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax syntax)
        => CodeAction.Create(
            "Replace with property access",
            ReplaceWithPropertyAccess(context.Document, syntax),
            nameof(OptionNoneInvocationCodeFix));

    private static Func<CancellationToken, Task<Document>> ReplaceWithPropertyAccess(Document document, InvocationExpressionSyntax syntax)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                var trailingTrivia = syntax.Expression.GetTrailingTrivia().AddRange(syntax.GetTrailingTrivia());
                editor.ReplaceNode(syntax, syntax.Expression.WithLeadingTrivia(syntax.GetLeadingTrivia()).WithTrailingTrivia(trailingTrivia));
                return editor.GetChangedDocument();
            };
}
