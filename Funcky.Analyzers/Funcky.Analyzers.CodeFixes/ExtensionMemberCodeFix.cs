using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Funcky.Analyzers;

/// <summary>Base of the code fixes that replace a call with one of a static extension member in <c>Funcky.Extensions</c> and add
/// the <c>using</c> for the namespace when needed. Every fix adds the same directive, which the batch fixer treats as colliding
/// edits: with three or more fixes in a document it drops some of them, call rewrite included. The Fix-All provider of this
/// base therefore rewrites all flagged calls of a document at once and adds the directive once.</summary>
public abstract class ExtensionMemberCodeFix : CodeFixProvider
{
    public sealed override FixAllProvider GetFixAllProvider() => ExtensionMemberFixAllProvider.Instance;

    /// <summary>Registers one action per fixable diagnostic, which applies its rewrite when invoked.</summary>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if (TryCreateRewrite(root, semanticModel, diagnostic, context.CancellationToken) is { } rewrite)
            {
                context.RegisterCodeFix(CodeAction.Create(GetTitle(rewrite), cancellationToken => ApplyAsync(context.Document, rewrite, cancellationToken), GetType().Name), diagnostic);
            }
        }
    }

    /// <summary>The title of the action that applies the rewrite.</summary>
    protected abstract string GetTitle(ExtensionMemberRewrite rewrite);

    /// <summary>The rewrite that fixes the diagnostic, or <see langword="null"/> when there is no fix for it.</summary>
    protected abstract ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken);

    /// <summary>Applies the rewrites to the document, adding the <c>using</c> when needed, or returns <see langword="null"/> when none of them can be applied.</summary>
    protected virtual Task<Document?> TryApplyAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, CancellationToken cancellationToken)
        => Task.FromResult<Document?>(FunckyExtensionsImport.ReplaceWithExtensionMemberCalls(document, root, semanticModel, rewrites, ExtensionMemberSupport.IsDeclaredInFunckyExtensions));

    private async Task<Document> ApplyAsync(Document document, ExtensionMemberRewrite rewrite, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        return root is null || semanticModel is null
            ? document
            : await TryApplyAsync(document, root, semanticModel, ImmutableArray.Create(rewrite), cancellationToken).ConfigureAwait(false) ?? document;
    }

    private async Task<Document?> FixAllAsync(Document document, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return null;
        }

        var rewrites = diagnostics
            .Select(diagnostic => TryCreateRewrite(root, semanticModel, diagnostic, cancellationToken))
            .OfType<ExtensionMemberRewrite>()
            .ToImmutableArray();

        return rewrites.IsEmpty
            ? null
            : await TryApplyAsync(document, root, semanticModel, rewrites, cancellationToken).ConfigureAwait(false);
    }

    private sealed class ExtensionMemberFixAllProvider : DocumentBasedFixAllProvider
    {
        public static readonly ExtensionMemberFixAllProvider Instance = new();

        protected override Task<Document?> FixAllAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            => fixAllContext.CodeFixProvider is ExtensionMemberCodeFix codeFix
                ? codeFix.FixAllAsync(document, diagnostics, fixAllContext.CancellationToken)
                : Task.FromResult<Document?>(null);
    }
}
