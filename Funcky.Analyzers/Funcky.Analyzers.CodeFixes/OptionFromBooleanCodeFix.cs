using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

/// <summary>
/// Offers a fix for the obsoletion warning CS0618 on <c>Option.FromBoolean</c>, which <c>bool.Then</c> replaces:
/// <c>Option.FromBoolean(condition, value)</c> becomes <c>condition.Then(value)</c>, the overload with a selector keeps
/// its selector, <c>Option.FromBoolean(condition)</c> becomes <c>condition.Then(Unit.Value)</c>, and the flattening idiom
/// <c>Option.FromBoolean(condition, option).SelectMany(Identity)</c> becomes <c>condition.Then(() => option)</c>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionFromBooleanCodeFix))]
public sealed class OptionFromBooleanCodeFix : CodeFixProvider
{
    /// <summary>The compiler diagnostic reported on the use of a member marked <c>[Obsolete]</c> with a message.</summary>
    private const string ObsoleteMemberDiagnosticId = "CS0618";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ObsoleteMemberDiagnosticId);

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
            // The warning spans the whole call; one with a smaller span belongs to an obsolete member inside an argument.
            if (root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
                && invocation.Span == diagnostic.Location.SourceSpan
                && semanticModel.GetOperation(invocation, context.CancellationToken) is IInvocationOperation { TargetMethod: { Name: OptionFromBooleanMethodName, IsStatic: true } method } operation
                && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbols.OptionType)
                && Rewrite.TryCreate(operation, invocation, semanticModel, symbols, context.CancellationToken) is { } rewrite)
            {
                context.RegisterCodeFix(new ReplaceWithThenCodeFixAction(context.Document, symbols, rewrite), diagnostic);
            }
        }
    }

    private static Symbols? GetSymbols(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetOptionOfTType() is { } optionOfTType
           && compilation.GetUnitType() is { } unitType
           && compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
           && BooleanThen.HasThen(booleanExtensionsType)
            ? new Symbols(optionType, optionOfTType, unitType, booleanExtensionsType, compilation.GetFunctionalType())
            : null;

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol OptionOfTType, INamedTypeSymbol UnitType, INamedTypeSymbol BooleanExtensionsType, INamedTypeSymbol? FunctionalType);

    /// <summary>What to replace (the <c>FromBoolean</c> call, or the <c>SelectMany(Identity)</c> around it) and with what.</summary>
    private sealed record Rewrite(InvocationExpressionSyntax Target, ExpressionSyntax Condition, SimpleNameSyntax ThenName, ArgumentSyntax ThenArgument)
    {
        public static Rewrite? TryCreate(IInvocationOperation operation, InvocationExpressionSyntax invocation, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            if (GetMethodName(invocation) is not { } name)
            {
                return null;
            }

            var thenName = BooleanThen.ThenName(name);

            // The arguments are in evaluation order, which named arguments can change.
            return operation.Arguments.OrderBy(argument => argument.Parameter?.Ordinal ?? int.MaxValue).Select(argument => argument.Syntax).ToArray() switch
            {
                [ArgumentSyntax condition] => new Rewrite(invocation, condition.Expression, thenName, Argument(UnitValue(symbols))),
                [ArgumentSyntax condition, ArgumentSyntax argument] => TryCreate(operation, invocation, condition.Expression, thenName, argument.Expression, semanticModel, symbols, cancellationToken),
                _ => null,
            };
        }

        private static Rewrite? TryCreate(IInvocationOperation operation, InvocationExpressionSyntax invocation, ExpressionSyntax condition, SimpleNameSyntax thenName, ExpressionSyntax argument, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            var isSelectorOverload = operation.TargetMethod.Parameters[1].Type.TypeKind == TypeKind.Delegate;
            var producesNestedOption = operation.TargetMethod.TypeArguments is [INamedTypeSymbol { OriginalDefinition: var itemType }] && SymbolEqualityComparer.Default.Equals(itemType, symbols.OptionOfTType);

            return (FindSelectManyIdentity(invocation, semanticModel, symbols, cancellationToken), isSelectorOverload, producesNestedOption) switch
            {
                // Option.FromBoolean(condition, option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, false, _) => new Rewrite(selectMany, condition, thenName, Argument(ParenthesizedLambdaExpression(argument.WithoutTrivia()))),

                // Option.FromBoolean(condition, () => option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, true, _) => new Rewrite(selectMany, condition, thenName, Argument(argument)),

                // Option.FromBoolean(condition, () => option) yields an Option<Option<T>>, which Then would flatten.
                (null, true, true) => null,

                (null, _, _) => new Rewrite(invocation, condition, thenName, Argument(argument)),
            };
        }

        private static InvocationExpressionSyntax? FindSelectManyIdentity(InvocationExpressionSyntax invocation, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
            => invocation.Parent is MemberAccessExpressionSyntax { Name.Identifier.ValueText: SelectManyMethodName, Parent: InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: var selector }] } selectMany }
               && symbols.FunctionalType is { } functionalType
               && semanticModel.GetSymbolInfo(selectMany, cancellationToken).Symbol is IMethodSymbol { Name: SelectManyMethodName } selectManyMethod
               && SymbolEqualityComparer.Default.Equals(selectManyMethod.ContainingType.OriginalDefinition, symbols.OptionOfTType)
               && IsIdentity(semanticModel.GetSymbolInfo(selector, cancellationToken), functionalType)
                ? selectMany
                : null;

        private static bool IsIdentity(SymbolInfo symbolInfo, INamedTypeSymbol functionalType)
            => (symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault()) is IMethodSymbol { Name: IdentityMethodName } identity
               && SymbolEqualityComparer.Default.Equals(identity.ContainingType, functionalType);

        private static SimpleNameSyntax? GetMethodName(InvocationExpressionSyntax invocation)
            => invocation.Expression switch
            {
                SimpleNameSyntax simpleName => simpleName,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
                _ => null,
            };

        private static ExpressionSyntax UnitValue(Symbols symbols)
            => ParseExpression($"global::{symbols.UnitType.ToDisplayString()}.{UnitValuePropertyName}").WithAdditionalAnnotations(Simplifier.Annotation);
    }

    private sealed class ReplaceWithThenCodeFixAction(Document document, Symbols symbols, Rewrite rewrite) : CodeAction
    {
        private static readonly SyntaxAnnotation ReplacementAnnotation = new();

        public override string Title => $"Replace Option.{OptionFromBooleanMethodName} with {BooleanThenMethodName}";

        public override string EquivalenceKey => nameof(OptionFromBooleanCodeFix);

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (root is null || semanticModel is null)
            {
                return document;
            }

            var replacement = BooleanThen.Invocation(rewrite.Condition, rewrite.ThenName, rewrite.ThenArgument)
                .WithTriviaFrom(rewrite.Target)
                .WithAdditionalAnnotations(ReplacementAnnotation);
            var rootWithReplacement = root.ReplaceNode(rewrite.Target, replacement);

            // The Then extension needs its namespace in scope; the using is only added when the call does not bind without it.
            return BooleanThen.IsThen(semanticModel.GetSpeculativeSymbolInfo(rewrite.Target.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol, symbols.BooleanExtensionsType)
                ? document.WithSyntaxRoot(rootWithReplacement)
                : document.WithSyntaxRoot(UsingDirectives.AddUsing(rootWithReplacement, rootWithReplacement.GetAnnotatedNodes(ReplacementAnnotation).Single(), BooleanThen.BooleanExtensionsNamespace));
        }
    }
}
