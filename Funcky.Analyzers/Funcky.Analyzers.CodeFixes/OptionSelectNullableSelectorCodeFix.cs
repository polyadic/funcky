using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

/// <summary>
/// Offers a fix for the compiler warning CS8714 ("Nullability of type argument doesn't match 'notnull' constraint")
/// on <c>Option&lt;T&gt;.Select</c>: the selector may return <c>null</c>, which <c>Select</c> cannot represent.
/// The fix rewrites <c>option.Select(x => expr)</c> to <c>option.SelectMany(x => Option.FromNullable(expr))</c>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionSelectNullableSelectorCodeFix))]
public sealed class OptionSelectNullableSelectorCodeFix : CodeFixProvider
{
    /// <summary>The compiler diagnostic reported when an inferred or explicit type argument violates a <c>notnull</c> constraint.</summary>
    private const string NotNullConstraintViolationDiagnosticId = "CS8714";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NotNullConstraintViolationDiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null || GetSymbols(semanticModel.Compilation) is not { } symbols)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess, ArgumentList.Arguments: [{ Expression: LambdaExpressionSyntax lambda }] } invocation
                && IsOptionSelect(semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol, symbols)
                && lambda is { ExpressionBody: not null } or { Block: not null })
            {
                context.RegisterCodeFix(new SelectManyFromNullableCodeFixAction(context.Document, symbols.OptionType, invocation, memberAccess, lambda), diagnostic);
            }
        }
    }

    private static Symbols? GetSymbols(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetOptionOfTType() is { } optionOfTType
           && optionType.GetMembers(OptionFromNullableMethodName).OfType<IMethodSymbol>().Any(method => method.IsStatic)
            ? new Symbols(optionType, optionOfTType)
            : null;

    private static bool IsOptionSelect(ISymbol? symbol, Symbols symbols)
        => symbol is IMethodSymbol { Name: SelectMethodName, Parameters.Length: 1 } method
           && SymbolEqualityComparer.Default.Equals(method.ContainingType.OriginalDefinition, symbols.OptionOfTType);

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol OptionOfTType);

    private sealed class SelectManyFromNullableCodeFixAction(
        Document document,
        INamedTypeSymbol optionType,
        InvocationExpressionSyntax invocation,
        MemberAccessExpressionSyntax memberAccess,
        LambdaExpressionSyntax lambda) : CodeAction
    {
        public override string Title => $"Replace {SelectMethodName} with {SelectManyMethodName} and Option.{OptionFromNullableMethodName}";

        public override string EquivalenceKey => nameof(OptionSelectNullableSelectorCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var fromNullable = (ExpressionSyntax)editor.Generator.TypeExpressionForStaticMemberAccess(optionType);

            editor.ReplaceNode(
                invocation,
                invocation
                    .WithExpression(memberAccess.WithName(IdentifierName(SelectManyMethodName).WithTriviaFrom(memberAccess.Name)))
                    .WithArgumentList(invocation.ArgumentList.ReplaceNode(lambda, WrapReturnValues(lambda, fromNullable))));

            return editor.GetChangedDocument();
        }

        private static LambdaExpressionSyntax WrapReturnValues(LambdaExpressionSyntax lambda, ExpressionSyntax optionTypeExpression)
            => lambda switch
            {
                { ExpressionBody: { } expressionBody } => lambda.WithExpressionBody(WrapInFromNullable(expressionBody, optionTypeExpression)),
                { Block: { } block } => lambda.WithBlock((BlockSyntax)new ReturnValueRewriter(optionTypeExpression).Visit(block)),
                _ => lambda,
            };

        private static InvocationExpressionSyntax WrapInFromNullable(ExpressionSyntax value, ExpressionSyntax optionTypeExpression)
            => InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, optionTypeExpression, IdentifierName(OptionFromNullableMethodName))
                        .WithAdditionalAnnotations(Simplifier.Annotation),
                    ArgumentList(SingletonSeparatedList(Argument(value.WithoutTrivia()))))
                .WithTriviaFrom(value);

        /// <summary>Wraps the value of every <c>return</c> that belongs to the lambda itself, leaving nested functions alone.</summary>
        private sealed class ReturnValueRewriter(ExpressionSyntax optionTypeExpression) : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
                => node.Expression is { } value
                    ? node.WithExpression(WrapInFromNullable(value, optionTypeExpression))
                    : node;

            public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) => node;

            public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) => node;

            public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) => node;

            public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) => node;
        }
    }
}
