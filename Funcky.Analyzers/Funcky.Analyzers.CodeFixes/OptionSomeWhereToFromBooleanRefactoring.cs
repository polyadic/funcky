using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeRefactoringProvider(LanguageNames.CSharp)]
public class OptionSomeWhereToFromBooleanRefactoring : CodeRefactoringProvider
{
    public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        var (document, cancellationToken) = (context.Document, context.CancellationToken);
        var root = await document.GetSyntaxRootAsync(cancellationToken) ?? throw new InvalidOperationException("Missing syntax root");
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken) ?? throw new InvalidOperationException("Unable to get semantic model");

        if (GetSymbolsRequiredForRefactoring(semanticModel.Compilation) is { } symbols
            && root.FindNode(context.Span, getInnermostNodeForTie: true) is { } node
            && node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is { } whereInvocationCandidate
            && semanticModel.GetOperation(whereInvocationCandidate) is IInvocationOperation whereInvocationCandidateOperation
            && whereInvocationCandidateOperation.TargetMethod.Name == "Where"
            && SymbolEqualityComparer.Default.Equals(symbols.GenericOptionType, whereInvocationCandidateOperation.TargetMethod.ContainingType.ConstructedFrom)
            && whereInvocationCandidateOperation.Instance is IInvocationOperation optionReturnInvocationCandidateOperation
            && optionReturnInvocationCandidateOperation.TargetMethod.Name is "Return" or "Some"
            && SymbolEqualityComparer.Default.Equals(symbols.OptionType, optionReturnInvocationCandidateOperation.TargetMethod.ContainingType))
        {
            context.RegisterRefactoring(CodeAction.Create("Replace with Option.FromBoolean", ReplaceWithOptionFromBoolean(document, symbols, whereInvocationCandidate, (InvocationExpressionSyntax)optionReturnInvocationCandidateOperation.Syntax)));
        }
    }

    private static Symbols? GetSymbolsRequiredForRefactoring(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetGenericOptionType() is { } genericOptionType
           && optionType.GetMembers().OfType<IMethodSymbol>().Any(m => m.IsStatic && m.Name == "FromBoolean")
            ? new Symbols(optionType, genericOptionType)
            : null;

    private Func<CancellationToken, Task<Document>> ReplaceWithOptionFromBoolean(Document document, Symbols symbols, InvocationExpressionSyntax whereInvocation, InvocationExpressionSyntax returnInvocation)
        => async cancellationToken =>
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var predicate = whereInvocation.ArgumentList.Arguments.First().Expression;
            var value = returnInvocation.ArgumentList.Arguments.Single().Expression;
            var returnMethodName = GetMethodName(returnInvocation);
            SimpleNameSyntax fromBooleanName = returnMethodName is GenericNameSyntax genericNameSyntax
                ? genericNameSyntax.WithIdentifier(Identifier("FromBoolean"))
                : IdentifierName("FromBoolean");

            var replacement = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        (ExpressionSyntax)editor.Generator.TypeExpressionForStaticMemberAccess(symbols.OptionType),
                        fromBooleanName)
                        .WithAdditionalAnnotations(Simplifier.Annotation),
                    ArgumentList(SeparatedList(new[] { Argument(ApplyPredicate(editor.SemanticModel, predicate, value)), Argument(value) })))
                .WithLeadingTrivia(returnInvocation.GetLeadingTrivia());

            editor.ReplaceNode(
                whereInvocation,
                replacement);

            return editor.GetChangedDocument();
        };

    private SimpleNameSyntax GetMethodName(InvocationExpressionSyntax invocationExpressionSyntax)
        => invocationExpressionSyntax.Expression switch
        {
            SimpleNameSyntax simpleNameSyntax => simpleNameSyntax,
            MemberAccessExpressionSyntax memberAccessExpressionSyntax => memberAccessExpressionSyntax.Name,
        };

    private ExpressionSyntax ApplyPredicate(SemanticModel semanticModel, ExpressionSyntax predicate, ExpressionSyntax value)
        => predicate switch
        {
            SimpleLambdaExpressionSyntax lambda => (ExpressionSyntax)new ReplaceParameterReferenceRewriter(semanticModel, lambda.Parameter.Identifier.Text, value).Visit(lambda.Body),
            ParenthesizedLambdaExpressionSyntax lambda => (ExpressionSyntax)new ReplaceParameterReferenceRewriter(semanticModel, lambda.ParameterList.Parameters.Single().Identifier.Text, value).Visit(lambda.Body),
            _ when semanticModel.GetOperation(predicate) is IMethodReferenceOperation
                => InvocationExpression(
                    predicate,
                    ArgumentList(SingletonSeparatedList(Argument(value)))),
        };

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol GenericOptionType);
}
