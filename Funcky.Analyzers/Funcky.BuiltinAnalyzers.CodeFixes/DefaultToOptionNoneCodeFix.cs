using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace Funcky.BuiltinAnalyzers;

/// <summary>Replaces a <c>default</c> literal flagged by λ0004 with <c>Option&lt;T&gt;.None</c>.
/// There is no fix for <c>Result</c> and <c>Either</c>: which error or left value was meant is not knowable.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DefaultToOptionNoneCodeFix))]
public sealed class DefaultToOptionNoneCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticName.DefaultInConversionId);

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
                if (root.FindNode(diagnostic.Location.SourceSpan) is LiteralExpressionSyntax defaultLiteral
                    && defaultLiteral.IsKind(SyntaxKind.DefaultLiteralExpression)
                    && FindTargetOptionType(semanticModel, defaultLiteral, optionOfTType, context.CancellationToken) is { } optionType)
                {
                    context.RegisterCodeFix(CreateFix(context.Document, defaultLiteral, optionType), diagnostic);
                }
            }
        }
    }

    /// <summary>The analyzer reports the literal, the conversion sits on an ancestor: the first one whose converted type is an option.</summary>
    private static INamedTypeSymbol? FindTargetOptionType(SemanticModel semanticModel, SyntaxNode defaultLiteral, INamedTypeSymbol optionOfTType, CancellationToken cancellationToken)
        => defaultLiteral
            .Ancestors()
            .TakeWhile(ancestor => ancestor is ExpressionSyntax or SwitchExpressionArmSyntax)
            .Select(ancestor => semanticModel.GetTypeInfo(ancestor, cancellationToken).ConvertedType)
            .OfType<INamedTypeSymbol>()
            .FirstOrDefault(type => SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, optionOfTType));

    private static CodeAction CreateFix(Document document, LiteralExpressionSyntax defaultLiteral, INamedTypeSymbol optionType)
        => CodeAction.Create(
            $"Replace with {optionType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.{WellKnownMemberNames.OptionOfT.None}",
            ReplaceWithNone(document, defaultLiteral, optionType),
            nameof(DefaultToOptionNoneCodeFix));

    private static Func<CancellationToken, Task<Document>> ReplaceWithNone(Document document, LiteralExpressionSyntax defaultLiteral, INamedTypeSymbol optionType)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                var none = editor.Generator
                    .MemberAccessExpression(editor.Generator.TypeExpression(optionType), WellKnownMemberNames.OptionOfT.None)
                    .WithTriviaFrom(defaultLiteral)
                    .WithAdditionalAnnotations(Simplifier.Annotation);
                editor.ReplaceNode(defaultLiteral, none);
                return editor.GetChangedDocument();
            };
}
