using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.OrNone;

/// <summary>Rewrites <c>xs.FirstOrDefault() ?? fallback</c> to <c>xs.FirstOrNone().GetOrElse(fallback)</c>.
/// The other null checks (<c>?.</c>, <c>is null</c>, …) have no single idiomatic shape, so they are reported without a fix.</summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferOrNoneCodeFix))]
public sealed class PreferOrNoneCodeFix : CodeFixProvider
{
    private const string ExtensionsNamespace = "Funcky.Extensions";

    private const string GetOrElseMethodName = "GetOrElse";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferOrNoneAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root?.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess } invocation
                && diagnostic.Properties.TryGetValue(PreferOrNoneAnalyzer.OrNoneMethodNameProperty, out var orNoneMethodName)
                && orNoneMethodName is not null
                && FindCoalesceExpression(invocation) is { } coalesce)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Replace with '{orNoneMethodName}().{GetOrElseMethodName}(…)'",
                        cancellationToken => ReplaceWithGetOrElseAsync(context.Document, new Rewrite(coalesce, invocation, memberAccess, orNoneMethodName), cancellationToken),
                        nameof(PreferOrNoneCodeFix)),
                    diagnostic);
            }
        }
    }

    private static BinaryExpressionSyntax? FindCoalesceExpression(InvocationExpressionSyntax invocation)
        => SkipParentheses(invocation) is { Parent: BinaryExpressionSyntax { RawKind: (int)SyntaxKind.CoalesceExpression } coalesce } left && coalesce.Left == left
            ? coalesce
            : null;

    private static SyntaxNode SkipParentheses(SyntaxNode node)
        => node.Parent is ParenthesizedExpressionSyntax parenthesized ? SkipParentheses(parenthesized) : node;

    private static async Task<Document> ReplaceWithGetOrElseAsync(Document document, Rewrite rewrite, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root
            || await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } semanticModel)
        {
            return document;
        }

        var orNoneInvocation = rewrite.Invocation.WithExpression(rewrite.MemberAccess.WithName(IdentifierName(rewrite.OrNoneMethodName).WithTriviaFrom(rewrite.MemberAccess.Name)));
        var getOrElse = InvocationExpression(
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, orNoneInvocation.WithoutTrailingTrivia(), IdentifierName(GetOrElseMethodName)),
                ArgumentList(SingletonSeparatedList(Argument(Fallback(rewrite.Coalesce.Right)))))
            .WithTriviaFrom(rewrite.Coalesce);

        var newRoot = root.ReplaceNode(rewrite.Coalesce, getOrElse);
        return document.WithSyntaxRoot(IsExtensionsNamespaceImported(semanticModel, rewrite.Invocation, orNoneInvocation) ? newRoot : UsingDirectives.AddUsing(newRoot, newRoot.FindNode(rewrite.Coalesce.Span), ExtensionsNamespace));
    }

    /// <summary>A <c>throw</c> fallback is a lazy one: <c>GetOrElse(() => throw …)</c>.</summary>
    private static ExpressionSyntax Fallback(ExpressionSyntax fallback)
        => fallback.WithoutTrivia() is ThrowExpressionSyntax throwExpression
            ? ParenthesizedLambdaExpression(throwExpression)
            : fallback.WithoutTrivia();

    private static bool IsExtensionsNamespaceImported(SemanticModel semanticModel, InvocationExpressionSyntax original, InvocationExpressionSyntax orNoneInvocation)
        => semanticModel.GetSpeculativeSymbolInfo(original.SpanStart, orNoneInvocation, SpeculativeBindingOption.BindAsExpression) is { Symbol: IMethodSymbol } or { CandidateSymbols.IsEmpty: false };

    private sealed record Rewrite(BinaryExpressionSyntax Coalesce, InvocationExpressionSyntax Invocation, MemberAccessExpressionSyntax MemberAccess, string OrNoneMethodName);
}
