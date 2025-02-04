using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Funcky.Analyzers.FunctionalAssert.FunctionalAssertAnalyzer;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddArgumentNameCodeFix))]
public sealed class FunctionalAssertFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(PreferFunctionalAssert.Id);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } root
            && await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Properties.TryGetValue(ExpectedArgumentIndex, out var expectedArgumentIndexString)
                    && int.TryParse(expectedArgumentIndexString, out var expectedArgumentIndex)
                    && diagnostic.Properties.TryGetValue(ActualArgumentIndex, out var actualArgumentIndexString)
                    && int.TryParse(actualArgumentIndexString, out var actualArgumentIndex)
                    && root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<InvocationExpressionSyntax>() is { } invocationSyntax
                    && invocationSyntax.ArgumentList.Arguments[expectedArgumentIndex] is var expectedArgumentSyntax
                    && invocationSyntax.ArgumentList.Arguments[actualArgumentIndex] is var actualArgumentSyntax
                    && actualArgumentSyntax.Expression is InvocationExpressionSyntax innerInvocationSyntax)
                {
                    context.RegisterCodeFix(CreateFix(context, new FixInputs(invocationSyntax, expectedArgumentSyntax, actualArgumentSyntax, innerInvocationSyntax)), diagnostic);
                }
            }
        }
    }

    private static CodeAction CreateFix(CodeFixContext context, FixInputs inputs)
        => CodeAction.Create(
            title: "Simplify assertion",
            SimplifyAssertionAsync(context.Document, inputs),
            nameof(AddArgumentNameCodeFix));

    private static Func<CancellationToken, Task<Document>> SimplifyAssertionAsync(Document document, FixInputs inputs)
        => async cancellationToken
            => await document.GetSyntaxRootAsync(cancellationToken) is { } syntaxRoot
                ? document.WithSyntaxRoot(syntaxRoot.ReplaceNode(inputs.InvocationSyntax, SimplifyAssertion(inputs)))
                : document;

    private static InvocationExpressionSyntax SimplifyAssertion(FixInputs inputs)
        => InvocationExpression(inputs.InnerInvocationSyntax.Expression)
            .WithArgumentList(FunctionalAssertArgumentList(inputs))
            .WithTriviaFrom(inputs.InvocationSyntax);

    private static ArgumentListSyntax FunctionalAssertArgumentList(FixInputs inputs)
    {
        var assertArgumentList = inputs.InvocationSyntax.ArgumentList;
        return assertArgumentList.WithArguments(SeparatedList(FunctionalAssertArguments(inputs), assertArgumentList.Arguments.GetSeparators()));
    }

    private static IEnumerable<ArgumentSyntax> FunctionalAssertArguments(FixInputs inputs)
        => [
            Argument(inputs.ExpectedArgumentSyntax.Expression).WithTriviaFrom(inputs.ExpectedArgumentSyntax),
            Argument(inputs.InnerInvocationSyntax.ArgumentList.Arguments[0].Expression).WithTriviaFrom(inputs.ActualArgumentSyntax),
        ];

    private sealed record FixInputs(
        InvocationExpressionSyntax InvocationSyntax,
        ArgumentSyntax ExpectedArgumentSyntax,
        ArgumentSyntax ActualArgumentSyntax,
        InvocationExpressionSyntax InnerInvocationSyntax);
}
