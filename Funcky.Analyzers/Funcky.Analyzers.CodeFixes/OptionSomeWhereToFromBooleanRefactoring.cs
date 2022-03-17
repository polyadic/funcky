using System.Composition;
using System.Diagnostics.CodeAnalysis;
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
    private const string FromBoolean = "FromBoolean";
    private const string Where = "Where";
    private const string Return = "Return";
    private const string Some = "Some";

    public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        var (document, cancellationToken) = (context.Document, context.CancellationToken);
        var root = await document.GetSyntaxRootAsync(cancellationToken) ?? throw new InvalidOperationException("Missing syntax root");
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken) ?? throw new InvalidOperationException("Unable to get semantic model");

        if (GetSymbolsRequiredForRefactoring(semanticModel.Compilation) is { } symbols
            && FindInvocationExpression(context, root) is { } invocationExpression
            && IsWhereInvocation(invocationExpression, semanticModel, symbols, out var whereInvocation)
            && IsOptionReturnInvocation(whereInvocation.Instance, symbols, out var optionReturnInvocation))
        {
            context.RegisterRefactoring(CodeAction.Create("Replace with Option.FromBoolean", ReplaceWithOptionFromBoolean(document, symbols, invocationExpression, (InvocationExpressionSyntax)optionReturnInvocation.Syntax)));
        }
    }

    private static Symbols? GetSymbolsRequiredForRefactoring(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetGenericOptionType() is { } genericOptionType
           && optionType.GetMembers().OfType<IMethodSymbol>().Any(m => m.IsStatic && m.Name == FromBoolean)
            ? new Symbols(optionType, genericOptionType)
            : null;

    private static InvocationExpressionSyntax? FindInvocationExpression(CodeRefactoringContext context, SyntaxNode root)
        => root.FindNode(context.Span, getInnermostNodeForTie: true) is { } node
            && node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is { } expression
            ? expression
            : null;

    private static bool IsWhereInvocation(SyntaxNode syntax, SemanticModel semanticModel, Symbols symbols, [NotNullWhen(true)] out IInvocationOperation? whereInvocation)
    {
        whereInvocation = null;
        return semanticModel.GetOperation(syntax) is IInvocationOperation operation
           && operation.TargetMethod.Name == Where
           && SymbolEqualityComparer.Default.Equals(symbols.GenericOptionType, operation.TargetMethod.ContainingType.ConstructedFrom)
           && (whereInvocation = operation) is var _;
    }

    private static bool IsOptionReturnInvocation(IOperation? candidate, Symbols symbols, [NotNullWhen(true)] out IInvocationOperation? returnInvocationOperation)
    {
        returnInvocationOperation = null;
        return candidate is IInvocationOperation operation
            && operation.TargetMethod.Name is Return or Some
            && SymbolEqualityComparer.Default.Equals(symbols.OptionType, operation.TargetMethod.ContainingType)
            && (returnInvocationOperation = operation) is var _;
    }

    private Func<CancellationToken, Task<Document>> ReplaceWithOptionFromBoolean(Document document, Symbols symbols, InvocationExpressionSyntax whereInvocation, InvocationExpressionSyntax returnInvocation)
        => async cancellationToken =>
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var predicate = whereInvocation.ArgumentList.Arguments.First().Expression;
            var returnValue = returnInvocation.ArgumentList.Arguments.Single().Expression;

            editor.ReplaceNode(
                whereInvocation,
                CreateFromBooleanInvocation(
                    returnInvocation,
                    symbols,
                    editor.Generator,
                    ApplyPredicate(editor.SemanticModel, predicate, returnValue),
                    returnValue));

            return editor.GetChangedDocument();
        };

    private InvocationExpressionSyntax CreateFromBooleanInvocation(
        InvocationExpressionSyntax returnInvocation,
        Symbols symbols,
        SyntaxGenerator generator,
        ExpressionSyntax condition,
        ExpressionSyntax returnValue)
        => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    (ExpressionSyntax)generator.TypeExpressionForStaticMemberAccess(symbols.OptionType),
                    GetFromBooleanName(returnInvocation))
                    .WithAdditionalAnnotations(Simplifier.Annotation),
                ArgumentList(SeparatedList(new[] { Argument(condition), Argument(returnValue) })))
            .WithLeadingTrivia(returnInvocation.GetLeadingTrivia());

    private SimpleNameSyntax GetFromBooleanName(InvocationExpressionSyntax returnInvocation)
        => GetMethodName(returnInvocation) is GenericNameSyntax genericNameSyntax
            ? genericNameSyntax.WithIdentifier(Identifier(FromBoolean))
            : IdentifierName(FromBoolean);

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
