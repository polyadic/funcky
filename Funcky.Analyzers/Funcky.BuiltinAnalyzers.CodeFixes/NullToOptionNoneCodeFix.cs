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

/// <summary>Replaces a <c>null</c> literal flagged by λ0005 with <c>Option&lt;T&gt;.None</c>; a call <c>Option.Some(null)</c>
/// or <c>Option.Return(null)</c> is replaced as a whole. There is no fix for <c>Result</c> and <c>Either</c>:
/// which error or left value was meant is not knowable.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullToOptionNoneCodeFix))]
public sealed class NullToOptionNoneCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticName.NullToMonadId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } root
            && await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel
            && semanticModel.Compilation.GetOptionOfTType() is { } optionOfTType
            && semanticModel.Compilation.GetOptionType() is { } optionType)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax nullLiteral
                    && nullLiteral.IsKind(SyntaxKind.NullLiteralExpression)
                    && FindReplacement(semanticModel, nullLiteral, optionOfTType, optionType, context.CancellationToken) is { } replacement)
                {
                    context.RegisterCodeFix(CreateFix(context.Document, replacement), diagnostic);
                }
            }
        }
    }

    private static Replacement? FindReplacement(SemanticModel semanticModel, LiteralExpressionSyntax nullLiteral, INamedTypeSymbol optionOfTType, INamedTypeSymbol optionType, CancellationToken cancellationToken)
        => FindSomeInvocation(semanticModel, nullLiteral, optionOfTType, optionType, cancellationToken)
           ?? FindConvertedExpression(semanticModel, nullLiteral, optionOfTType, cancellationToken);

    /// <summary>The literal is the only argument of <c>Option.Some</c> or <c>Option.Return</c>: the whole call is <c>None</c>.</summary>
    private static Replacement? FindSomeInvocation(SemanticModel semanticModel, LiteralExpressionSyntax nullLiteral, INamedTypeSymbol optionOfTType, INamedTypeSymbol optionType, CancellationToken cancellationToken)
        => nullLiteral.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } }
           && semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol { ReturnType: INamedTypeSymbol returnType } method
           && SymbolEqualityComparer.Default.Equals(method.ContainingType, optionType)
           && SymbolEqualityComparer.Default.Equals(returnType.OriginalDefinition, optionOfTType)
            ? new Replacement(invocation, returnType)
            : null;

    /// <summary>The analyzer reports the literal, the conversion sits on it or on an ancestor. The fix is for the first user-defined
    /// conversion on the way up, which is the one that produced the diagnostic: in <c>result == null</c> returned as <c>Option&lt;bool&gt;</c>
    /// that is the conversion to <c>Result</c>, so there is no fix even though an option sits further out. A cast on the way
    /// (<c>(string)null</c>) would reject or box the <c>None</c>, so there is no fix either.</summary>
    private static Replacement? FindConvertedExpression(SemanticModel semanticModel, LiteralExpressionSyntax nullLiteral, INamedTypeSymbol optionOfTType, CancellationToken cancellationToken)
    {
        foreach (var ancestor in nullLiteral.AncestorsAndSelf().TakeWhile(ancestor => ancestor is ExpressionSyntax or SwitchExpressionArmSyntax))
        {
            if (ancestor is CastExpressionSyntax)
            {
                return null;
            }

            if (ancestor is ExpressionSyntax expression && semanticModel.GetConversion(expression, cancellationToken).IsUserDefined)
            {
                return semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType is INamedTypeSymbol targetType
                    && SymbolEqualityComparer.Default.Equals(targetType.OriginalDefinition, optionOfTType)
                    ? new Replacement(nullLiteral, targetType)
                    : null;
            }
        }

        return null;
    }

    private static CodeAction CreateFix(Document document, Replacement replacement)
        => CodeAction.Create(
            $"Replace with {replacement.OptionType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.{WellKnownMemberNames.OptionOfT.None}",
            ReplaceWithNone(document, replacement),
            nameof(NullToOptionNoneCodeFix));

    private static Func<CancellationToken, Task<Document>> ReplaceWithNone(Document document, Replacement replacement)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                var none = editor.Generator
                    .MemberAccessExpression(editor.Generator.TypeExpression(replacement.OptionType), WellKnownMemberNames.OptionOfT.None)
                    .WithTriviaFrom(replacement.Node)
                    .WithAdditionalAnnotations(Simplifier.Annotation);
                editor.ReplaceNode(replacement.Node, none);
                return editor.GetChangedDocument();
            };

    private sealed class Replacement
    {
        public Replacement(SyntaxNode node, INamedTypeSymbol optionType)
            => (Node, OptionType) = (node, optionType);

        public SyntaxNode Node { get; }

        public INamedTypeSymbol OptionType { get; }
    }
}
