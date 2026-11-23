using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Funcky.Analyzers.CodeFixResources;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JoinToStringEmptyCodeFix))]
public sealed class JoinToStringEmptyCodeFix : CodeFixProvider
{
    private const string ConcatToString = "ConcatToString";

    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(JoinToStringEmptyAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();

        // The diagnostic spans the whole JoinToString invocation, so we look up the node with exactly that span.
        // Searching from the first token instead would find the innermost invocation of a method chain
        // (e.g. the GroupBy in modes.GroupBy(...).Select(...).JoinToString("")). A static call such as
        // EnumerableExtensions.JoinToString(source, "") passes the source as an argument, so we leave it alone.
        if (root?.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } invocation
            && ReplaceWithConcatToString(invocation) is { } replacement)
        {
            context.RegisterCodeFix(CreateFix(context, invocation, replacement), diagnostic);
        }
    }

    private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax invocation, InvocationExpressionSyntax replacement)
        => CodeAction.Create(
            JoinToStringEmptyCodeFixTitle,
            ReplaceAsync(context.Document, invocation, replacement),
            nameof(JoinToStringEmptyCodeFixTitle));

    private static Func<CancellationToken, Task<Document>> ReplaceAsync(Document document, InvocationExpressionSyntax invocation, InvocationExpressionSyntax replacement)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                editor.ReplaceNode(invocation, replacement);
                return editor.GetChangedDocument();
            };

    // Only the method name and the argument list change, so the receiver and its formatting stay as they are.
    private static InvocationExpressionSyntax? ReplaceWithConcatToString(InvocationExpressionSyntax invocation)
        => RenameToConcatToString(invocation.Expression) is { } expression
            ? invocation
                .WithExpression(expression)
                .WithArgumentList(ArgumentList().WithTriviaFrom(invocation.ArgumentList))
            : null;

    // The method is accessed either directly (strings.JoinToString) or through a conditional access (strings?.JoinToString).
    private static ExpressionSyntax? RenameToConcatToString(ExpressionSyntax expression)
        => expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.WithName(ConcatToStringName(memberAccess.Name)),
            MemberBindingExpressionSyntax memberBinding => memberBinding.WithName(ConcatToStringName(memberBinding.Name)),
            _ => null,
        };

    private static IdentifierNameSyntax ConcatToStringName(SimpleNameSyntax name)
        => IdentifierName(ConcatToString).WithTriviaFrom(name);
}
