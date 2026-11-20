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
        if (root?.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess, ArgumentList.Arguments.Count: 1 } invocation)
        {
            context.RegisterCodeFix(CreateFix(context, invocation, memberAccess), diagnostic);
        }
    }

    private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        => CodeAction.Create(
            JoinToStringEmptyCodeFixTitle,
            CreateConcatToStringAsync(context.Document, invocation, memberAccess),
            nameof(JoinToStringEmptyCodeFixTitle));

    private static Func<CancellationToken, Task<Document>> CreateConcatToStringAsync(Document document, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        => async cancellationToken
            =>
            {
                var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                editor.ReplaceNode(invocation, ReplaceWithConcatToString(invocation, memberAccess));
                return editor.GetChangedDocument();
            };

    // Only the method name and the argument list change, so the receiver and its formatting stay as they are.
    private static InvocationExpressionSyntax ReplaceWithConcatToString(InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        => invocation
            .WithExpression(memberAccess.WithName(IdentifierName(ConcatToString).WithTriviaFrom(memberAccess.Name)))
            .WithArgumentList(ArgumentList().WithTriviaFrom(invocation.ArgumentList));
}
