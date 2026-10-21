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
            // CS8714 is reported on the member access of the offending call. A diagnostic on a constrained type inside
            // the lambda (`option.Select(_ => new Box<string?>())`) has the same enclosing invocation but is not ours.
            if (root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { Expression: MemberAccessExpressionSyntax memberAccess, ArgumentList.Arguments: [{ Expression: LambdaExpressionSyntax lambda }] } invocation
                && memberAccess.Span == diagnostic.Location.SourceSpan
                && semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol { TypeArguments: [var selectorResultType] } method
                && IsOptionSelect(method, symbols)
                && HasRewritableBody(lambda)
                && GetNonNullableType(selectorResultType) is { } resultType)
            {
                context.RegisterCodeFix(new SelectManyFromNullableCodeFixAction(context.Document, symbols, resultType, invocation, memberAccess, lambda), diagnostic);
            }
        }
    }

    private static Symbols? GetSymbols(Compilation compilation)
    {
        if (compilation.GetOptionType() is { } optionType
            && compilation.GetOptionOfTType() is { } optionOfTType
            && optionType.GetMembers(OptionFromNullableMethodName).OfType<IMethodSymbol>().Any(method => method.IsStatic))
        {
            return new Symbols(optionType, optionOfTType);
        }

        return null;
    }

    private static bool IsOptionSelect(IMethodSymbol method, Symbols symbols)
        => method is { Name: SelectMethodName, Parameters.Length: 1 }
           && SymbolEqualityComparer.Default.Equals(method.ContainingType.OriginalDefinition, symbols.OptionOfTType);

    // A throw expression cannot be an argument, so `_ => throw ...` cannot be wrapped. A lambda that spells out its
    // return type is only rewritten when that type is `T?`, which becomes `Option<T>`.
    private static bool HasRewritableBody(LambdaExpressionSyntax lambda)
        => lambda switch
        {
            { ExpressionBody: ThrowExpressionSyntax } => false,
            { ExpressionBody: null, Block: null } => false,
            ParenthesizedLambdaExpressionSyntax { ReturnType: { } returnType } => returnType is NullableTypeSyntax,
            _ => true,
        };

    // `Option.FromNullable` has an overload for `T?` reference types and one for `Nullable<T>`; both yield `Option<T>`.
    private static ITypeSymbol? GetNonNullableType(ITypeSymbol selectorResultType)
        => selectorResultType switch
        {
            INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments: [var underlyingType] } => underlyingType,
            { IsReferenceType: true, TypeKind: not TypeKind.TypeParameter } => selectorResultType.WithNullableAnnotation(NullableAnnotation.NotAnnotated),
            _ => null,
        };

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol OptionOfTType);

    private sealed class SelectManyFromNullableCodeFixAction(
        Document document,
        Symbols symbols,
        ITypeSymbol resultType,
        InvocationExpressionSyntax invocation,
        MemberAccessExpressionSyntax memberAccess,
        LambdaExpressionSyntax lambda) : CodeAction
    {
        public override string Title => $"Replace {SelectMethodName} with {SelectManyMethodName} and Option.{OptionFromNullableMethodName}";

        public override string EquivalenceKey => nameof(OptionSelectNullableSelectorCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var wrapper = new FromNullableWrapper(
                OptionTypeExpression: (ExpressionSyntax)editor.Generator.TypeExpressionForStaticMemberAccess(symbols.OptionType),
                ResultType: (TypeSyntax)editor.Generator.TypeExpression(resultType).WithAdditionalAnnotations(Simplifier.Annotation));
            var optionOfResultType = (TypeSyntax)editor.Generator.TypeExpression(symbols.OptionOfTType.Construct(resultType)).WithAdditionalAnnotations(Simplifier.Annotation);

            editor.ReplaceNode(
                invocation,
                invocation
                    .WithExpression(memberAccess.WithName(IdentifierName(SelectManyMethodName).WithTriviaFrom(memberAccess.Name)))
                    .WithArgumentList(invocation.ArgumentList.ReplaceNode(lambda, WithOptionReturnType(WrapReturnValues(lambda, wrapper), optionOfResultType))));

            return editor.GetChangedDocument();
        }

        private static LambdaExpressionSyntax WrapReturnValues(LambdaExpressionSyntax lambda, FromNullableWrapper wrapper)
            => lambda switch
            {
                { ExpressionBody: { } expressionBody } => lambda.WithExpressionBody(wrapper.Wrap(expressionBody)),
                { Block: { } block } => lambda.WithBlock((BlockSyntax)new ReturnValueRewriter(wrapper).Visit(block)),
                _ => lambda,
            };

        private static LambdaExpressionSyntax WithOptionReturnType(LambdaExpressionSyntax lambda, TypeSyntax optionOfResultType)
            => lambda is ParenthesizedLambdaExpressionSyntax { ReturnType: { } returnType } parenthesizedLambda
                ? parenthesizedLambda.WithReturnType(optionOfResultType.WithTriviaFrom(returnType))
                : lambda;

        private sealed record FromNullableWrapper(ExpressionSyntax OptionTypeExpression, TypeSyntax ResultType)
        {
            public InvocationExpressionSyntax Wrap(ExpressionSyntax value)
                => InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, OptionTypeExpression, FromNullableName(value))
                            .WithAdditionalAnnotations(Simplifier.Annotation),
                        ArgumentList(SingletonSeparatedList(Argument(value.WithoutTrivia()))))
                    .WithTriviaFrom(value);

            // `null` and `default` carry no type of their own, so the type argument inference of `FromNullable` would fail.
            private SimpleNameSyntax FromNullableName(ExpressionSyntax value)
                => NeedsExplicitTypeArgument(value)
                    ? GenericName(Identifier(OptionFromNullableMethodName), TypeArgumentList(SingletonSeparatedList(ResultType)))
                    : IdentifierName(OptionFromNullableMethodName);

            private static bool NeedsExplicitTypeArgument(ExpressionSyntax value)
                => value switch
                {
                    ParenthesizedExpressionSyntax parenthesized => NeedsExplicitTypeArgument(parenthesized.Expression),
                    LiteralExpressionSyntax literal => literal.IsKind(SyntaxKind.NullLiteralExpression) || literal.IsKind(SyntaxKind.DefaultLiteralExpression),
                    _ => false,
                };
        }

        /// <summary>Wraps the value of every <c>return</c> that belongs to the lambda itself, leaving nested functions alone.</summary>
        private sealed class ReturnValueRewriter(FromNullableWrapper wrapper) : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
                => node.Expression is { } value
                    ? node.WithExpression(wrapper.Wrap(value))
                    : node;

            public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) => node;

            public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) => node;

            public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) => node;

            public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) => node;
        }
    }
}
