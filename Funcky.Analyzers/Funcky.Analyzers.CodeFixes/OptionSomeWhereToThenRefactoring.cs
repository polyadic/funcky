using System.Composition;
using System.Diagnostics.CodeAnalysis;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

/// <summary>Rewrites <c>Option.Some(value).Where(predicate)</c> to <c>predicate(value).Then(value)</c>, with the predicate
/// inlined when it is a lambda.</summary>
[Shared]
[ExportCodeRefactoringProvider(LanguageNames.CSharp)]
public class OptionSomeWhereToThenRefactoring : CodeRefactoringProvider
{
    private delegate ExpressionSyntax ApplyPredicate(ExpressionSyntax value);

    public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        var (document, cancellationToken) = (context.Document, context.CancellationToken);
        var root = await document.GetSyntaxRootAsync(cancellationToken) ?? throw new InvalidOperationException("Missing syntax root");
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken) ?? throw new InvalidOperationException("Unable to get semantic model");

        // The document is rewritten up front: the refactoring is withheld when the Then call would not bind to Funcky's extension.
        if (GetSymbolsRequiredForRefactoring(semanticModel.Compilation) is { } symbols
            && FindInvocationExpression(context, root) is { } invocationExpression
            && IsWhereInvocation(invocationExpression, semanticModel, symbols, out var whereInvocation)
            && IsOptionReturnInvocation(whereInvocation.Instance, symbols, out var optionReturnInvocation)
            && TryGetPredicateApplier(invocationExpression, semanticModel) is { } applyPredicate
            && await ReplaceWithThenAsync(document, root, semanticModel, symbols, applyPredicate, invocationExpression, optionReturnInvocation, cancellationToken) is { } changedDocument)
        {
            context.RegisterRefactoring(CodeAction.Create($"Replace with {BooleanThenMethodName}", _ => Task.FromResult(changedDocument), nameof(OptionSomeWhereToThenRefactoring)));
        }
    }

    private static Symbols? GetSymbolsRequiredForRefactoring(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetOptionOfTType() is { } genericOptionType
           && compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
           && BooleanThen.HasThen(booleanExtensionsType)
            ? new Symbols(optionType, genericOptionType, booleanExtensionsType)
            : null;

    private static InvocationExpressionSyntax? FindInvocationExpression(CodeRefactoringContext context, SyntaxNode root)
        => root.FindNode(context.Span, getInnermostNodeForTie: true) is { } node
            && node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is { } expression
                ? expression
                : null;

    private static bool IsWhereInvocation(SyntaxNode syntax, SemanticModel semanticModel, Symbols symbols, [NotNullWhen(true)] out IInvocationOperation? whereInvocation)
    {
        whereInvocation = null;
        return semanticModel.GetOperation(syntax) is IInvocationOperation { TargetMethod.Name: WhereMethodName } operation
               && SymbolEquals(symbols.GenericOptionType, operation.TargetMethod.ContainingType.ConstructedFrom)
               && (whereInvocation = operation) is var _;
    }

    private static bool IsOptionReturnInvocation(IOperation? candidate, Symbols symbols, [NotNullWhen(true)] out IInvocationOperation? returnInvocationOperation)
    {
        returnInvocationOperation = null;
        return candidate is IInvocationOperation { TargetMethod.Name: MonadReturnMethodName or OptionSomeMethodName } operation
               && SymbolEquals(symbols.OptionType, operation.TargetMethod.ContainingType)
               && (returnInvocationOperation = operation) is var _;
    }

    private static ApplyPredicate? TryGetPredicateApplier(InvocationExpressionSyntax whereInvocation, SemanticModel semanticModel)
        => whereInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression switch
        {
            LambdaExpressionSyntax lambda when
                TryGetSingleParameter(lambda) is { } parameter
                && TryGetLambdaExpression(lambda) is { } expression
                => value => expression.ReplaceParameterReferences(semanticModel, parameter.Identifier.Text, value),
            { } predicate when semanticModel.GetOperation(predicate) is IMethodReferenceOperation
                => value => InvocationExpression(predicate, ArgumentList(SingletonSeparatedList(Argument(value)))),
            _ => null,
        };

    private static ParameterSyntax? TryGetSingleParameter(LambdaExpressionSyntax lambda)
        => lambda switch
        {
            SimpleLambdaExpressionSyntax { Parameter: var parameter } => parameter,
            ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters: [var parameter] } => parameter,
            _ => null,
        };

    private static ExpressionSyntax? TryGetLambdaExpression(LambdaExpressionSyntax lambda)
        => lambda switch
        {
            { ExpressionBody: { } expressionBody } => expressionBody,
            { Block.Statements: [ReturnStatementSyntax { Expression: var returnExpression }] } => returnExpression,
            _ => null,
        };

    private static Task<Document?> ReplaceWithThenAsync(Document document, SyntaxNode root, SemanticModel semanticModel, Symbols symbols, ApplyPredicate applyPredicate, InvocationExpressionSyntax whereInvocation, IInvocationOperation returnInvocation, CancellationToken cancellationToken)
    {
        var returnInvocationSyntax = (InvocationExpressionSyntax)returnInvocation.Syntax;
        var returnValue = returnInvocationSyntax.ArgumentList.Arguments.Single().Expression;
        var valueType = returnInvocation.TargetMethod.TypeArguments is [var typeArgument] ? typeArgument : null;
        var replacement = BooleanThen.Invocation(applyPredicate(returnValue), BooleanThen.ThenName(GetMethodName(returnInvocationSyntax)), BooleanThen.ValueArgument(returnValue, valueType))
            .WithLeadingTrivia(returnInvocationSyntax.GetLeadingTrivia());

        return BooleanThen.TryReplaceAsync(document, root, semanticModel, whereInvocation, replacement, symbols.BooleanExtensionsType, cancellationToken);
    }

    private static SimpleNameSyntax GetMethodName(InvocationExpressionSyntax invocationExpressionSyntax)
        => invocationExpressionSyntax.Expression switch
        {
            SimpleNameSyntax simpleNameSyntax => simpleNameSyntax,
            MemberAccessExpressionSyntax memberAccessExpressionSyntax => memberAccessExpressionSyntax.Name,
            var expression => throw new InvalidOperationException($"Unexpected invocation expression: {expression}"),
        };

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol GenericOptionType, INamedTypeSymbol BooleanExtensionsType);
}
