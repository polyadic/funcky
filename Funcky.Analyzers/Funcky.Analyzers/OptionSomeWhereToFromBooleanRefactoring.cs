using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
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
        var optionType = semanticModel.Compilation.GetGenericOptionType();
        var nonGenericOptionType = semanticModel.Compilation.GetOptionType();

        if (root.FindNode(context.Span, getInnermostNodeForTie: true) is { } node
            && node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is { } whereInvocationCandidate
            && semanticModel.GetOperation(whereInvocationCandidate) is IInvocationOperation whereInvocationCandidateOperation
            && whereInvocationCandidateOperation.TargetMethod.Name == "Where"
            && SymbolEqualityComparer.Default.Equals(optionType, whereInvocationCandidateOperation.TargetMethod.ContainingType.ConstructedFrom)
            && whereInvocationCandidateOperation.Instance is IInvocationOperation optionReturnInvocationCandidateOperation
            && optionReturnInvocationCandidateOperation.TargetMethod.Name is "Return" or "Some"
            && SymbolEqualityComparer.Default.Equals(nonGenericOptionType, optionReturnInvocationCandidateOperation.TargetMethod.ContainingType)
            && nonGenericOptionType.GetMembers().Any(m => m is IMethodSymbol && m.IsStatic && m.Name == "FromBoolean"))
        {
            context.RegisterRefactoring(CodeAction.Create("Replace with Option.FromBoolean", ReplaceWithOptionFromBoolean(document, whereInvocationCandidate, (InvocationExpressionSyntax)optionReturnInvocationCandidateOperation.Syntax)));
        }
    }

    private Func<CancellationToken, Task<Document>> ReplaceWithOptionFromBoolean(Document document, InvocationExpressionSyntax whereInvocation, InvocationExpressionSyntax returnInvocation)
        => async cancellationToken =>
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var nonGenericOptionType = editor.SemanticModel.Compilation.GetTypeByMetadataName("Funcky.Monads.Option")!;
            var predicate = whereInvocation.ArgumentList.Arguments.First().Expression;
            var value = returnInvocation.ArgumentList.Arguments.Single().Expression;
            var returnMethodName = GetMethodName(returnInvocation);
            SimpleNameSyntax fromBooleanName = returnMethodName is GenericNameSyntax genericNameSyntax
                ? genericNameSyntax.WithIdentifier(Identifier("FromBoolean"))
                : IdentifierName("FromBoolean");

            var replacement = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(nonGenericOptionType.ToMinimalDisplayString(editor.SemanticModel, whereInvocation.SpanStart)),
                        fromBooleanName),
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
            SimpleLambdaExpressionSyntax lambda => (ExpressionSyntax)new Rewriter(semanticModel, lambda.Parameter.Identifier.Text, value).Visit(lambda.Body),
            ParenthesizedLambdaExpressionSyntax lambda => (ExpressionSyntax)new Rewriter(semanticModel, lambda.ParameterList.Parameters.Single().Identifier.Text, value).Visit(lambda.Body),
            _ when semanticModel.GetOperation(predicate) is IMethodReferenceOperation
                => InvocationExpression(
                    predicate,
                    ArgumentList(SingletonSeparatedList(Argument(value)))),
        };
}

internal static class CompilationExtensions
{
    public static INamedTypeSymbol? GetGenericOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option`1");

    public static INamedTypeSymbol? GetOptionType(this Compilation compilation) => compilation.GetTypeByMetadataName("Funcky.Monads.Option");
}

internal sealed class Rewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly string _parameterName;
    private readonly ExpressionSyntax _replacement;

    public Rewriter(SemanticModel semanticModel, string parameterName, ExpressionSyntax replacement)
        : base(visitIntoStructuredTrivia: false)
    {
        _semanticModel = semanticModel;
        _parameterName = parameterName;
        _replacement = replacement;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (_semanticModel.GetOperation(node) is IParameterReferenceOperation { Parameter.Name: var name } && name == _parameterName)
        {
            return _replacement.WithTriviaFrom(node);
        }

        return node;
    }
}
